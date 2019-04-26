// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.Syntax.SyntaxInfoHelpers;

namespace Roslynator.CSharp.Syntax
{
    /// <summary>
    /// Provides information about "is" expression.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct IsExpressionInfo
    {
        private IsExpressionInfo(
            BinaryExpressionSyntax isExpression,
            ExpressionSyntax expression,
            TypeSyntax type)
        {
            IsExpression = isExpression;
            Expression = expression;
            Type = type;
        }

        /// <summary>
        /// The "is" expression.
        /// </summary>
        public BinaryExpressionSyntax IsExpression { get; }

        /// <summary>
        /// The expression that is being casted.
        /// </summary>
        public ExpressionSyntax Expression { get; }

        /// <summary>
        /// The type to which the expression is being cast.
        /// </summary>
        public TypeSyntax Type { get; }

        /// <summary>
        /// The "is" operator token.
        /// </summary>
        public SyntaxToken OperatorToken
        {
            get { return IsExpression?.OperatorToken ?? default(SyntaxToken); }
        }

        /// <summary>
        /// Determines whether this struct was initialized with an actual syntax.
        /// </summary>
        public bool Success
        {
            get { return Expression != null; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return SyntaxInfoHelpers.ToDebugString(Success, this, Expression); }
        }

        internal static IsExpressionInfo Create(
            SyntaxNode node,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return CreateImpl(
                Walk(node, walkDownParentheses) as BinaryExpressionSyntax,
                walkDownParentheses,
                allowMissing);
        }

        internal static IsExpressionInfo Create(
            BinaryExpressionSyntax binaryExpression,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return CreateImpl(binaryExpression, walkDownParentheses, allowMissing);
        }

        private static IsExpressionInfo CreateImpl(
            BinaryExpressionSyntax binaryExpression,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            if (binaryExpression?.Kind() != SyntaxKind.IsExpression)
                return default;

            ExpressionSyntax expression = Walk(binaryExpression.Left, walkDownParentheses);

            if (!Check(expression, allowMissing))
                return default;

            var type = binaryExpression.Right as TypeSyntax;

            if (!Check(type, allowMissing))
                return default;

            return new IsExpressionInfo(binaryExpression, expression, type);
        }
    }
}