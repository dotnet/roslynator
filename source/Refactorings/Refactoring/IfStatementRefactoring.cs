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
                if (context.Settings.IsAnyRefactoringEnabled(
                        RefactoringIdentifiers.ReplaceEmbeddedStatementWithBlockInIfElse,
                        RefactoringIdentifiers.ReplaceBlockWithEmbeddedStatementInIfElse)
                    && ifStatement.Else != null)
                {
                    var result = new IfElseChainAnalysisResult(ifStatement);

                    if (result.ReplaceEmbeddedStatementWithBlock
                        && context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceEmbeddedStatementWithBlockInIfElse))
                    {
                        context.RegisterRefactoring(
                            "Replace embedded statement with block (in if-else)",
                            cancellationToken =>
                            {
                                return ReplaceEmbeddedStatementWithBlockInIfElseRefactoring.RefactorAsync(
                                    context.Document,
                                    ifStatement,
                                    cancellationToken);
                            });
                    }

                    if (result.ReplaceBlockWithEmbeddedStatement
                        && context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceBlockWithEmbeddedStatementInIfElse))
                    {
                        context.RegisterRefactoring(
                            "Replace block with embedded statement (in if-else)",
                            cancellationToken =>
                            {
                                return ReplaceBlockWithEmbeddedStatementInIfElseRefactoring.RefactorAsync(
                                    context.Document,
                                    ifStatement,
                                    cancellationToken);
                            });
                    }
                }

                if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.SwapStatementsInIfElse)
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
            }

            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.AddBooleanComparison)
                && ifStatement.Condition != null
                && ifStatement.Condition.Span.Contains(context.Span)
                && context.SupportsSemanticModel)
            {
                await AddBooleanComparisonRefactoring.ComputeRefactoringAsync(context, ifStatement.Condition);
            }
        }
    }
}