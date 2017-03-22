// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Syntax
{
    internal struct EqualsToNullExpression : IEquatable<EqualsToNullExpression>
    {
        private EqualsToNullExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            Left = left;
            Right = right;
        }

        public BinaryExpressionSyntax EqualsExpression
        {
            get { return (BinaryExpressionSyntax)Parent; }
        }

        public ExpressionSyntax Left { get; }
        public ExpressionSyntax Right { get; }

        private SyntaxNode Parent
        {
            get { return Left?.Parent; }
        }

        public static bool TryCreate(ExpressionSyntax equalsExpression, out EqualsToNullExpression equalsToNull)
        {
            if (equalsExpression?.IsKind(SyntaxKind.EqualsExpression) == true)
                return TryCreateCore((BinaryExpressionSyntax)equalsExpression, out equalsToNull);

            equalsToNull = default(EqualsToNullExpression);
            return false;
        }

        public static bool TryCreate(BinaryExpressionSyntax equalsExpression, out EqualsToNullExpression equalsToNull)
        {
            if (equalsExpression?.IsKind(SyntaxKind.EqualsExpression) == true)
                return TryCreateCore(equalsExpression, out equalsToNull);

            equalsToNull = default(EqualsToNullExpression);
            return false;
        }

        private static bool TryCreateCore(BinaryExpressionSyntax equalsExpression, out EqualsToNullExpression equalsToNull)
        {
            ExpressionSyntax left = equalsExpression.Left;

            if (left?.IsMissing == false)
            {
                ExpressionSyntax right = equalsExpression.Right;

                if (right?.IsMissing == false
                    && right.IsKind(SyntaxKind.NullLiteralExpression))
                {
                    equalsToNull = new EqualsToNullExpression(left, right);
                    return true;
                }
            }

            equalsToNull = default(EqualsToNullExpression);
            return false;
        }

        public bool Equals(EqualsToNullExpression other)
        {
            return Parent == other.Parent;
        }

        public override bool Equals(object obj)
        {
            return obj is EqualsToNullExpression
                && Equals((EqualsToNullExpression)obj);
        }

        public override int GetHashCode()
        {
            return Parent?.GetHashCode() ?? 0;
        }

        public static bool operator ==(EqualsToNullExpression left, EqualsToNullExpression right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EqualsToNullExpression left, EqualsToNullExpression right)
        {
            return !left.Equals(right);
        }
    }
}
