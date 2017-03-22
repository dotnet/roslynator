// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;

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

        public static bool TryCreate(ExpressionSyntax equalsExpression, out NotEqualsToNullExpression notEqualsToNull)
        {
            if (equalsExpression?.IsKind(SyntaxKind.NotEqualsExpression) == true)
                return TryCreateCore((BinaryExpressionSyntax)equalsExpression, out notEqualsToNull);

            notEqualsToNull = default(NotEqualsToNullExpression);
            return false;
        }

        public static bool TryCreate(BinaryExpressionSyntax equalsExpression, out NotEqualsToNullExpression notEqualsToNull)
        {
            if (equalsExpression?.IsKind(SyntaxKind.NotEqualsExpression) == true)
                return TryCreateCore(equalsExpression, out notEqualsToNull);

            notEqualsToNull = default(NotEqualsToNullExpression);
            return false;
        }

        private static bool TryCreateCore(BinaryExpressionSyntax equalsExpression, out NotEqualsToNullExpression notEqualsToNull)
        {
            ExpressionSyntax left = equalsExpression.Left;

            if (left?.IsMissing == false)
            {
                ExpressionSyntax right = equalsExpression.Right;

                if (right?.IsMissing == false
                    && right.IsKind(SyntaxKind.NullLiteralExpression))
                {
                    notEqualsToNull = new NotEqualsToNullExpression(left, right);
                    return true;
                }
            }

            notEqualsToNull = default(NotEqualsToNullExpression);
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
