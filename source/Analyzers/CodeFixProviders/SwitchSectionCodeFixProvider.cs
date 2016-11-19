// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixProviders
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SwitchSectionCodeFixProvider))]
    [Shared]
    public class SwitchSectionCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.RemoveRedundantDefaultSwitchSection,
                    DiagnosticIdentifiers.DefaultLabelShouldBeLastLabelInSwitchSection,
                    DiagnosticIdentifiers.AddBracesToSwitchSectionWithMultipleStatements);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.Document
                .GetSyntaxRootAsync(context.CancellationToken)
                .ConfigureAwait(false);

            SwitchSectionSyntax switchSection = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<SwitchSectionSyntax>();

            Debug.Assert(switchSection != null, $"{nameof(switchSection)} is null");

            if (switchSection == null)
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.RemoveRedundantDefaultSwitchSection:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Remove redundant switch section",
                                cancellationToken => RemoveRedundantSwitchSectionAsync(context.Document, switchSection, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.DefaultLabelShouldBeLastLabelInSwitchSection:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Move default label to the last position",
                                cancellationToken => MoveDefaultLabelToLastPositionAsync(context.Document, switchSection, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.AddBracesToSwitchSectionWithMultipleStatements:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                AddBracesToSwitchSectionRefactoring.Title,
                                cancellationToken => AddBracesToSwitchSectionRefactoring.RefactorAsync(context.Document, switchSection, cancellationToken),
                                diagnostic.Id + EquivalenceKeySuffix);

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }

        private static async Task<Document> MoveDefaultLabelToLastPositionAsync(
            Document document,
            SwitchSectionSyntax switchSection,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxList<SwitchLabelSyntax> labels = switchSection.Labels;

            SwitchLabelSyntax defaultLabel = labels.First(f => f.IsKind(SyntaxKind.DefaultSwitchLabel));

            int index = labels.IndexOf(defaultLabel);

            SwitchLabelSyntax lastLabel = labels.Last();

            labels = labels.Replace(lastLabel, defaultLabel.WithTriviaFrom(lastLabel));

            labels = labels.Replace(labels[index], lastLabel.WithTriviaFrom(defaultLabel));

            SwitchSectionSyntax newSwitchSection = switchSection.WithLabels(labels);

            SyntaxNode newRoot = root.ReplaceNode(switchSection, newSwitchSection);

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> RemoveRedundantSwitchSectionAsync(
            Document document,
            SwitchSectionSyntax switchSection,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var switchStatement = (SwitchStatementSyntax)switchSection.Parent;

            SwitchStatementSyntax newSwitchStatement = GetNewSwitchStatement(switchSection, switchStatement);

            SyntaxNode newRoot = oldRoot.ReplaceNode(switchStatement, newSwitchStatement);

            return document.WithSyntaxRoot(newRoot);
        }

        private static SwitchStatementSyntax GetNewSwitchStatement(SwitchSectionSyntax switchSection, SwitchStatementSyntax switchStatement)
        {
            if (switchSection.GetLeadingTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                int index = switchStatement.Sections.IndexOf(switchSection);

                if (index > 0)
                {
                    SwitchSectionSyntax previousSection = switchStatement.Sections[index - 1];

                    if (previousSection.GetTrailingTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                    {
                        SwitchStatementSyntax newSwitchStatement = switchStatement.RemoveNode(
                            switchSection,
                            SyntaxRemoveOptions.KeepNoTrivia);

                        previousSection = newSwitchStatement.Sections[index - 1];

                        return newSwitchStatement.ReplaceNode(
                            previousSection,
                            previousSection.WithTrailingTrivia(switchSection.GetTrailingTrivia()));
                    }
                }
                else
                {
                    SyntaxToken openBrace = switchStatement.OpenBraceToken;

                    if (!openBrace.IsMissing
                        && openBrace.TrailingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                    {
                        return switchStatement
                            .RemoveNode(switchSection, SyntaxRemoveOptions.KeepNoTrivia)
                            .WithOpenBraceToken(openBrace.WithTrailingTrivia(switchSection.GetTrailingTrivia()));
                    }
                }
            }

            return switchStatement.RemoveNode(switchSection, SyntaxRemoveOptions.KeepExteriorTrivia);
        }
    }
}
