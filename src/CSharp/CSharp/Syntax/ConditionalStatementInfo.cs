// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.Syntax.SyntaxInfoHelpers;

namespace Roslynator.CSharp.Syntax
{
    /// <summary>
    /// Provides information about a simple if-else where if and else contains single non-block statement.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal readonly struct ConditionalStatementInfo
    {
        private ConditionalStatementInfo(
            IfStatementSyntax ifStatement,
            ExpressionSyntax condition,
            StatementSyntax whenTrue,
            StatementSyntax whenFalse)
        {
            IfStatement = ifStatement;
            Condition = condition;
            WhenTrue = whenTrue;
            WhenFalse = whenFalse;
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
        /// The statement that is executed if the condition evaluates to true.
        /// </summary>
        public StatementSyntax WhenTrue { get; }

        /// <summary>
        /// The statement that is executed if the condition evaluates to false.
        /// </summary>
        public StatementSyntax WhenFalse { get; }

        /// <summary>
        /// The else clause.
        /// </summary>
        public ElseClauseSyntax Else
        {
            get { return IfStatement?.Else; }
        }

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
            get { return ToDebugString(Success, this, IfStatement); }
        }

        internal static ConditionalStatementInfo Create(
            IfStatementSyntax ifStatement,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            if (ifStatement?.IsParentKind(SyntaxKind.ElseClause) != false)
                return default;

            StatementSyntax whenTrue = ifStatement.Statement.SingleNonBlockStatementOrDefault();

            if (!Check(whenTrue, allowMissing))
                return default;

            StatementSyntax whenFalse = ifStatement.Else?.Statement.SingleNonBlockStatementOrDefault();

            if (!Check(whenFalse, allowMissing))
                return default;

            if (whenFalse.IsKind(SyntaxKind.IfStatement))
                return default;

            ExpressionSyntax condition = WalkAndCheck(ifStatement.Condition, walkDownParentheses, allowMissing);

            if (condition == null)
                return default;

            return new ConditionalStatementInfo(ifStatement, condition, whenTrue, whenFalse);
        }
    }
}
