// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.RemoveRedundantStatement
{
    internal static class RemoveRedundantStatementRefactoring
    {
        public static RemoveRedundantContinueStatementRefactoring ContinueStatement { get; } = new RemoveRedundantContinueStatementRefactoring();

        public static RemoveRedundantReturnStatementRefactoring ReturnStatement { get; } = new RemoveRedundantReturnStatementRefactoring();

        public static RemoveRedundantYieldBreakStatementRefactoring YieldBreakStatement { get; } = new RemoveRedundantYieldBreakStatementRefactoring();

        public static Task<Document> RefactorAsync(
            Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken)
        {
            return document.RemoveStatementAsync(statement, cancellationToken);
        }
    }
}
