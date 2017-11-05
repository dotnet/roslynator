// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.Syntax.SyntaxInfoHelpers;

namespace Roslynator.CSharp.Syntax
{
    internal struct ConditionalReturnStatementInfo
    {
        private static ConditionalReturnStatementInfo Default { get; } = new ConditionalReturnStatementInfo();

        public ConditionalReturnStatementInfo(
            ExpressionSyntax condition,
            ExpressionSyntax whenTrue,
            ExpressionSyntax whenFalse)
        {
            Condition = condition;
            WhenTrue = whenTrue;
            WhenFalse = whenFalse;
        }

        public IfStatementSyntax IfStatement
        {
            get { return (IfStatementSyntax)Condition?.WalkUpParentheses().Parent; }
        }

        public ExpressionSyntax Condition { get; }

        public ExpressionSyntax WhenTrue { get; }

        public ReturnStatementSyntax WhenTrueStatement
        {
            get { return (ReturnStatementSyntax)WhenTrue?.WalkUpParentheses().Parent; }
        }

        public ExpressionSyntax WhenFalse { get; }

        public ReturnStatementSyntax WhenFalseStatement
        {
            get { return (ReturnStatementSyntax)WhenFalse?.WalkUpParentheses().Parent; }
        }

        public bool Success
        {
            get { return Condition != null; }
        }

        internal static ConditionalReturnStatementInfo Create(
            IfStatementSyntax ifStatement,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            if (ifStatement?.IsParentKind(SyntaxKind.ElseClause) != false)
                return Default;

            if (!(ifStatement.Statement?.SingleNonBlockStatementOrDefault() is ReturnStatementSyntax whenTrueReturnStatement))
                return Default;

            ExpressionSyntax whenTrue = WalkAndCheck(whenTrueReturnStatement.Expression, allowMissing, walkDownParentheses);

            if (whenTrue == null)
                return Default;

            ElseClauseSyntax elseClause = ifStatement.Else;

            StatementSyntax statement = (elseClause != null)
                ? elseClause.Statement.SingleNonBlockStatementOrDefault()
                : ifStatement.NextStatementOrDefault();

            if (!(statement is ReturnStatementSyntax whenFalseReturnStatement))
                return Default;

            ExpressionSyntax whenFalse = WalkAndCheck(whenFalseReturnStatement.Expression, allowMissing, walkDownParentheses);

            if (whenFalse == null)
                return Default;

            ExpressionSyntax condition = WalkAndCheck(ifStatement.Condition, allowMissing, walkDownParentheses);

            if (condition == null)
                return Default;

            return new ConditionalReturnStatementInfo(condition, whenTrue, whenFalse);
        }

        public override string ToString()
        {
            return IfStatement?.ToString() ?? base.ToString();
        }
    }
}
