// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CommonRefactorings
    {
        public static Task<Document> SwapBinaryOperandsAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ExpressionSyntax left = binaryExpression.Left;
            ExpressionSyntax right = binaryExpression.Right;
            SyntaxToken token = binaryExpression.OperatorToken;

            ExpressionSyntax newBinaryExpressions = binaryExpression.Update(
                left: right.WithTriviaFrom(left),
                operatorToken: Token(token.LeadingTrivia, GetOperatorTokenKind(token.Kind()), token.TrailingTrivia),
                right: left.WithTriviaFrom(right));

            return document.ReplaceNodeAsync(binaryExpression, newBinaryExpressions, cancellationToken);

            SyntaxKind GetOperatorTokenKind(SyntaxKind kind)
            {
                switch (kind)
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
                        return kind;
                }
            }
        }
    }
}
