// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.Analysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings
{
    internal static class RemoveBracesRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, BlockSyntax block)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveBraces)
                && CanRefactor(context, block))
            {
                context.RegisterRefactoring(
                    "Remove braces",
                    cancellationToken => RefactorAsync(context.Document, block, cancellationToken));
            }
        }

        private static bool CanRefactor(RefactoringContext context, BlockSyntax block)
        {
            if (context.Span.IsEmpty
                && EmbeddedStatementAnalysis.IsEmbeddableBlock(block))
            {
                StatementSyntax statement = EmbeddedStatementAnalysis.GetEmbeddedStatement(block.Statements[0]);

                return statement == null
                    || !statement.FullSpan.Contains(context.Span);
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            BlockSyntax block,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            StatementSyntax statement = block.Statements[0];

            if (block.Parent?.IsKind(SyntaxKind.ElseClause) == true
                && statement.IsKind(SyntaxKind.IfStatement))
            {
                var elseClause = (ElseClauseSyntax)block.Parent;

                ElseClauseSyntax newElseClause = elseClause
                    .WithStatement(statement)
                    .WithElseKeyword(elseClause.ElseKeyword.WithoutTrailingTrivia())
                    .WithFormatterAnnotation();

                SyntaxNode newRoot = oldRoot.ReplaceNode(elseClause, newElseClause);

                return document.WithSyntaxRoot(newRoot);
            }
            else
            {
                StatementSyntax newNode = statement.TrimLeadingTrivia()
                    .WithFormatterAnnotation();

                SyntaxNode newRoot = oldRoot.ReplaceNode(block, newNode);

                return document.WithSyntaxRoot(newRoot);
            }
        }
    }
}
