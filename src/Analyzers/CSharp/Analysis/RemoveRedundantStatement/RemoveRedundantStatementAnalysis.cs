// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analysis.RemoveRedundantStatement
{
    internal static class RemoveRedundantStatementAnalysis
    {
        public static RemoveRedundantContinueStatementAnalysis ContinueStatement { get; } = new RemoveRedundantContinueStatementAnalysis();

        public static RemoveRedundantReturnStatementAnalysis ReturnStatement { get; } = new RemoveRedundantReturnStatementAnalysis();

        public static RemoveRedundantYieldBreakStatementAnalysis YieldBreakStatement { get; } = new RemoveRedundantYieldBreakStatementAnalysis();
    }
}
