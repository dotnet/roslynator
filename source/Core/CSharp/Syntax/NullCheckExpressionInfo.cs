// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.Syntax.SyntaxInfoHelpers;

namespace Roslynator.CSharp.Syntax
{
    public struct NullCheckExpressionInfo
    {
        private static NullCheckExpressionInfo Default { get; } = new NullCheckExpressionInfo();

        private NullCheckExpressionInfo(
            ExpressionSyntax containingExpression,
            ExpressionSyntax expression,
            NullCheckKind kind)
        {
            ContainingExpression = containingExpression;
            Expression = expression;
            Kind = kind;
        }

        public ExpressionSyntax ContainingExpression { get; }

        public ExpressionSyntax Expression { get; }

        public NullCheckKind Kind { get; }

        public bool IsCheckingNull
        {
            get { return (Kind & NullCheckKind.IsNull) != 0; }
        }

        public bool IsCheckingNotNull
        {
            get { return (Kind & NullCheckKind.IsNotNull) != 0; }
        }

        public bool Success
        {
            get { return Kind != NullCheckKind.None; }
        }

        internal static NullCheckExpressionInfo Create(
            SyntaxNode node,
            NullCheckKind allowedKinds = NullCheckKind.All,
            bool walkDownParentheses = true,
            bool allowMissing = false,
            SemanticModel semanticModel = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (semanticModel == null
                && (allowedKinds & NullCheckKind.HasValueProperty) != 0)
            {
                return Default;
            }

            ExpressionSyntax expression = WalkAndCheck(node, allowMissing, walkDownParentheses);

            if (expression == null)
                return Default;

            SyntaxKind kind = expression.Kind();

            switch (kind)
            {
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                    {
                        var binaryExpression = (BinaryExpressionSyntax)expression;

                        ExpressionSyntax left = WalkAndCheck(binaryExpression.Left, allowMissing, walkDownParentheses);

                        if (left == null)
                            break;

                        ExpressionSyntax right = WalkAndCheck(binaryExpression.Right, allowMissing, walkDownParentheses);

                        if (right == null)
                            break;

                        NullCheckExpressionInfo info = Create(binaryExpression, kind, left, right, allowedKinds, allowMissing, semanticModel, cancellationToken);

                        if (info.Success)
                        {
                            return info;
                        }
                        else
                        {
                            return Create(binaryExpression, kind, right, left, allowedKinds, allowMissing, semanticModel, cancellationToken);
                        }
                    }
                case SyntaxKind.SimpleMemberAccessExpression:
                    {
                        if ((allowedKinds & NullCheckKind.HasValue) == 0)
                            break;

                        var memberAccessExpression = (MemberAccessExpressionSyntax)expression;

                        if (!IsPropertyOfNullableOfT(memberAccessExpression.Name, "HasValue", semanticModel, cancellationToken))
                            break;

                        return new NullCheckExpressionInfo(expression, memberAccessExpression.Expression, NullCheckKind.HasValue);
                    }
                case SyntaxKind.LogicalNotExpression:
                    {
                        if ((allowedKinds & NullCheckKind.NotHasValue) == 0)
                            break;

                        var logicalNotExpression = (PrefixUnaryExpressionSyntax)expression;

                        ExpressionSyntax operand = WalkAndCheck(logicalNotExpression.Operand, allowMissing, walkDownParentheses);

                        if (!(operand is MemberAccessExpressionSyntax memberAccessExpression))
                            break;

                        if (memberAccessExpression.Kind() != SyntaxKind.SimpleMemberAccessExpression)
                            break;

                        if (!IsPropertyOfNullableOfT(memberAccessExpression.Name, "HasValue", semanticModel, cancellationToken))
                            break;

                        return new NullCheckExpressionInfo(expression, memberAccessExpression.Expression, NullCheckKind.NotHasValue);
                    }
            }

            return Default;
        }

        private static NullCheckExpressionInfo Create(
            BinaryExpressionSyntax binaryExpression,
            SyntaxKind binaryExpressionKind,
            ExpressionSyntax expression1,
            ExpressionSyntax expression2,
            NullCheckKind allowedKinds,
            bool allowMissing,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            switch (expression1.Kind())
            {
                case SyntaxKind.NullLiteralExpression:
                    {
                        NullCheckKind kind = (binaryExpressionKind == SyntaxKind.EqualsExpression) ? NullCheckKind.EqualsToNull : NullCheckKind.NotEqualsToNull;

                        if ((allowedKinds & kind) == 0)
                            break;

                        return new NullCheckExpressionInfo(
                            binaryExpression,
                            expression2,
                            kind);
                    }
                case SyntaxKind.TrueLiteralExpression:
                    {
                        NullCheckKind kind = (binaryExpressionKind == SyntaxKind.EqualsExpression) ? NullCheckKind.HasValue : NullCheckKind.NotHasValue;

                        return Create(
                            binaryExpression,
                            expression2,
                            kind,
                            allowedKinds,
                            allowMissing,
                            semanticModel,
                            cancellationToken);
                    }
                case SyntaxKind.FalseLiteralExpression:
                    {
                        NullCheckKind kind = (binaryExpressionKind == SyntaxKind.EqualsExpression) ? NullCheckKind.NotHasValue : NullCheckKind.HasValue;

                        return Create(
                            binaryExpression,
                            expression2,
                            kind,
                            allowedKinds,
                            allowMissing,
                            semanticModel,
                            cancellationToken);
                    }
            }

            return Default;
        }

        private static NullCheckExpressionInfo Create(
            BinaryExpressionSyntax binaryExpression,
            ExpressionSyntax expression,
            NullCheckKind kind,
            NullCheckKind allowedKinds,
            bool allowMissing,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if ((allowedKinds & (NullCheckKind.HasValueProperty)) == 0)
                return Default;

            if (!(expression is MemberAccessExpressionSyntax memberAccessExpression))
                return Default;

            if (memberAccessExpression.Kind() != SyntaxKind.SimpleMemberAccessExpression)
                return Default;

            if (!IsPropertyOfNullableOfT(memberAccessExpression.Name, "HasValue", semanticModel, cancellationToken))
                return Default;

            if ((allowedKinds & kind) == 0)
                return Default;

            ExpressionSyntax expression2 = memberAccessExpression.Expression;

            if (!Check(expression2, allowMissing))
                return Default;

            return new NullCheckExpressionInfo(binaryExpression, expression2, kind);
        }

        private static bool IsPropertyOfNullableOfT(
            ExpressionSyntax expression,
            string name,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return expression?.IsKind(SyntaxKind.IdentifierName) == true
                && string.Equals(((IdentifierNameSyntax)expression).Identifier.ValueText, name, StringComparison.Ordinal)
                && SyntaxUtility.IsPropertyOfNullableOfT(expression, name, semanticModel, cancellationToken);
        }

        public override string ToString()
        {
            return ContainingExpression?.ToString() ?? base.ToString();
        }
    }
}
