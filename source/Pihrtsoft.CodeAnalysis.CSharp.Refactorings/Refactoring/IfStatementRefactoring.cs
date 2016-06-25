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
            if (ifStatement.IfKeyword.Span.Contains(context.Span)
                && IfElseChainAnalysis.IsTopmostIf(ifStatement))
            {
                if (ifStatement.Else != null)
                {
                    var result = new IfElseChainAnalysisResult(ifStatement);

                    if (result.AddBracesToChain)
                    {
                        context.RegisterRefactoring(
                            "Add braces to if-else chain",
                            cancellationToken =>
                            {
                                return AddBracesToIfElseChainRefactoring.RefactorAsync(
                                    context.Document,
                                    ifStatement,
                                    cancellationToken);
                            });
                    }

                    if (result.RemoveBracesFromChain)
                    {
                        context.RegisterRefactoring(
                            "Remove braces from if-else chain",
                            cancellationToken =>
                            {
                                return RemoveBracesFromIfElseChainRefactoring.RefactorAsync(
                                    context.Document,
                                    ifStatement,
                                    cancellationToken);
                            });
                    }
                }

                if (SwapStatementInIfElseRefactoring.CanRefactor(context, ifStatement))
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
            }

            if (ifStatement.Condition != null
                && ifStatement.Condition.Span.Contains(context.Span)
                && context.SupportsSemanticModel)
            {
                await AddBooleanComparisonRefactoring.ComputeRefactoringAsync(context, ifStatement.Condition);
            }
        }
    }
}