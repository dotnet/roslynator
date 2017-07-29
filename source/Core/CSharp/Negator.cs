// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp
{
    internal static class Negator
    {
        public static ExpressionSyntax LogicallyNegate(ExpressionSyntax booleanExpression)
        {
            if (booleanExpression == null)
                throw new ArgumentNullException(nameof(booleanExpression));

            return LogicallyNegateCore(booleanExpression)
                .WithTriviaFrom(booleanExpression);
        }

        private static ExpressionSyntax LogicallyNegateCore(ExpressionSyntax expression)
        {
            switch (expression?.Kind())
            {
                case SyntaxKind.SimpleMemberAccessExpression:
                case SyntaxKind.InvocationExpression:
                case SyntaxKind.ElementAccessExpression:
                case SyntaxKind.PostIncrementExpression:
                case SyntaxKind.PostDecrementExpression:
                case SyntaxKind.ObjectCreationExpression:
                case SyntaxKind.AnonymousObjectCreationExpression:
                case SyntaxKind.TypeOfExpression:
                case SyntaxKind.DefaultExpression:
                case SyntaxKind.CheckedExpression:
                case SyntaxKind.UncheckedExpression:
                case SyntaxKind.IdentifierName:
                    {
                        return LogicalNotExpression(expression);
                    }
                case SyntaxKind.LogicalNotExpression:
                    {
                        return ((PrefixUnaryExpressionSyntax)expression).Operand;
                    }
                case SyntaxKind.CastExpression:
                    {
                        return LogicalNotExpressionWithParentheses(expression);
                    }
                case SyntaxKind.LessThanExpression:
                case SyntaxKind.LessThanOrEqualExpression:
                case SyntaxKind.GreaterThanExpression:
                case SyntaxKind.GreaterThanOrEqualExpression:
                    {
                        return NegateBinaryOperator(expression);
                    }
                case SyntaxKind.IsExpression:
                case SyntaxKind.AsExpression:
                    {
                        return LogicalNotExpressionWithParentheses(expression);
                    }
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                    {
                        return NegateBinaryOperator(expression);
                    }
                case SyntaxKind.BitwiseAndExpression:
                    {
                        return NegateBinaryExpression(expression);
                    }
                case SyntaxKind.ExclusiveOrExpression:
                    {
                        return LogicalNotExpressionWithParentheses(expression);
                    }
                case SyntaxKind.BitwiseOrExpression:
                case SyntaxKind.LogicalOrExpression:
                case SyntaxKind.LogicalAndExpression:
                    {
                        return NegateBinaryExpression(expression);
                    }
                case SyntaxKind.ConditionalExpression:
                    {
                        return NegateConditionalExpression((ConditionalExpressionSyntax)expression);
                    }
                case SyntaxKind.SimpleAssignmentExpression:
                case SyntaxKind.AddAssignmentExpression:
                case SyntaxKind.SubtractAssignmentExpression:
                case SyntaxKind.MultiplyAssignmentExpression:
                case SyntaxKind.DivideAssignmentExpression:
                case SyntaxKind.ModuloAssignmentExpression:
                case SyntaxKind.AndAssignmentExpression:
                case SyntaxKind.ExclusiveOrAssignmentExpression:
                case SyntaxKind.OrAssignmentExpression:
                case SyntaxKind.LeftShiftAssignmentExpression:
                case SyntaxKind.RightShiftAssignmentExpression:
                    {
                        return LogicalNotExpressionWithParentheses(expression);
                    }
                case SyntaxKind.TrueLiteralExpression:
                    {
                        return FalseLiteralExpression();
                    }
                case SyntaxKind.FalseLiteralExpression:
                    {
                        return TrueLiteralExpression();
                    }
                case SyntaxKind.ParenthesizedExpression:
                    {
                        var parenthesizedExpression = (ParenthesizedExpressionSyntax)expression;

                        return parenthesizedExpression
                            .WithExpression(LogicallyNegate(parenthesizedExpression.Expression));
                    }
                default:
                    {
                        if (expression != null)
                        {
                            Debug.Fail($"Negate {expression.Kind()}");
                            return LogicalNotExpressionWithParentheses(expression);
                        }
                        else
                        {
                            return expression;
                        }
                    }
            }
        }

        private static ExpressionSyntax NegateBinaryOperator(ExpressionSyntax expression)
        {
            return NegateBinaryOperator((BinaryExpressionSyntax)expression);
        }

        private static ExpressionSyntax NegateBinaryOperator(BinaryExpressionSyntax binaryExpression)
        {
            SyntaxToken operatorToken = binaryExpression
                .OperatorToken
                .NegateBinaryOperator();

            return binaryExpression.WithOperatorToken(operatorToken);
        }

        private static SyntaxToken NegateBinaryOperator(this SyntaxToken operatorToken)
        {
            return Token(
                operatorToken.LeadingTrivia,
                NegateBinaryOperator(operatorToken.Kind()),
                operatorToken.TrailingTrivia);
        }

        private static SyntaxKind NegateBinaryOperator(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.LessThanToken:
                    return SyntaxKind.GreaterThanEqualsToken;
                case SyntaxKind.LessThanEqualsToken:
                    return SyntaxKind.GreaterThanToken;
                case SyntaxKind.GreaterThanToken:
                    return SyntaxKind.LessThanEqualsToken;
                case SyntaxKind.GreaterThanEqualsToken:
                    return SyntaxKind.LessThanToken;
                case SyntaxKind.EqualsEqualsToken:
                    return SyntaxKind.ExclamationEqualsToken;
                case SyntaxKind.ExclamationEqualsToken:
                    return SyntaxKind.EqualsEqualsToken;
                case SyntaxKind.AmpersandToken:
                    return SyntaxKind.BarToken;
                case SyntaxKind.BarToken:
                    return SyntaxKind.AmpersandToken;
                case SyntaxKind.BarBarToken:
                    return SyntaxKind.AmpersandAmpersandToken;
                case SyntaxKind.AmpersandAmpersandToken:
                    return SyntaxKind.BarBarToken;
                default:
                    {
                        Debug.Fail(kind.ToString());
                        return kind;
                    }
            }
        }

        private static ExpressionSyntax NegateBinaryExpression(ExpressionSyntax expression)
        {
            return NegateBinaryExpression((BinaryExpressionSyntax)expression);
        }

        private static ExpressionSyntax NegateBinaryExpression(BinaryExpressionSyntax binaryExpression)
        {
            ExpressionSyntax left = binaryExpression.Left;
            ExpressionSyntax right = binaryExpression.Right;
            SyntaxToken operatorToken = binaryExpression.OperatorToken;

            SyntaxKind kind = NegateBinaryExpressionKind(binaryExpression);

            left = LogicallyNegate(left, kind);

            right = LogicallyNegate(right, kind);

            BinaryExpressionSyntax newBinaryExpression = BinaryExpression(
                kind,
                left,
                operatorToken.NegateBinaryOperator(),
                right);

            return newBinaryExpression.WithTriviaFrom(binaryExpression);
        }

        private static SyntaxKind NegateBinaryExpressionKind(BinaryExpressionSyntax binaryExpression)
        {
            switch (binaryExpression.Kind())
            {
                case SyntaxKind.LessThanExpression:
                    return SyntaxKind.GreaterThanOrEqualExpression;
                case SyntaxKind.LessThanOrEqualExpression:
                    return SyntaxKind.GreaterThanExpression;
                case SyntaxKind.GreaterThanExpression:
                    return SyntaxKind.LessThanOrEqualExpression;
                case SyntaxKind.GreaterThanOrEqualExpression:
                    return SyntaxKind.LessThanExpression;
                case SyntaxKind.EqualsExpression:
                    return SyntaxKind.NotEqualsExpression;
                case SyntaxKind.NotEqualsExpression:
                    return SyntaxKind.EqualsExpression;
                case SyntaxKind.BitwiseAndExpression:
                    return SyntaxKind.BitwiseOrExpression;
                case SyntaxKind.BitwiseOrExpression:
                    return SyntaxKind.BitwiseAndExpression;
                case SyntaxKind.LogicalOrExpression:
                    return SyntaxKind.LogicalAndExpression;
                case SyntaxKind.LogicalAndExpression:
                    return SyntaxKind.LogicalOrExpression;
                default:
                    {
                        Debug.Fail(binaryExpression.Kind().ToString());
                        return binaryExpression.Kind();
                    }
            }
        }

        private static ExpressionSyntax NegateConditionalExpression(ConditionalExpressionSyntax conditionalExpression)
        {
            ExpressionSyntax whenTrue = conditionalExpression.WhenTrue;
            ExpressionSyntax whenFalse = conditionalExpression.WhenFalse;

            if (whenTrue?.IsKind(SyntaxKind.ThrowExpression) == false)
            {
                whenTrue = LogicallyNegate(whenTrue, SyntaxKind.ConditionalExpression);
            }

            if (whenFalse?.IsKind(SyntaxKind.ThrowExpression) == false)
            {
                whenFalse = LogicallyNegate(whenFalse, SyntaxKind.ConditionalExpression);
            }

            ConditionalExpressionSyntax newConditionalExpression = conditionalExpression.Update(
                conditionalExpression.Condition,
                conditionalExpression.QuestionToken,
                whenTrue,
                conditionalExpression.ColonToken,
                whenFalse);

            return newConditionalExpression.WithTriviaFrom(conditionalExpression);
        }

        private static ExpressionSyntax LogicallyNegate(ExpressionSyntax expression, SyntaxKind kind)
        {
            if (expression == null)
                return null;

            return LogicallyNegate(expression)
                .ParenthesizeIfNecessary(kind)
                .WithTriviaFrom(expression);
        }

        private static ExpressionSyntax ParenthesizeIfNecessary(this ExpressionSyntax expression, SyntaxKind kind)
        {
            if (expression != null
                && OperatorPrecedence.GetPrecedence(expression) > OperatorPrecedence.GetPrecedence(kind))
            {
                expression = expression.Parenthesize(simplifiable: false);
            }

            return expression;
        }

        private static ExpressionSyntax LogicalNotExpressionWithParentheses(this ExpressionSyntax expression)
        {
            if (expression?.IsMissing == false)
            {
                if (!expression.IsKind(SyntaxKind.ParenthesizedExpression))
                    expression = expression.Parenthesize(simplifiable: false);

                return LogicalNotExpression(expression);
            }

            Debug.Fail(expression.Kind().ToString());

            return expression;
        }
    }
}
