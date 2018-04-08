// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SwapExpressionsInBinaryExpressionRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, BinaryExpressionSyntax binaryExpression)
        {
            if (CanRefactor(binaryExpression))
            {
                context.RegisterRefactoring(
                    "Swap expressions",
                    cancellationToken => RefactorAsync(context.Document, binaryExpression, cancellationToken));
            }
        }

        public static bool CanRefactor(BinaryExpressionSyntax binaryExpression)
        {
            SyntaxKind kind = binaryExpression.Kind();

            switch (kind)
            {
                case SyntaxKind.LogicalAndExpression:
                case SyntaxKind.LogicalOrExpression:
                case SyntaxKind.AddExpression:
                case SyntaxKind.MultiplyExpression:
                    {
                        return binaryExpression.Left?.IsKind(kind) == false
                            && IsNotNullLiteralOrBooleanLiteral(binaryExpression.Right);
                    }
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                case SyntaxKind.GreaterThanExpression:
                case SyntaxKind.GreaterThanOrEqualExpression:
                case SyntaxKind.LessThanExpression:
                case SyntaxKind.LessThanOrEqualExpression:
                    {
                        return binaryExpression?.IsMissing == false
                            && IsNotNullLiteralOrBooleanLiteral(binaryExpression.Right);
                    }
            }

            return false;
        }

        private static bool IsNotNullLiteralOrBooleanLiteral(ExpressionSyntax expression)
        {
            return expression?.IsKind(
                SyntaxKind.NullLiteralExpression,
                SyntaxKind.TrueLiteralExpression,
                SyntaxKind.FalseLiteralExpression) == false;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ExpressionSyntax left = binaryExpression.Left;
            ExpressionSyntax right = binaryExpression.Right;
            SyntaxToken token = binaryExpression.OperatorToken;

            ExpressionSyntax newNode = binaryExpression
                .WithOperatorToken(
                    Token(token.LeadingTrivia, GetOperatorTokenKind(token.Kind()), token.TrailingTrivia))
                .WithLeft(right.WithTriviaFrom(left))
                .WithRight(left.WithTriviaFrom(right));

            return document.ReplaceNodeAsync(binaryExpression, newNode, cancellationToken);
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
