// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class ExpressionSyntaxExtensions
    {
        public static ExpressionSyntax Negate(this ExpressionSyntax expression)
        {
            return NegateInternal(expression)
                .WithTriviaFrom(expression);
        }

        private static ExpressionSyntax NegateInternal(this ExpressionSyntax expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            switch (expression.Kind())
            {
                case SyntaxKind.ParenthesizedExpression:
                    {
                        var parenthesizedExpression = (ParenthesizedExpressionSyntax)expression;

                        return parenthesizedExpression
                            .WithExpression(Negate(parenthesizedExpression.Expression));
                    }
                case SyntaxKind.TrueLiteralExpression:
                    {
                        return LiteralExpression(SyntaxKind.FalseLiteralExpression);
                    }
                case SyntaxKind.FalseLiteralExpression:
                    {
                        return LiteralExpression(SyntaxKind.TrueLiteralExpression);
                    }
                case SyntaxKind.IdentifierName:
                case SyntaxKind.InvocationExpression:
                case SyntaxKind.SimpleMemberAccessExpression:
                    {
                        return PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, expression);
                    }
                case SyntaxKind.LogicalNotExpression:
                    {
                        return ((PrefixUnaryExpressionSyntax)expression).Operand;
                    }
                case SyntaxKind.LogicalAndExpression:
                    {
                        var binaryExpression = (BinaryExpressionSyntax)expression;

                        return BinaryExpression(
                            SyntaxKind.LogicalOrExpression,
                            binaryExpression.Left?.Negate().WithTriviaFrom(binaryExpression.Left),
                            Token(SyntaxKind.BarBarToken).WithTriviaFrom(binaryExpression.OperatorToken),
                            binaryExpression.Right?.Negate().WithTriviaFrom(binaryExpression.Right));

                    }
                case SyntaxKind.LogicalOrExpression:
                    {
                        var binaryExpression = (BinaryExpressionSyntax)expression;

                        return BinaryExpression(
                            SyntaxKind.LogicalAndExpression,
                            binaryExpression.Left?.Negate().WithTriviaFrom(binaryExpression.Left),
                            Token(SyntaxKind.AmpersandAmpersandToken).WithTriviaFrom(binaryExpression.OperatorToken),
                            binaryExpression.Right?.Negate().WithTriviaFrom(binaryExpression.Right));

                    }
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                case SyntaxKind.GreaterThanExpression:
                case SyntaxKind.GreaterThanOrEqualExpression:
                case SyntaxKind.LessThanExpression:
                case SyntaxKind.LessThanOrEqualExpression:
                    {
                        return ((BinaryExpressionSyntax)expression)
                            .WithOperatorToken(Token(GetOperatorTokenKind(expression.Kind())));
                    }
            }

            return PrefixUnaryExpression(
                SyntaxKind.LogicalNotExpression,
                ParenthesizedExpression(expression));
        }

        private static bool IsConversionDefined(this ExpressionSyntax expression)
        {
            switch (expression?.Kind())
            {
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                case SyntaxKind.IdentifierName:
                case SyntaxKind.InvocationExpression:
                case SyntaxKind.SimpleMemberAccessExpression:
                case SyntaxKind.LogicalNotExpression:
                case SyntaxKind.LogicalAndExpression:
                case SyntaxKind.LogicalOrExpression:
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                case SyntaxKind.GreaterThanExpression:
                case SyntaxKind.GreaterThanOrEqualExpression:
                case SyntaxKind.LessThanExpression:
                case SyntaxKind.LessThanOrEqualExpression:
                    return true;
                default:
                    return false;
            }
        }

        private static SyntaxKind GetOperatorTokenKind(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.EqualsExpression:
                    return SyntaxKind.ExclamationEqualsToken;
                case SyntaxKind.NotEqualsExpression:
                    return SyntaxKind.EqualsEqualsToken;
                case SyntaxKind.GreaterThanExpression:
                    return SyntaxKind.LessThanEqualsToken;
                case SyntaxKind.GreaterThanOrEqualExpression:
                    return SyntaxKind.LessThanToken;
                case SyntaxKind.LessThanExpression:
                    return SyntaxKind.GreaterThanOrEqualExpression;
                case SyntaxKind.LessThanOrEqualExpression:
                    return SyntaxKind.GreaterThanToken;
                default:
                    {
                        Debug.Assert(false, kind.ToString());
                        return SyntaxKind.None;
                    }
            }
        }
    }
}
