// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ElseClauseRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, ElseClauseSyntax elseClause)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveConditionFromLastElse)
                && elseClause.ElseKeyword.Span.Contains(context.Span)
                && elseClause.Statement?.IsKind(SyntaxKind.IfStatement) == true
                && ((IfStatementSyntax)elseClause.Statement).Else == null)
            {
                context.RegisterRefactoring(
                    "Remove condition",
                    cancellationToken =>
                    {
                        return RemoveConditionFromLastElseAsync(
                            context.Document,
                            elseClause,
                            cancellationToken);
                    });
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddBraces)
                && elseClause.Statement?.IsKind(SyntaxKind.IfStatement) == true)
            {
                var ifStatement = (IfStatementSyntax)elseClause.Statement;

                if (ifStatement.Else == null
                    && (ifStatement.IfKeyword.Span.Contains(context.Span)
                        || context.Span.IsBetweenSpans(ifStatement)))
                {
                    AddBracesRefactoring.RegisterRefactoring(context, ifStatement);
                }
            }
        }

        private static async Task<Document> RemoveConditionFromLastElseAsync(
            Document document,
            ElseClauseSyntax elseClause,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var ifStatement = (IfStatementSyntax)elseClause.Statement;

            ElseClauseSyntax newElseClause = elseClause
                .WithElseKeyword(
                    elseClause.ElseKeyword
                        .WithTrailingTrivia(ifStatement.CloseParenToken.TrailingTrivia))
                .WithStatement(ifStatement.Statement);

            SyntaxNode newRoot = oldRoot.ReplaceNode(elseClause, newElseClause);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}