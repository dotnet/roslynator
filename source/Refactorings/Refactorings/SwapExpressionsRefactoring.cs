// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SwapExpressionsRefactoring
    {
        public static bool CanRefactor(BinaryExpressionSyntax binaryExpression)
        {
            return CanRefactor(binaryExpression.Kind())
                && binaryExpression.Left?.IsMissing == false
                && binaryExpression.Right?.IsKind(
                    SyntaxKind.NullLiteralExpression,
                    SyntaxKind.TrueLiteralExpression,
                    SyntaxKind.FalseLiteralExpression) == false;
        }

        public static bool CanRefactor(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.LogicalAndExpression:
                case SyntaxKind.LogicalOrExpression:
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                case SyntaxKind.AddExpression:
                case SyntaxKind.MultiplyExpression:
                case SyntaxKind.GreaterThanExpression:
                case SyntaxKind.GreaterThanOrEqualExpression:
                case SyntaxKind.LessThanExpression:
                case SyntaxKind.LessThanOrEqualExpression:
                    return true;
                default:
                    return false;
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            ExpressionSyntax left = binaryExpression.Left;
            ExpressionSyntax right = binaryExpression.Right;
            SyntaxToken token = binaryExpression.OperatorToken;

            ExpressionSyntax newNode = binaryExpression
                .WithOperatorToken(
                    Token(token.LeadingTrivia, GetOperatorTokenKind(token.Kind()), token.TrailingTrivia))
                .WithLeft(right.WithTriviaFrom(left))
                .WithRight(left.WithTriviaFrom(right));

            SyntaxNode newRoot = root.ReplaceNode(binaryExpression, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static SyntaxKind GetOperatorTokenKind(SyntaxKind operatorKind)
        {
            switch (operatorKind)
            {
                case SyntaxKind.LessThanToken:
                    return SyntaxKind.GreaterThanToken;
                case SyntaxKind.LessThanEqualsToken:
                    return SyntaxKind.GreaterThanEqualsToken;
                case SyntaxKind.GreaterThanToken:
                    return SyntaxKind.LessThanToken;
                case SyntaxKind.GreaterThanEqualsToken:
                    return SyntaxKind.LessThanEqualsToken;
                default:
                    return operatorKind;
            }
        }
    }
}
