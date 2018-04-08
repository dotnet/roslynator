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
    /// Provides information about "as" expression.
    /// </summary>
    public readonly struct AsExpressionInfo : IEquatable<AsExpressionInfo>
    {
        private AsExpressionInfo(
            BinaryExpressionSyntax asExpression,
            ExpressionSyntax expression,
            TypeSyntax type)
        {
            AsExpression = asExpression;
            Expression = expression;
            Type = type;
        }

        private static AsExpressionInfo Default { get; } = new AsExpressionInfo();

        /// <summary>
        /// The "as" expression.
        /// </summary>
        public BinaryExpressionSyntax AsExpression { get; }

        /// <summary>
        /// The expression that is being casted.
        /// </summary>
        public ExpressionSyntax Expression { get; }

        /// <summary>
        /// The type to which the expression is being cast.
        /// </summary>
        public TypeSyntax Type { get; }

        /// <summary>
        /// The "as" operator token.
        /// </summary>
        public SyntaxToken OperatorToken
        {
            get { return AsExpression?.OperatorToken ?? default(SyntaxToken); }
        }

        /// <summary>
        /// Determines whether this struct was initialized with an actual syntax.
        /// </summary>
        public bool Success
        {
            get { return Expression != null; }
        }

        internal static AsExpressionInfo Create(
            SyntaxNode node,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return CreateImpl(
                Walk(node, walkDownParentheses) as BinaryExpressionSyntax,
                walkDownParentheses,
                allowMissing);
        }

        internal static AsExpressionInfo Create(
            BinaryExpressionSyntax binaryExpression,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return CreateImpl(binaryExpression, walkDownParentheses, allowMissing);
        }

        private static AsExpressionInfo CreateImpl(
            BinaryExpressionSyntax binaryExpression,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            if (binaryExpression?.Kind() != SyntaxKind.AsExpression)
                return Default;

            ExpressionSyntax expression = Walk(binaryExpression.Left, walkDownParentheses);

            if (!Check(expression, allowMissing))
                return Default;

            var type = binaryExpression.Right as TypeSyntax;

            if (!Check(type, allowMissing))
                return Default;

            return new AsExpressionInfo(binaryExpression, expression, type);
        }

        /// <summary>
        /// Returns the string representation of the underlying syntax, not including its leading and trailing trivia.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return AsExpression?.ToString() ?? "";
        }

        /// <summary>
        /// Determines whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance. </param>
        /// <returns>true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            return obj is AsExpressionInfo other && Equals(other);
        }

        /// <summary>
        /// Determines whether this instance is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(AsExpressionInfo other)
        {
            return EqualityComparer<BinaryExpressionSyntax>.Default.Equals(AsExpression, other.AsExpression);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return EqualityComparer<BinaryExpressionSyntax>.Default.GetHashCode(AsExpression);
        }

        public static bool operator ==(AsExpressionInfo info1, AsExpressionInfo info2)
        {
            return info1.Equals(info2);
        }

        public static bool operator !=(AsExpressionInfo info1, AsExpressionInfo info2)
        {
            return !(info1 == info2);
        }
    }
}