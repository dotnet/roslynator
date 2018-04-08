// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.Syntax.SyntaxInfoHelpers;

namespace Roslynator.CSharp.Syntax
{
    internal readonly struct BinaryExpressionChainInfo : IEquatable<BinaryExpressionChainInfo>, IReadOnlyList<ExpressionSyntax>
    {
        private BinaryExpressionChainInfo(
            BinaryExpressionSyntax binaryExpression,
            ImmutableArray<ExpressionSyntax> expressions)
        {
            BinaryExpression = binaryExpression;
            Expressions = expressions;
        }

        private static BinaryExpressionChainInfo Default { get; } = new BinaryExpressionChainInfo();

        public BinaryExpressionSyntax BinaryExpression { get; }

        public SyntaxKind Kind
        {
            get { return BinaryExpression?.Kind() ?? SyntaxKind.None; }
        }

        public ImmutableArray<ExpressionSyntax> Expressions { get; }

        public bool Success
        {
            get { return BinaryExpression != null; }
        }

        public int Count
        {
            get { return Expressions.Length; }
        }

        public ExpressionSyntax this[int index]
        {
            get { return Expressions[index]; }
        }

        public ImmutableArray<ExpressionSyntax>.Enumerator GetEnumerator()
        {
            return Expressions.GetEnumerator();
        }

        IEnumerator<ExpressionSyntax> IEnumerable<ExpressionSyntax>.GetEnumerator()
        {
            return ((IEnumerable<ExpressionSyntax>)Expressions).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Expressions).GetEnumerator();
        }

        internal static BinaryExpressionChainInfo Create(
            SyntaxNode node,
            bool walkDownParentheses = true)
        {
            return Create(Walk(node, walkDownParentheses) as BinaryExpressionSyntax);
        }

        internal static BinaryExpressionChainInfo Create(BinaryExpressionSyntax binaryExpression)
        {
            if (binaryExpression == null)
                return Default;

            SyntaxKind kind = binaryExpression.Kind();

            ImmutableArray<ExpressionSyntax> expressions = GetExpressions(binaryExpression, kind);

            if (expressions.IsDefault)
                return Default;

            return new BinaryExpressionChainInfo(binaryExpression, expressions);
        }

        private static ImmutableArray<ExpressionSyntax> GetExpressions(
            BinaryExpressionSyntax binaryExpression,
            SyntaxKind kind)
        {
            ImmutableArray<ExpressionSyntax>.Builder builder = null;
            bool success = true;

            while (success)
            {
                success = false;

                if (binaryExpression.Kind() == kind)
                {
                    ExpressionSyntax right = binaryExpression.Right;

                    if (right?.IsMissing == false)
                    {
                        (builder ?? (builder = ImmutableArray.CreateBuilder<ExpressionSyntax>())).Add(right);

                        ExpressionSyntax left = binaryExpression.Left;

                        if (left?.IsMissing == false)
                        {
                            if (left.Kind() == kind)
                            {
                                binaryExpression = (BinaryExpressionSyntax)left;
                                success = true;
                            }
                            else
                            {
                                builder.Add(left);
                                builder.Reverse();
                                return builder.ToImmutable();
                            }
                        }
                    }
                }
            }

            return default(ImmutableArray<ExpressionSyntax>);
        }

        public override string ToString()
        {
            return BinaryExpression?.ToString();
        }

        public override bool Equals(object obj)
        {
            return obj is BinaryExpressionChainInfo other && Equals(other);
        }

        public bool Equals(BinaryExpressionChainInfo other)
        {
            return EqualityComparer<BinaryExpressionSyntax>.Default.Equals(BinaryExpression, other.BinaryExpression);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<BinaryExpressionSyntax>.Default.GetHashCode(BinaryExpression);
        }

        public static bool operator ==(BinaryExpressionChainInfo info1, BinaryExpressionChainInfo info2)
        {
            return info1.Equals(info2);
        }

        public static bool operator !=(BinaryExpressionChainInfo info1, BinaryExpressionChainInfo info2)
        {
            return !(info1 == info2);
        }
    }
}
