// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.Syntax.SyntaxInfoHelpers;

namespace Roslynator.CSharp.Syntax
{
    /// <summary>
    /// Provides information about a simple if statement.
    /// Simple if statement is defined as follows: it is not a child of an else clause and it has no else clause.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct SimpleIfStatementInfo
    {
        private SimpleIfStatementInfo(
            IfStatementSyntax ifStatement,
            ExpressionSyntax condition,
            StatementSyntax statement)
        {
            IfStatement = ifStatement;
            Condition = condition;
            Statement = statement;
        }

        /// <summary>
        /// The if statement.
        /// </summary>
        public IfStatementSyntax IfStatement { get; }

        /// <summary>
        /// The condition.
        /// </summary>
        public ExpressionSyntax Condition { get; }

        /// <summary>
        /// The statement.
        /// </summary>
        public StatementSyntax Statement { get; }

        /// <summary>
        /// Determines whether this struct was initialized with an actual syntax.
        /// </summary>
        public bool Success
        {
            get { return IfStatement != null; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get { return SyntaxInfoHelpers.ToDebugString(Success, this, IfStatement); }
        }

        internal static SimpleIfStatementInfo Create(
            SyntaxNode node,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            return Create(node as IfStatementSyntax, walkDownParentheses, allowMissing);
        }

        internal static SimpleIfStatementInfo Create(
            IfStatementSyntax ifStatement,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            if (ifStatement?.IsSimpleIf() != true)
                return default;

            ExpressionSyntax condition = WalkAndCheck(ifStatement.Condition, walkDownParentheses, allowMissing);

            if (condition == null)
                return default;

            StatementSyntax statement = ifStatement.Statement;

            if (!Check(statement, allowMissing))
                return default;

            return new SimpleIfStatementInfo(ifStatement, condition, statement);
        }
    }
}
