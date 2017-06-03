// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
            get { return (BinaryExpressionSyntax)Node; }
        }

        public ExpressionSyntax Left { get; }
        public ExpressionSyntax Right { get; }

        private SyntaxNode Node
        {
            get { return Left?.Parent; }
        }

        public static EqualsToNullExpression Create(BinaryExpressionSyntax equalsExpression)
        {
            if (equalsExpression == null)
                throw new ArgumentNullException(nameof(equalsExpression));

            if (!equalsExpression.IsKind(SyntaxKind.EqualsExpression))
                throw new ArgumentException("", nameof(equalsExpression));

            ExpressionSyntax left = equalsExpression.Left;

            ExpressionSyntax right = equalsExpression.Right;

            if (right == null
                || !right.IsKind(SyntaxKind.NullLiteralExpression))
            {
                throw new ArgumentException("", nameof(equalsExpression));
            }

            return new EqualsToNullExpression(left, right);
        }

        public static bool TryCreate(SyntaxNode equalsExpression, out EqualsToNullExpression result)
        {
            if (equalsExpression?.IsKind(SyntaxKind.EqualsExpression) == true)
                return TryCreateCore((BinaryExpressionSyntax)equalsExpression, out result);

            result = default(EqualsToNullExpression);
            return false;
        }

        public static bool TryCreate(BinaryExpressionSyntax equalsExpression, out EqualsToNullExpression result)
        {
            if (equalsExpression?.IsKind(SyntaxKind.EqualsExpression) == true)
                return TryCreateCore(equalsExpression, out result);

            result = default(EqualsToNullExpression);
            return false;
        }

        private static bool TryCreateCore(BinaryExpressionSyntax equalsExpression, out EqualsToNullExpression result)
        {
            ExpressionSyntax left = equalsExpression.Left;
            ExpressionSyntax right = equalsExpression.Right;

            if (right?.IsKind(SyntaxKind.NullLiteralExpression) == true)
            {
                result = new EqualsToNullExpression(left, right);
                return true;
            }

            result = default(EqualsToNullExpression);
            return false;
        }

        public override string ToString()
        {
            return Node?.ToString() ?? base.ToString();
        }

        public bool Equals(EqualsToNullExpression other)
        {
            return Node == other.Node;
        }

        public override bool Equals(object obj)
        {
            return obj is EqualsToNullExpression
                && Equals((EqualsToNullExpression)obj);
        }

        public override int GetHashCode()
        {
            return Node?.GetHashCode() ?? 0;
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
