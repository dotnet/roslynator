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
                        var binaryExpression = (BinaryExpressionSyntax)expression;

                        return BinaryExpression(
                            NegateExpressionKind(expression.Kind()),
                            binaryExpression.Left,
                            Token(NegateOperatorTokenKind(binaryExpression.OperatorToken.Kind()))
                                .WithTriviaFrom(binaryExpression.OperatorToken),
                            binaryExpression.Right
                        ).WithTriviaFrom(binaryExpression);
                    }
            }

            return PrefixUnaryExpression(
                SyntaxKind.LogicalNotExpression,
                ParenthesizedExpression(expression));
        }

        private static SyntaxKind NegateOperatorTokenKind(this SyntaxKind operatorKind)
        {
            switch (operatorKind)
            {
                case SyntaxKind.EqualsEqualsToken:
                    return SyntaxKind.ExclamationEqualsToken;
                case SyntaxKind.ExclamationEqualsToken:
                    return SyntaxKind.EqualsEqualsToken;
                case SyntaxKind.GreaterThanToken:
                    return SyntaxKind.LessThanEqualsToken;
                case SyntaxKind.GreaterThanEqualsToken:
                    return SyntaxKind.LessThanToken;
                case SyntaxKind.LessThanToken:
                    return SyntaxKind.GreaterThanEqualsToken;
                case SyntaxKind.LessThanEqualsToken:
                    return SyntaxKind.GreaterThanToken;
            }

            Debug.Assert(false, operatorKind.ToString());
            return SyntaxKind.None;
        }

        private static SyntaxKind NegateExpressionKind(this SyntaxKind expressionKind)
        {
            switch (expressionKind)
            {
                case SyntaxKind.EqualsExpression:
                    return SyntaxKind.NotEqualsExpression;
                case SyntaxKind.NotEqualsExpression:
                    return SyntaxKind.EqualsExpression;
                case SyntaxKind.GreaterThanExpression:
                    return SyntaxKind.LessThanOrEqualExpression;
                case SyntaxKind.GreaterThanOrEqualExpression:
                    return SyntaxKind.LessThanExpression;
                case SyntaxKind.LessThanExpression:
                    return SyntaxKind.GreaterThanOrEqualExpression;
                case SyntaxKind.LessThanOrEqualExpression:
                    return SyntaxKind.GreaterThanExpression;
            }

            Debug.Assert(false, expressionKind.ToString());
            return SyntaxKind.None;
        }
    }
}
