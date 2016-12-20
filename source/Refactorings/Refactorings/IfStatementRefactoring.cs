// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings.ReplaceIfWithStatement;

namespace Roslynator.CSharp.Refactorings
{
    internal static class IfStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, IfStatementSyntax ifStatement)
        {
            if (context.IsAnyRefactoringEnabled(
                    RefactoringIdentifiers.SwapStatementsInIfElse,
                    RefactoringIdentifiers.ReplaceIfElseWithAssignment,
                    RefactoringIdentifiers.ReplaceIfStatementWithReturnStatement,
                    RefactoringIdentifiers.ReplaceIfElseWithSwitch)
                && IfElseChain.IsTopmostIf(ifStatement)
                && context.Span.IsBetweenSpans(ifStatement))
            {
                if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceIfStatementWithReturnStatement))
                    await ReplaceIfWithStatementRefactoring.ComputeRefactoringAsync(context, ifStatement).ConfigureAwait(false);

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceIfElseWithAssignment))
                    ReplaceIfElseWithAssignmentRefactoring.ComputeRefactoring(context, ifStatement);

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.SwapStatementsInIfElse))
                    SwapStatementInIfElseRefactoring.ComputeRefactoring(context, ifStatement);

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceIfElseWithSwitch))
                    await ReplaceIfElseWithSwitchRefactoring.ComputeRefactoringAsync(context, ifStatement).ConfigureAwait(false);
            }
        }
    }
}