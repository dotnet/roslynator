// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.Syntax.SyntaxInfoHelpers;

namespace Roslynator.CSharp.Syntax
{
    public struct SimpleIfElseInfo
    {
        private static SimpleIfElseInfo Default { get; } = new SimpleIfElseInfo();

        private SimpleIfElseInfo(
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

        public IfStatementSyntax IfStatement { get; }

        public ExpressionSyntax Condition { get; }

        public StatementSyntax WhenTrue { get; }

        public StatementSyntax WhenFalse { get; }

        internal static SimpleIfElseInfo Create(
            IfStatementSyntax ifStatement,
            bool walkDownParentheses = true,
            bool allowMissing = false)
        {
            if (ifStatement?.IsParentKind(SyntaxKind.ElseClause) != false)
                return Default;

            StatementSyntax whenFalse = ifStatement.Else?.Statement;

            if (!Check(whenFalse, allowMissing))
                return Default;

            if (whenFalse.IsKind(SyntaxKind.IfStatement))
                return Default;

            StatementSyntax whenTrue = ifStatement.Statement;

            if (!Check(whenTrue, allowMissing))
                return Default;

            ExpressionSyntax condition = WalkAndCheck(ifStatement.Condition, allowMissing, walkDownParentheses);

            if (condition == null)
                return Default;

            return new SimpleIfElseInfo(ifStatement, condition, whenTrue, whenFalse);
        }

        public override string ToString()
        {
            return IfStatement?.ToString() ?? base.ToString();
        }
    }
}
