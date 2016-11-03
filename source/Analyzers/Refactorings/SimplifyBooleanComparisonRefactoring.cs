// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SimplifyBooleanComparisonRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newNode = Refactor(binaryExpression)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(binaryExpression, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static ExpressionSyntax Refactor(BinaryExpressionSyntax binaryExpression)
        {
            if (binaryExpression.Left.IsKind(SyntaxKind.TrueLiteralExpression, SyntaxKind.FalseLiteralExpression))
            {
                ExpressionSyntax expression = binaryExpression.Right.Negate();

                SyntaxTriviaList triviaList = SyntaxFactory.TriviaList()
                    .AddRange(binaryExpression.Left.GetLeadingTrivia())
                    .AddRange(binaryExpression.Left.GetTrailingTrivia())
                    .AddRange(binaryExpression.OperatorToken.LeadingTrivia)
                    .AddRange(binaryExpression.OperatorToken.TrailingTrivia)
                    .AddRange(expression.GetLeadingTrivia());

                return expression.WithLeadingTrivia(triviaList);
            }
            else
            {
                ExpressionSyntax expression = binaryExpression.Left.Negate();

                SyntaxTriviaList triviaList = SyntaxFactory.TriviaList()
                    .AddRange(expression.GetTrailingTrivia())
                    .AddRange(binaryExpression.OperatorToken.LeadingTrivia)
                    .AddRange(binaryExpression.OperatorToken.TrailingTrivia)
                    .AddRange(binaryExpression.Right.GetLeadingTrivia())
                    .AddRange(binaryExpression.Right.GetTrailingTrivia());

                return expression.WithTrailingTrivia(triviaList);
            }
        }
    }
}
