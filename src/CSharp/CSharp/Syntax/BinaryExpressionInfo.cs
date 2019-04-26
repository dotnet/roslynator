// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
    public readonly struct BinaryExpressionInfo
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
        /// The operator of the binary expression.
        /// </summary>
        public SyntaxToken OperatorToken
        {
            get { return BinaryExpression?.OperatorToken ?? default; }
        }

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
        /// Returns <see cref="ExpressionChain"/> that enables to enumerate expressions of a binary expression.
        /// </summary>
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
    }
}