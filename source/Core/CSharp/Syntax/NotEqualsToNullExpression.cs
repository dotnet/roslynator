// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Syntax
{
    internal struct NotEqualsToNullExpression : IEquatable<NotEqualsToNullExpression>
    {
        private NotEqualsToNullExpression(ExpressionSyntax left, ExpressionSyntax right)
        {
            Left = left;
            Right = right;
        }

        public BinaryExpressionSyntax NotEqualsExpression
        {
            get { return (BinaryExpressionSyntax)Parent; }
        }

        public ExpressionSyntax Left { get; }
        public ExpressionSyntax Right { get; }

        private SyntaxNode Parent
        {
            get { return Left?.Parent; }
        }

        public static NotEqualsToNullExpression Create(BinaryExpressionSyntax notEqualsExpression)
        {
            if (notEqualsExpression == null)
                throw new ArgumentNullException(nameof(notEqualsExpression));

            if (!notEqualsExpression.IsKind(SyntaxKind.NotEqualsExpression))
                throw new ArgumentException("", nameof(notEqualsExpression));

            ExpressionSyntax left = notEqualsExpression.Left;

            ExpressionSyntax right = notEqualsExpression.Right;

            if (right == null
                || !right.IsKind(SyntaxKind.NullLiteralExpression))
            {
                throw new ArgumentException("", nameof(notEqualsExpression));
            }

            return new NotEqualsToNullExpression(left, right);
        }

        public static bool TryCreate(SyntaxNode notEqualsExpression, out NotEqualsToNullExpression result)
        {
            if (notEqualsExpression?.IsKind(SyntaxKind.NotEqualsExpression) == true)
                return TryCreateCore((BinaryExpressionSyntax)notEqualsExpression, out result);

            result = default(NotEqualsToNullExpression);
            return false;
        }

        public static bool TryCreate(BinaryExpressionSyntax notEqualsExpression, out NotEqualsToNullExpression result)
        {
            if (notEqualsExpression?.IsKind(SyntaxKind.NotEqualsExpression) == true)
                return TryCreateCore(notEqualsExpression, out result);

            result = default(NotEqualsToNullExpression);
            return false;
        }

        private static bool TryCreateCore(BinaryExpressionSyntax notEqualsExpression, out NotEqualsToNullExpression result)
        {
            ExpressionSyntax left = notEqualsExpression.Left;

            ExpressionSyntax right = notEqualsExpression.Right;

            if (right?.IsKind(SyntaxKind.NullLiteralExpression) == true)
            {
                result = new NotEqualsToNullExpression(left, right);
                return true;
            }

            result = default(NotEqualsToNullExpression);
            return false;
        }

        public bool Equals(NotEqualsToNullExpression other)
        {
            return Parent == other.Parent;
        }

        public override bool Equals(object obj)
        {
            return obj is NotEqualsToNullExpression
                && Equals((NotEqualsToNullExpression)obj);
        }

        public override int GetHashCode()
        {
            return Parent?.GetHashCode() ?? 0;
        }

        public static bool operator ==(NotEqualsToNullExpression left, NotEqualsToNullExpression right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(NotEqualsToNullExpression left, NotEqualsToNullExpression right)
        {
            return !left.Equals(right);
        }
    }
}
