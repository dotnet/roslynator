// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp
{
    internal static class Inverter
    {
        public static ExpressionSyntax LogicallyNegate(
            ExpressionSyntax expression,
            SemanticModel semanticModel = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            ExpressionSyntax newExpression = LogicallyNegateImpl(expression, semanticModel, cancellationToken);

            return newExpression.WithTriviaFrom(expression);
        }

        private static ParenthesizedExpressionSyntax LogicallyNegateAndParenthesize(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (expression == null)
                return null;

            return LogicallyNegateImpl(expression, semanticModel, cancellationToken).Parenthesize();
        }

        private static ExpressionSyntax LogicallyNegateImpl(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (expression == null)
                return expression;

            switch (expression.Kind())
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
                        return DefaultInvert(expression);
                    }
                case SyntaxKind.LogicalNotExpression:
                    {
                        return ((PrefixUnaryExpressionSyntax)expression).Operand;
                    }
                case SyntaxKind.CastExpression:
                    {
                        return DefaultInvert(expression);
                    }
                case SyntaxKind.LessThanExpression:
                case SyntaxKind.LessThanOrEqualExpression:
                case SyntaxKind.GreaterThanExpression:
                case SyntaxKind.GreaterThanOrEqualExpression:
                    {
                        return (semanticModel != null)
                            ? InvertLessThanGreaterThan((BinaryExpressionSyntax)expression, semanticModel, cancellationToken)
                            : DefaultInvert(expression);
                    }
                case SyntaxKind.IsExpression:
                case SyntaxKind.AsExpression:
                case SyntaxKind.IsPatternExpression:
                    {
                        return DefaultInvert(expression);
                    }
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                    {
                        return InvertBinaryExpression((BinaryExpressionSyntax)expression);
                    }
                case SyntaxKind.BitwiseAndExpression:
                    {
                        return InvertBinaryExpression((BinaryExpressionSyntax)expression, semanticModel, cancellationToken);
                    }
                case SyntaxKind.ExclusiveOrExpression:
                    {
                        return DefaultInvert(expression);
                    }
                case SyntaxKind.BitwiseOrExpression:
                case SyntaxKind.LogicalOrExpression:
                case SyntaxKind.LogicalAndExpression:
                    {
                        return InvertBinaryExpression((BinaryExpressionSyntax)expression, semanticModel, cancellationToken);
                    }
                case SyntaxKind.ConditionalExpression:
                    {
                        return InvertConditionalExpression((ConditionalExpressionSyntax)expression, semanticModel, cancellationToken);
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
                        return DefaultInvert(expression);
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

                        ExpressionSyntax expression2 = parenthesizedExpression.Expression;

                        if (expression2 == null)
                            return parenthesizedExpression;

                        if (expression2.IsMissing)
                            return parenthesizedExpression;

                        ExpressionSyntax newExpression = LogicallyNegateImpl(expression2, semanticModel, cancellationToken);

                        newExpression = newExpression.WithTriviaFrom(expression2);

                        return parenthesizedExpression.WithExpression(newExpression);
                    }
                case SyntaxKind.AwaitExpression:
                    {
                        return DefaultInvert(expression);
                    }
            }

            Debug.Fail($"Logical negation of unknown kind '{expression.Kind()}'");

            return DefaultInvert(expression);
        }

        private static ExpressionSyntax InvertLessThanGreaterThan(
            BinaryExpressionSyntax binaryExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax left = binaryExpression.Left;
            ExpressionSyntax right = binaryExpression.Right;

            if (IsConstructedFromNullableOfT(left))
            {
                if (!IsConstructedFromNullableOfT(right))
                {
                    return InvertLessThanGreaterThan(binaryExpression, left, right, isLeft: true);
                }
                else
                {
                    return DefaultInvert(binaryExpression);
                }
            }
            else if (IsConstructedFromNullableOfT(right))
            {
                if (semanticModel.HasConstantValue(left, cancellationToken))
                {
                    return InvertLessThanGreaterThan(binaryExpression, right, left, isLeft: false);
                }
                else
                {
                    return DefaultInvert(binaryExpression);
                }
            }

            return InvertBinaryExpression(binaryExpression);

            bool IsConstructedFromNullableOfT(ExpressionSyntax expression)
            {
                return expression?.IsMissing == false
                    && expression.Kind() != SyntaxKind.NumericLiteralExpression
                    && semanticModel
                        .GetTypeSymbol(expression, cancellationToken)?
                        .IsNullableType() == true;
            }
        }

        private static ExpressionSyntax InvertLessThanGreaterThan(
            BinaryExpressionSyntax binaryExpression,
            ExpressionSyntax expression,
            ExpressionSyntax otherExpression,
            bool isLeft)
        {
            if (expression.Kind() == SyntaxKind.IdentifierName)
            {
                return LogicalOrExpression(
                    EqualsExpression(expression, NullLiteralExpression()),
                    InvertBinaryExpression(binaryExpression));
            }

            if (!(expression is ConditionalAccessExpressionSyntax conditionalAccess))
                return DefaultInvert(binaryExpression);

            if (conditionalAccess.Expression.Kind() != SyntaxKind.IdentifierName)
                return DefaultInvert(binaryExpression);

            ExpressionSyntax newExpression = TryCreateExpressionWithoutConditionalAccess(conditionalAccess);

            if (newExpression == null)
                return DefaultInvert(binaryExpression);

            return LogicalOrExpression(
                EqualsExpression(conditionalAccess.Expression, NullLiteralExpression()),
                binaryExpression.Update(
                    (isLeft) ? newExpression : otherExpression,
                    InvertBinaryOperatorToken(binaryExpression.OperatorToken),
                    (isLeft) ? otherExpression : newExpression));
        }

        private static ExpressionSyntax TryCreateExpressionWithoutConditionalAccess(ConditionalAccessExpressionSyntax conditionalAccess)
        {
            switch (conditionalAccess.WhenNotNull)
            {
                case MemberBindingExpressionSyntax memberBinding:
                    {
                        return SimpleMemberAccessExpression(conditionalAccess.Expression, memberBinding.Name);
                    }
                case ElementBindingExpressionSyntax elementBinding:
                    {
                        return ElementAccessExpression(conditionalAccess.Expression, elementBinding.ArgumentList);
                    }
                case InvocationExpressionSyntax invocation:
                    {
                        if (!(invocation.Expression is MemberBindingExpressionSyntax memberBinding))
                            return null;

                        return InvocationExpression(SimpleMemberAccessExpression(conditionalAccess.Expression, memberBinding.Name), invocation.ArgumentList);
                    }
            }

            return null;
        }

        private static BinaryExpressionSyntax InvertBinaryExpression(BinaryExpressionSyntax binaryExpression)
        {
            SyntaxToken operatorToken = InvertBinaryOperatorToken(binaryExpression.OperatorToken);

            return binaryExpression.WithOperatorToken(operatorToken);
        }

        private static SyntaxToken InvertBinaryOperatorToken(SyntaxToken operatorToken)
        {
            return Token(
                operatorToken.LeadingTrivia,
                InvertBinaryOperator(operatorToken.Kind()),
                operatorToken.TrailingTrivia);

            SyntaxKind InvertBinaryOperator(SyntaxKind kind)
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
                }

                Debug.Fail(kind.ToString());
                return kind;
            }
        }

        private static BinaryExpressionSyntax InvertBinaryExpression(
            BinaryExpressionSyntax binaryExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax left = binaryExpression.Left;
            ExpressionSyntax right = binaryExpression.Right;
            SyntaxToken operatorToken = binaryExpression.OperatorToken;

            SyntaxKind kind = InvertBinaryExpressionKind(binaryExpression.Kind());

            left = LogicallyNegateAndParenthesize(left, semanticModel, cancellationToken);

            right = LogicallyNegateAndParenthesize(right, semanticModel, cancellationToken);

            BinaryExpressionSyntax newBinaryExpression = BinaryExpression(
                kind,
                left,
                InvertBinaryOperatorToken(operatorToken),
                right);

            return newBinaryExpression.WithTriviaFrom(binaryExpression);
        }

        private static SyntaxKind InvertBinaryExpressionKind(SyntaxKind kind)
        {
            switch (kind)
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
            }

            Debug.Fail(kind.ToString());
            return kind;
        }

        private static ConditionalExpressionSyntax InvertConditionalExpression(
            ConditionalExpressionSyntax conditionalExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax whenTrue = conditionalExpression.WhenTrue;
            ExpressionSyntax whenFalse = conditionalExpression.WhenFalse;

            if (whenTrue?.IsKind(SyntaxKind.ThrowExpression) == false)
            {
                whenTrue = LogicallyNegateAndParenthesize(whenTrue, semanticModel, cancellationToken);
            }

            if (whenFalse?.IsKind(SyntaxKind.ThrowExpression) == false)
            {
                whenFalse = LogicallyNegateAndParenthesize(whenFalse, semanticModel, cancellationToken);
            }

            ConditionalExpressionSyntax newConditionalExpression = conditionalExpression.Update(
                conditionalExpression.Condition,
                conditionalExpression.QuestionToken,
                whenTrue,
                conditionalExpression.ColonToken,
                whenFalse);

            return newConditionalExpression.WithTriviaFrom(conditionalExpression);
        }

        private static PrefixUnaryExpressionSyntax DefaultInvert(ExpressionSyntax expression)
        {
            Debug.Assert(expression.Kind() != SyntaxKind.ParenthesizedExpression, expression.Kind().ToString());

            SyntaxTriviaList leadingTrivia = expression.GetLeadingTrivia();

            return LogicalNotExpression(
                expression.WithoutLeadingTrivia().Parenthesize(),
                Token(leadingTrivia, SyntaxKind.ExclamationToken, SyntaxTriviaList.Empty));
        }
    }
}
