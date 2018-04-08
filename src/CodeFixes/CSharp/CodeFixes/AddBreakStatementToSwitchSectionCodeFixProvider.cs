// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddBreakStatementToSwitchSectionCodeFixProvider))]
    [Shared]
    public class AddBreakStatementToSwitchSectionCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    CompilerDiagnosticIdentifiers.ControlCannotFallThroughFromOneCaseLabelToAnother,
                    CompilerDiagnosticIdentifiers.ControlCannotFallOutOfSwitchFromFinalCaseLabel);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (!Settings.IsCodeFixEnabled(CodeFixIdentifiers.AddBreakStatementToSwitchSection))
                return;

            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out SwitchSectionSyntax switchSection))
                return;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case CompilerDiagnosticIdentifiers.ControlCannotFallThroughFromOneCaseLabelToAnother:
                    case CompilerDiagnosticIdentifiers.ControlCannotFallOutOfSwitchFromFinalCaseLabel:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Add break statement",
                                cancellationToken => RefactorAsync(context.Document, switchSection, cancellationToken),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            SwitchSectionSyntax switchSection,
            CancellationToken cancellationToken)
        {
            SwitchSectionSyntax newNode = switchSection;

            SyntaxList<StatementSyntax> statements = switchSection.Statements;

            if (statements.Count == 1
                && statements.First().IsKind(SyntaxKind.Block))
            {
                var block = (BlockSyntax)statements.First();
                newNode = newNode.ReplaceNode(block, block.AddStatements(BreakStatement()));
            }
            else
            {
                newNode = switchSection.AddStatements(BreakStatement());
            }

            return document.ReplaceNodeAsync(switchSection, newNode, cancellationToken);
        }
    }
}
