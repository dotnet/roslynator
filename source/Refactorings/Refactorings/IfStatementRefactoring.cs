// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.Refactorings
{
    internal static class IfStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, IfStatementSyntax ifStatement)
        {
            if (context.IsAnyRefactoringEnabled(
                    RefactoringIdentifiers.SwapStatementsInIfElse,
                    RefactoringIdentifiers.ReplaceIfElseWithConditionalExpression)
                && IfElseAnalysis.IsTopmostIf(ifStatement)
                && context.Span.IsBetweenSpans(ifStatement))
            {
                if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceIfElseWithConditionalExpression))
                    ReplaceIfElseWithConditionalExpressionRefactoring.ComputeRefactoring(context, ifStatement);

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.SwapStatementsInIfElse)
                    && SwapStatementInIfElseRefactoring.CanRefactor(ifStatement))
                {
                    context.RegisterRefactoring(
                        "Swap statements in if-else",
                        cancellationToken =>
                        {
                            return SwapStatementInIfElseRefactoring.RefactorAsync(
                                context.Document,
                                ifStatement,
                                cancellationToken);
                        });
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddBooleanComparison)
                && ifStatement.Condition != null
                && ifStatement.Condition.Span.Contains(context.Span)
                && context.SupportsSemanticModel)
            {
                await AddBooleanComparisonRefactoring.ComputeRefactoringAsync(context, ifStatement.Condition).ConfigureAwait(false);
            }
        }
    }
}