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
    /// Provides information about a simple assignment expression in an expression statement.
    /// </summary>
    public readonly struct SimpleAssignmentStatementInfo : IEquatable<SimpleAssignmentStatementInfo>
    {
        private readonly SimpleAssignmentExpressionInfo _info;

        private SimpleAssignmentStatementInfo(SimpleAssignmentExpressionInfo info)
        {
            _info = info;
        }

        private static SimpleAssignmentStatementInfo Default { get; } = new SimpleAssignmentStatementInfo();

        /// <summary>
        /// The simple assignment expression.
        /// </summary>
        public AssignmentExpressionSyntax AssignmentExpression => _info.AssignmentExpression;

        /// <summary>
        /// The expression on the left of the assignment operator.
        /// </summary>
        public ExpressionSyntax Left => _info.Left;

        /// <summary>
        /// The expression of the right of the assignment operator.
        /// </summary>
        public ExpressionSyntax Right => _info.Right;

        /// <summary>
        /// The operator of the simple assignment expression.
        /// </summary>
        public SyntaxToken OperatorToken => _info.OperatorToken;

        /// <summary>
        /// The expression statement the simple assignment expression is contained in.
        /// </summary>
        public ExpressionStatementSyntax Statement
        {
            get { return (ExpressionStatementSyntax)AssignmentExpression?.Parent; }
        }

        /// <summary>
        /// Determines whether this struct was initialized with an actual syntax.
        /// </summary>
        public bool Success => _info.Success;

        internal static SimpleAssignmentStatementInfo Create(
            StatementSyntax statement,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return Create(statement as ExpressionStatementSyntax, walkDownParentheses, allowMissing);
        }

        internal static SimpleAssignmentStatementInfo Create(
            ExpressionStatementSyntax expressionStatement,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            ExpressionSyntax expression = expressionStatement?.Expression;

            if (Check(expression, allowMissing))
                return CreateImpl(expression as AssignmentExpressionSyntax, walkDownParentheses, allowMissing);

            return Default;
        }

        internal static SimpleAssignmentStatementInfo Create(
            AssignmentExpressionSyntax assignmentExpression,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            if (assignmentExpression?.Parent?.Kind() == SyntaxKind.ExpressionStatement)
                return CreateImpl(assignmentExpression, walkDownParentheses, allowMissing);

            return Default;
        }

        private static SimpleAssignmentStatementInfo CreateImpl(
            AssignmentExpressionSyntax assignmentExpression,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            SimpleAssignmentExpressionInfo info = SimpleAssignmentExpressionInfo.Create(assignmentExpression, walkDownParentheses, allowMissing);

            return new SimpleAssignmentStatementInfo(info);
        }

        /// <summary>
        /// Returns the string representation of the underlying syntax, not including its leading and trailing trivia.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return AssignmentExpression?.Parent.ToString() ?? "";
        }

        /// <summary>
        /// Determines whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance. </param>
        /// <returns>true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false. </returns>
        public override bool Equals(object obj)
        {
            return obj is SimpleAssignmentStatementInfo other && Equals(other);
        }

        /// <summary>
        /// Determines whether this instance is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(SimpleAssignmentStatementInfo other)
        {
            return EqualityComparer<SyntaxNode>.Default.Equals(AssignmentExpression?.Parent, other.AssignmentExpression?.Parent);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return EqualityComparer<SyntaxNode>.Default.GetHashCode(AssignmentExpression?.Parent);
        }

        public static bool operator ==(SimpleAssignmentStatementInfo info1, SimpleAssignmentStatementInfo info2)
        {
            return info1.Equals(info2);
        }

        public static bool operator !=(SimpleAssignmentStatementInfo info1, SimpleAssignmentStatementInfo info2)
        {
            return !(info1 == info2);
        }
    }
}
