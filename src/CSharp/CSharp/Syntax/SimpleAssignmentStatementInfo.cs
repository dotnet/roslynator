// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.Syntax.SyntaxInfoHelpers;

namespace Roslynator.CSharp.Syntax
{
    /// <summary>
    /// Provides information about a simple assignment expression in an expression statement.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct SimpleAssignmentStatementInfo
    {
        private readonly SimpleAssignmentExpressionInfo _info;

        private SimpleAssignmentStatementInfo(in SimpleAssignmentExpressionInfo info)
        {
            _info = info;
        }

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

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return SyntaxInfoHelpers.ToDebugString(Success, this, Statement); }
        }

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

            return default;
        }

        internal static SimpleAssignmentStatementInfo Create(
            AssignmentExpressionSyntax assignmentExpression,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            if (assignmentExpression?.Parent?.Kind() == SyntaxKind.ExpressionStatement)
                return CreateImpl(assignmentExpression, walkDownParentheses, allowMissing);

            return default;
        }

        private static SimpleAssignmentStatementInfo CreateImpl(
            AssignmentExpressionSyntax assignmentExpression,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            SimpleAssignmentExpressionInfo info = SimpleAssignmentExpressionInfo.Create(assignmentExpression, walkDownParentheses, allowMissing);

            return new SimpleAssignmentStatementInfo(info);
        }
    }
}
