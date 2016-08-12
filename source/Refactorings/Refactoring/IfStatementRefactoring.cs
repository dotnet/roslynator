// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis.CSharp.Analysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class IfStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, IfStatementSyntax ifStatement)
        {
            bool isTopmostIf = IfElseChainAnalysis.IsTopmostIf(ifStatement);

            if (ifStatement.IfKeyword.Span.Contains(context.Span)
                && isTopmostIf)
            {
                if (context.Settings.IsAnyRefactoringEnabled(
                        RefactoringIdentifiers.AddBracesToIfElse,
                        RefactoringIdentifiers.RemoveBracesFromIfElse)
                    && ifStatement.Else != null)
                {
                    var result = new IfElseChainAnalysisResult(ifStatement);

                    if (result.AddBraces
                        && context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.AddBracesToIfElse))
                    {
                        context.RegisterRefactoring(
                            "Add braces to if-else",
                            cancellationToken =>
                            {
                                return AddBracesToIfElseRefactoring.RefactorAsync(
                                    context.Document,
                                    ifStatement,
                                    cancellationToken);
                            });
                    }

                    if (result.RemoveBraces
                        && context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.RemoveBracesFromIfElse))
                    {
                        context.RegisterRefactoring(
                            "Remove braces from if-else",
                            cancellationToken =>
                            {
                                return RemoveBracesFromIfElseElseRefactoring.RefactorAsync(
                                    context.Document,
                                    ifStatement,
                                    cancellationToken);
                            });
                    }
                }
            }

            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.SwapStatementsInIfElse)
                && isTopmostIf
                && ifStatement.Span.Equals(context.Span)
                && SwapStatementInIfElseRefactoring.CanRefactor(context, ifStatement))
            {
                context.RegisterRefactoring(
                    "Swap statements",
                    cancellationToken =>
                    {
                        return SwapStatementInIfElseRefactoring.RefactorAsync(
                            context.Document,
                            ifStatement,
                            cancellationToken);
                    });
            }

            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.AddBooleanComparison)
                && ifStatement.Condition != null
                && ifStatement.Condition.Span.Contains(context.Span)
                && context.SupportsSemanticModel)
            {
                await AddBooleanComparisonRefactoring.ComputeRefactoringAsync(context, ifStatement.Condition).ConfigureAwait(false);
            }
        }
    }
}