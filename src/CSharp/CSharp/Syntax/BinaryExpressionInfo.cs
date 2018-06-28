// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.Syntax.SyntaxInfoHelpers;

namespace Roslynator.CSharp.Syntax
{
    /// <summary>
    /// Provides information about binary expression.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct BinaryExpressionInfo : IEquatable<BinaryExpressionInfo>
    {
        internal BinaryExpressionInfo(
            BinaryExpressionSyntax binaryExpression,
            ExpressionSyntax left,
            ExpressionSyntax right)
        {
            BinaryExpression = binaryExpression;
            Left = left;
            Right = right;
        }

        /// <summary>
        /// The binary expression.
        /// </summary>
        public BinaryExpressionSyntax BinaryExpression { get; }

        /// <summary>
        /// The expression on the left of the binary operator.
        /// </summary>
        public ExpressionSyntax Left { get; }

        /// <summary>
        /// The expression on the right of the binary operator.
        /// </summary>
        public ExpressionSyntax Right { get; }

        /// <summary>
        /// The kind of the binary expression.
        /// </summary>
        public SyntaxKind Kind
        {
            get { return BinaryExpression?.Kind() ?? SyntaxKind.None; }
        }

        /// <summary>
        /// Determines whether this struct was initialized with an actual syntax.
        /// </summary>
        public bool Success
        {
            get { return BinaryExpression != null; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return ToDebugString(Success, this, BinaryExpression); }
        }

        /// <summary>
        /// Returns expressions of this binary expression, including expressions of nested binary expressions of the same kind as parent binary expression.
        /// </summary>
        /// <param name="leftToRight">If true expressions are enumerated as they are displayed in the source code.</param>
        /// <returns></returns>
        [Obsolete("This method is obsolete. Use method 'AsChain' instead.")]
        public IEnumerable<ExpressionSyntax> Expressions(bool leftToRight = false)
        {
            ThrowInvalidOperationIfNotInitialized();

            if (leftToRight)
            {
                return AsChain();
            }
            else
            {
                return AsChain().Reverse();
            }
        }

        public ExpressionChain AsChain()
        {
            return new ExpressionChain(BinaryExpression);
        }

        internal static BinaryExpressionInfo Create(
            SyntaxNode node,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return CreateImpl(
                Walk(node, walkDownParentheses) as BinaryExpressionSyntax,
                walkDownParentheses,
                allowMissing);
        }

        internal static BinaryExpressionInfo Create(
            BinaryExpressionSyntax binaryExpression,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return CreateImpl(binaryExpression, walkDownParentheses, allowMissing);
        }

        private static BinaryExpressionInfo CreateImpl(
            BinaryExpressionSyntax binaryExpression,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            if (binaryExpression == null)
                return default;

            ExpressionSyntax left = Walk(binaryExpression.Left, walkDownParentheses);

            if (!Check(left, allowMissing))
                return default;

            ExpressionSyntax right = Walk(binaryExpression.Right, walkDownParentheses);

            if (!Check(right, allowMissing))
                return default;

            return new BinaryExpressionInfo(binaryExpression, left, right);
        }

        private void ThrowInvalidOperationIfNotInitialized()
        {
            if (BinaryExpression == null)
                throw new InvalidOperationException($"{nameof(BinaryExpressionInfo)} is not initalized.");
        }

        /// <summary>
        /// Returns the string representation of the underlying syntax, not including its leading and trailing trivia.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return BinaryExpression?.ToString() ?? "";
        }

        /// <summary>
        /// Determines whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance. </param>
        /// <returns>true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            return obj is BinaryExpressionInfo other && Equals(other);
        }

        /// <summary>
        /// Determines whether this instance is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(BinaryExpressionInfo other)
        {
            return EqualityComparer<BinaryExpressionSyntax>.Default.Equals(BinaryExpression, other.BinaryExpression);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return EqualityComparer<BinaryExpressionSyntax>.Default.GetHashCode(BinaryExpression);
        }

        public static bool operator ==(in BinaryExpressionInfo info1, in BinaryExpressionInfo info2)
        {
            return info1.Equals(info2);
        }

        public static bool operator !=(in BinaryExpressionInfo info1, in BinaryExpressionInfo info2)
        {
            return !(info1 == info2);
        }
    }
}