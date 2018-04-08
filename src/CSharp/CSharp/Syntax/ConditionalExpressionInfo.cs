// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.Syntax.SyntaxInfoHelpers;

namespace Roslynator.CSharp.Syntax
{
    /// <summary>
    /// Provides information about conditional expression.
    /// </summary>
    public readonly struct ConditionalExpressionInfo : IEquatable<ConditionalExpressionInfo>
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

        private static ConditionalExpressionInfo Default { get; } = new ConditionalExpressionInfo();

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
                return Default;

            ExpressionSyntax condition = WalkAndCheck(conditionalExpression.Condition, walkDownParentheses, allowMissing);

            if (condition == null)
                return Default;

            ExpressionSyntax whenTrue = WalkAndCheck(conditionalExpression.WhenTrue, walkDownParentheses, allowMissing);

            if (whenTrue == null)
                return Default;

            ExpressionSyntax whenFalse = WalkAndCheck(conditionalExpression.WhenFalse, walkDownParentheses, allowMissing);

            if (whenFalse == null)
                return Default;

            return new ConditionalExpressionInfo(condition, whenTrue, whenFalse);
        }

        /// <summary>
        /// Returns the string representation of the underlying syntax, not including its leading and trailing trivia.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ConditionalExpression?.ToString() ?? "";
        }

        /// <summary>
        /// Determines whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance. </param>
        /// <returns>true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            return obj is ConditionalExpressionInfo other && Equals(other);
        }

        /// <summary>
        /// Determines whether this instance is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(ConditionalExpressionInfo other)
        {
            return EqualityComparer<ConditionalExpressionSyntax>.Default.Equals(ConditionalExpression, other.ConditionalExpression);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return EqualityComparer<ConditionalExpressionSyntax>.Default.GetHashCode(ConditionalExpression);
        }

        public static bool operator ==(ConditionalExpressionInfo info1, ConditionalExpressionInfo info2)
        {
            return info1.Equals(info2);
        }

        public static bool operator !=(ConditionalExpressionInfo info1, ConditionalExpressionInfo info2)
        {
            return !(info1 == info2);
        }
    }
}