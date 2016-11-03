// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceDoStatementWithWhileStatementRefactoring
    {
        public static bool CanRefactor(DoStatementSyntax doStatement)
        {
            return doStatement.Condition?.IsKind(SyntaxKind.TrueLiteralExpression) == true;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            DoStatementSyntax doStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxTriviaList trailingTrivia = doStatement.Statement
                .GetTrailingTrivia()
                .AddRange(doStatement.CloseParenToken.TrailingTrivia)
                .AddRange(doStatement.SemicolonToken.LeadingTrivia)
                .AddRange(doStatement.SemicolonToken.TrailingTrivia);

            WhileStatementSyntax newNode = WhileStatement(
                doStatement.WhileKeyword.WithLeadingTrivia(doStatement.DoKeyword.LeadingTrivia),
                doStatement.OpenParenToken,
                doStatement.Condition,
                doStatement.CloseParenToken.WithTrailingTrivia(doStatement.DoKeyword.TrailingTrivia),
                doStatement.Statement.WithTrailingTrivia(trailingTrivia));

            newNode = newNode.WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(doStatement, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
