// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.Syntax.SyntaxInfoHelpers;

namespace Roslynator.CSharp.Syntax
{
    /// <summary>
    /// Provides information about conditional expression.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct ConditionalExpressionInfo
    {
        private ConditionalExpressionInfo(
            ExpressionSyntax condition,
            ExpressionSyntax whenTrue,
            ExpressionSyntax whenFalse)
        {
            Condition = condition;
            WhenTrue = whenTrue;
            WhenFalse = whenFalse;
        }

        /// <summary>
        /// The conditional expression.
        /// </summary>
        public ConditionalExpressionSyntax ConditionalExpression
        {
            get { return (ConditionalExpressionSyntax)Condition?.WalkUpParentheses().Parent; }
        }

        /// <summary>
        /// The condition expression.
        /// </summary>
        public ExpressionSyntax Condition { get; }

        /// <summary>
        /// The expression to be executed when the expression is true.
        /// </summary>
        public ExpressionSyntax WhenTrue { get; }

        /// <summary>
        /// The expression to be executed when the expression is false.
        /// </summary>
        public ExpressionSyntax WhenFalse { get; }

        /// <summary>
        /// The token representing the question mark.
        /// </summary>
        public SyntaxToken QuestionToken
        {
            get { return ConditionalExpression?.QuestionToken ?? default(SyntaxToken); }
        }

        /// <summary>
        /// The token representing the colon.
        /// </summary>
        public SyntaxToken ColonToken
        {
            get { return ConditionalExpression?.ColonToken ?? default(SyntaxToken); }
        }

        /// <summary>
        /// Determines whether this struct was initialized with an actual syntax.
        /// </summary>
        public bool Success
        {
            get { return Condition != null; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return ToDebugString(Success, this, ConditionalExpression); }
        }

        internal static ConditionalExpressionInfo Create(
            SyntaxNode node,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return CreateImpl(
                Walk(node, walkDownParentheses) as ConditionalExpressionSyntax,
                walkDownParentheses,
                allowMissing);
        }

        internal static ConditionalExpressionInfo Create(
            ConditionalExpressionSyntax conditionalExpression,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return CreateImpl(conditionalExpression, walkDownParentheses, allowMissing);
        }

        private static ConditionalExpressionInfo CreateImpl(
            ConditionalExpressionSyntax conditionalExpression,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            if (conditionalExpression == null)
                return default;

            ExpressionSyntax condition = WalkAndCheck(conditionalExpression.Condition, walkDownParentheses, allowMissing);

            if (condition == null)
                return default;

            ExpressionSyntax whenTrue = WalkAndCheck(conditionalExpression.WhenTrue, walkDownParentheses, allowMissing);

            if (whenTrue == null)
                return default;

            ExpressionSyntax whenFalse = WalkAndCheck(conditionalExpression.WhenFalse, walkDownParentheses, allowMissing);

            if (whenFalse == null)
                return default;

            return new ConditionalExpressionInfo(condition, whenTrue, whenFalse);
        }
    }
}