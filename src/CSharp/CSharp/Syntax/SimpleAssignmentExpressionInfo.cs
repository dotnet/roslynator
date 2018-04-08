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
    /// Provides information about simple assignment expression.
    /// </summary>
    public readonly struct SimpleAssignmentExpressionInfo : IEquatable<SimpleAssignmentExpressionInfo>
    {
        private SimpleAssignmentExpressionInfo(
            AssignmentExpressionSyntax assignmentExpression,
            ExpressionSyntax left,
            ExpressionSyntax right)
        {
            AssignmentExpression = assignmentExpression;
            Left = left;
            Right = right;
        }

        private static SimpleAssignmentExpressionInfo Default { get; } = new SimpleAssignmentExpressionInfo();

        /// <summary>
        /// The simple assignment expression.
        /// </summary>
        public AssignmentExpressionSyntax AssignmentExpression { get; }

        /// <summary>
        /// The expression on the left of the assignment operator.
        /// </summary>
        public ExpressionSyntax Left { get; }

        /// <summary>
        /// The expression on the right of the assignment operator.
        /// </summary>
        public ExpressionSyntax Right { get; }

        /// <summary>
        /// The operator of the simple assignment expression.
        /// </summary>
        public SyntaxToken OperatorToken
        {
            get { return AssignmentExpression?.OperatorToken ?? default(SyntaxToken); }
        }

        /// <summary>
        /// Determines whether this struct was initialized with an actual syntax.
        /// </summary>
        public bool Success
        {
            get { return AssignmentExpression != null; }
        }

        internal static SimpleAssignmentExpressionInfo Create(
            SyntaxNode node,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return Create(WalkAndCheck(node, walkDownParentheses, allowMissing) as AssignmentExpressionSyntax, walkDownParentheses, allowMissing);
        }

        internal static SimpleAssignmentExpressionInfo Create(
            AssignmentExpressionSyntax assignmentExpression,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            if (assignmentExpression?.Kind() != SyntaxKind.SimpleAssignmentExpression)
                return Default;

            ExpressionSyntax left = WalkAndCheck(assignmentExpression.Left, walkDownParentheses, allowMissing);

            if (left == null)
                return Default;

            ExpressionSyntax right = WalkAndCheck(assignmentExpression.Right, walkDownParentheses, allowMissing);

            if (right == null)
                return Default;

            return new SimpleAssignmentExpressionInfo(assignmentExpression, left, right);
        }

        /// <summary>
        /// Returns the string representation of the underlying syntax, not including its leading and trailing trivia.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return AssignmentExpression?.ToString() ?? "";
        }

        /// <summary>
        /// Determines whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance. </param>
        /// <returns>true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            return obj is SimpleAssignmentExpressionInfo other && Equals(other);
        }

        /// <summary>
        /// Determines whether this instance is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(SimpleAssignmentExpressionInfo other)
        {
            return EqualityComparer<AssignmentExpressionSyntax>.Default.Equals(AssignmentExpression, other.AssignmentExpression);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return EqualityComparer<AssignmentExpressionSyntax>.Default.GetHashCode(AssignmentExpression);
        }

        public static bool operator ==(SimpleAssignmentExpressionInfo info1, SimpleAssignmentExpressionInfo info2)
        {
            return info1.Equals(info2);
        }

        public static bool operator !=(SimpleAssignmentExpressionInfo info1, SimpleAssignmentExpressionInfo info2)
        {
            return !(info1 == info2);
        }
    }
}
