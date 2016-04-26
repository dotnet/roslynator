// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class UseLogicalNotOperatorRefactoring
    {
        public static bool CanRefactor(LiteralExpressionSyntax literalExpression)
        {
            if (literalExpression == null)
                throw new ArgumentNullException(nameof(literalExpression));

            var binaryExpression = literalExpression.Parent as BinaryExpressionSyntax;

            if (binaryExpression != null
                && binaryExpression.Left != null
                && binaryExpression.Right != null)
            {
                switch (literalExpression.Kind())
                {
                    case SyntaxKind.TrueLiteralExpression:
                        return binaryExpression.IsKind(SyntaxKind.NotEqualsExpression);
                    case SyntaxKind.FalseLiteralExpression:
                        return binaryExpression.IsKind(SyntaxKind.EqualsExpression);
                }
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            LiteralExpressionSyntax literalExpression,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            var binaryExpression = (BinaryExpressionSyntax)literalExpression.Parent;

            SyntaxNode newNode = UseLogicalNotExpression(binaryExpression)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(binaryExpression, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static ExpressionSyntax UseLogicalNotExpression(BinaryExpressionSyntax binaryExpression)
        {
            if (binaryExpression.Left.IsAnyKind(SyntaxKind.TrueLiteralExpression, SyntaxKind.FalseLiteralExpression))
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
