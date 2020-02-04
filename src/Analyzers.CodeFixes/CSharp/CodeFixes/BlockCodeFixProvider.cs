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
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(BlockCodeFixProvider))]
    [Shared]
    public class BlockCodeFixProvider : BaseCodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticIdentifiers.SimplifyLazyInitialization,
                    DiagnosticIdentifiers.FormatSingleLineBlock,
                    DiagnosticIdentifiers.RemoveUnnecessaryBraces);
            }
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out BlockSyntax block))
                return;

            Document document = context.Document;

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case DiagnosticIdentifiers.SimplifyLazyInitialization:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Simplify lazy initialization",
                                ct => SimplifyLazyInitializationRefactoring.RefactorAsync(document, block, ct),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.FormatSingleLineBlock:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Format block",
                                ct => FormatSingleLineBlockAsync(document, block, ct),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                    case DiagnosticIdentifiers.RemoveUnnecessaryBraces:
                        {
                            CodeAction codeAction = CodeAction.Create(
                                "Remove braces",
                                ct => RemoveBracesAsync(document, block, ct),
                                GetEquivalenceKey(diagnostic));

                            context.RegisterCodeFix(codeAction, diagnostic);
                            break;
                        }
                }
            }
        }

        private static Task<Document> FormatSingleLineBlockAsync(
            Document document,
            BlockSyntax block,
            CancellationToken cancellationToken)
        {
            SyntaxToken closeBrace = block.CloseBraceToken;

            BlockSyntax newBlock = block
                .WithCloseBraceToken(closeBrace.WithLeadingTrivia(closeBrace.LeadingTrivia.Add(CSharpFactory.NewLine())))
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(block, newBlock, cancellationToken);
        }

        private static Task<Document> RemoveBracesAsync(
            Document document,
            BlockSyntax block,
            CancellationToken cancellationToken)
        {
            var switchSection = (SwitchSectionSyntax)block.Parent;

            SyntaxList<StatementSyntax> statements = block.Statements;

            SyntaxTriviaList leadingTrivia = block.OpenBraceToken.LeadingTrivia;

            leadingTrivia = AddTriviaIfNecessary(leadingTrivia, block.OpenBraceToken.TrailingTrivia);
            leadingTrivia = AddTriviaIfNecessary(leadingTrivia, statements[0].GetLeadingTrivia());

            SyntaxTriviaList trailingTrivia = statements.Last().GetTrailingTrivia();

            trailingTrivia = AddTriviaIfNecessary(trailingTrivia, block.CloseBraceToken.LeadingTrivia);
            trailingTrivia = AddTriviaIfNecessary(trailingTrivia, block.CloseBraceToken.TrailingTrivia);

            trailingTrivia = trailingTrivia.TrimEnd().Add(CSharpFactory.NewLine());

            var switchStatement = (SwitchStatementSyntax)switchSection.Parent;

            if (!switchStatement.Sections.IsLast(switchSection))
                trailingTrivia = trailingTrivia.Add(CSharpFactory.NewLine());

            SyntaxList<StatementSyntax> newStatements = statements.ReplaceAt(0, statements[0].WithLeadingTrivia(leadingTrivia));

            newStatements = newStatements.ReplaceAt(newStatements.Count - 1, newStatements.Last().WithTrailingTrivia(trailingTrivia));

            SwitchSectionSyntax newSwitchSection = switchSection
                .WithStatements(newStatements)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(switchSection, newSwitchSection, cancellationToken);

            static SyntaxTriviaList AddTriviaIfNecessary(SyntaxTriviaList trivia, SyntaxTriviaList triviaToAdd)
            {
                if (triviaToAdd.Any(f => f.IsKind(SyntaxKind.SingleLineCommentTrivia)))
                    trivia = trivia.AddRange(triviaToAdd);

                return trivia;
            }
        }
    }
}