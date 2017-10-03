// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Syntax
{
    internal struct NullCheckExpression
    {
        public NullCheckExpression(
            ExpressionSyntax containingNode,
            ExpressionSyntax expression,
            NullCheckKind kind)
        {
            ContainingNode = containingNode;
            Expression = expression;
            Kind = kind;
        }

        public ExpressionSyntax ContainingNode { get; }

        public ExpressionSyntax Expression { get; }

        public NullCheckKind Kind { get; }

        public bool IsCheckingNull
        {
            get { return Kind == NullCheckKind.EqualsToNull || Kind == NullCheckKind.NotHasValue; }
        }

        public bool IsCheckingNotNull
        {
            get { return Kind == NullCheckKind.NotEqualsToNull || Kind == NullCheckKind.HasValue; }
        }

        public static bool TryCreate(
            SyntaxNode node,
            SemanticModel semanticModel,
            out NullCheckExpression result,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (node != null)
            {
                var expression = node as ExpressionSyntax;

                if (expression != null)
                {
                    expression = expression.WalkDownParentheses();

                    SyntaxKind kind = expression.Kind();

                    switch (kind)
                    {
                        case SyntaxKind.EqualsExpression:
                        case SyntaxKind.NotEqualsExpression:
                            {
                                var binaryExpression = (BinaryExpressionSyntax)expression;

                                ExpressionSyntax left = binaryExpression.Left?.WalkDownParentheses();
                                ExpressionSyntax right = binaryExpression.Right?.WalkDownParentheses();

                                return TryCreate(binaryExpression, kind, left, right, semanticModel, cancellationToken, out result)
                                    || TryCreate(binaryExpression, kind, right, left, semanticModel, cancellationToken, out result);
                            }
                        case SyntaxKind.SimpleMemberAccessExpression:
                            {
                                if (SyntaxUtility.IsPropertyOfNullableOfT(expression, "HasValue", semanticModel, cancellationToken))
                                {
                                    var memberAccessExpression = (MemberAccessExpressionSyntax)expression;

                                    result = new NullCheckExpression(expression, memberAccessExpression.Expression, NullCheckKind.HasValue);
                                    return true;
                                }

                                break;
                            }
                        case SyntaxKind.LogicalNotExpression:
                            {
                                var logicalNotExpression = (PrefixUnaryExpressionSyntax)expression;

                                ExpressionSyntax operand = logicalNotExpression.Operand?.WalkDownParentheses();

                                if (operand?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true
                                    && SyntaxUtility.IsPropertyOfNullableOfT(operand, "HasValue", semanticModel, cancellationToken))
                                {
                                    var memberAccessExpression = (MemberAccessExpressionSyntax)operand;

                                    result = new NullCheckExpression(expression, memberAccessExpression.Expression, NullCheckKind.NotHasValue);
                                    return true;
                                }

                                break;
                            }
                    }
                }
            }

            result = default(NullCheckExpression);
            return false;
        }

        private static bool TryCreate(
            BinaryExpressionSyntax binaryExpression,
            SyntaxKind kind,
            ExpressionSyntax expression1,
            ExpressionSyntax expression2,
            SemanticModel semanticModel,
            CancellationToken cancellationToken,
            out NullCheckExpression result)
        {
            if (expression1 != null
                && expression2 != null)
            {
                switch (expression1.Kind())
                {
                    case SyntaxKind.NullLiteralExpression:
                        {
                            result = new NullCheckExpression(
                                binaryExpression,
                                expression2,
                                (kind == SyntaxKind.EqualsExpression) ? NullCheckKind.EqualsToNull : NullCheckKind.NotEqualsToNull);

                            return true;
                        }
                    case SyntaxKind.TrueLiteralExpression:
                        {
                            if (expression2?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true
                                && SyntaxUtility.IsPropertyOfNullableOfT(expression2, "HasValue", semanticModel, cancellationToken))
                            {
                                result = new NullCheckExpression(
                                    binaryExpression,
                                    ((MemberAccessExpressionSyntax)expression2).Expression,
                                    (kind == SyntaxKind.EqualsExpression) ? NullCheckKind.HasValue : NullCheckKind.NotHasValue);

                                return true;
                            }

                            break;
                        }
                    case SyntaxKind.FalseLiteralExpression:
                        {
                            if (expression2?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true
                                && SyntaxUtility.IsPropertyOfNullableOfT(expression2, "HasValue", semanticModel, cancellationToken))
                            {
                                result = new NullCheckExpression(
                                    binaryExpression,
                                    ((MemberAccessExpressionSyntax)expression2).Expression,
                                    (kind == SyntaxKind.EqualsExpression) ? NullCheckKind.NotHasValue : NullCheckKind.HasValue);

                                return true;
                            }

                            break;
                        }
                }
            }

            result = default(NullCheckExpression);
            return false;
        }

        public override string ToString()
        {
            return ContainingNode?.ToString() ?? base.ToString();
        }
    }
}
