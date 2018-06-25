// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.RemoveRedundantStatement
{
    internal static class RemoveRedundantStatementAnalysis
    {
        public static bool IsFixable(StatementSyntax statement, SyntaxKind statementKind)
        {
            if (!(statement.Parent is BlockSyntax block))
                return false;

            if (!block.Statements.IsLast(statement, ignoreLocalFunctions: true))
                return false;

            switch (statementKind)
            {
                case SyntaxKind.ContinueStatement:
                    return RemoveRedundantContinueStatementAnalysis.Instance.IsFixable(statement, block);
                case SyntaxKind.ReturnStatement:
                    return RemoveRedundantReturnStatementAnalysis.Instance.IsFixable(statement, block);
                case SyntaxKind.YieldBreakStatement:
                    return RemoveRedundantYieldBreakStatementAnalysis.Instance.IsFixable(statement, block);
                default:
                    return false;
            }
        }

        public static bool IsFixable(ContinueStatementSyntax continueStatement)
        {
            return RemoveRedundantContinueStatementAnalysis.Instance.IsFixable(continueStatement);
        }

        public static bool IsFixable(ReturnStatementSyntax returnStatement)
        {
            return RemoveRedundantReturnStatementAnalysis.Instance.IsFixable(returnStatement);
        }

        public static bool IsFixable(YieldStatementSyntax yieldBreakStatement)
        {
            return RemoveRedundantYieldBreakStatementAnalysis.Instance.IsFixable(yieldBreakStatement);
        }
    }
}
