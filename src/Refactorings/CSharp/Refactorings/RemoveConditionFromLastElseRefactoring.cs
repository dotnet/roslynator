// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveConditionFromLastElseRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, ElseClauseSyntax elseClause)
        {
            if (elseClause.Statement?.Kind() == SyntaxKind.IfStatement
                && ((IfStatementSyntax)elseClause.Statement).Else == null)
            {
                context.RegisterRefactoring(
                    "Remove condition",
                    cancellationToken => RefactorAsync(context.Document, elseClause, cancellationToken));
            }
        }

        private static Task<Document> RefactorAsync(
            Document document,
            ElseClauseSyntax elseClause,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var ifStatement = (IfStatementSyntax)elseClause.Statement;

            ElseClauseSyntax newElseClause = elseClause
                .WithElseKeyword(
                    elseClause.ElseKeyword
                        .WithTrailingTrivia(ifStatement.CloseParenToken.TrailingTrivia))
                .WithStatement(ifStatement.Statement);

            return document.ReplaceNodeAsync(elseClause, newElseClause, cancellationToken);
        }
    }
}