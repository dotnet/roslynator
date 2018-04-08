// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.Syntax.SyntaxInfoHelpers;

namespace Roslynator.CSharp.Syntax
{
    /// <summary>
    /// Provides information about "is" expression.
    /// </summary>
    public readonly struct IsExpressionInfo : IEquatable<IsExpressionInfo>
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

        private static IsExpressionInfo Default { get; } = new IsExpressionInfo();

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
                return Default;

            ExpressionSyntax expression = Walk(binaryExpression.Left, walkDownParentheses);

            if (!Check(expression, allowMissing))
                return Default;

            var type = binaryExpression.Right as TypeSyntax;

            if (!Check(type, allowMissing))
                return Default;

            return new IsExpressionInfo(binaryExpression, expression, type);
        }

        /// <summary>
        /// Returns the string representation of the underlying syntax, not including its leading and trailing trivia.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return IsExpression?.ToString() ?? "";
        }

        /// <summary>
        /// Determines whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance. </param>
        /// <returns>true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            return obj is IsExpressionInfo other && Equals(other);
        }

        /// <summary>
        /// Determines whether this instance is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(IsExpressionInfo other)
        {
            return EqualityComparer<BinaryExpressionSyntax>.Default.Equals(IsExpression, other.IsExpression);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return EqualityComparer<BinaryExpressionSyntax>.Default.GetHashCode(IsExpression);
        }

        public static bool operator ==(IsExpressionInfo info1, IsExpressionInfo info2)
        {
            return info1.Equals(info2);
        }

        public static bool operator !=(IsExpressionInfo info1, IsExpressionInfo info2)
        {
            return !(info1 == info2);
        }
    }
}