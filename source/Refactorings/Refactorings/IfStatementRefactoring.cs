// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings.If;

namespace Roslynator.CSharp.Refactorings
{
    internal static class IfStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, IfStatementSyntax ifStatement)
        {
            if (context.IsAnyRefactoringEnabled(
                    RefactoringIdentifiers.UseCoalesceExpressionInsteadOfIf,
                    RefactoringIdentifiers.UseConditionalExpressionInsteadOfIf,
                    RefactoringIdentifiers.SimplifyIf,
                    RefactoringIdentifiers.SwapStatementsInIfElse,
                    RefactoringIdentifiers.ReplaceIfElseWithSwitch)
                && ifStatement.IsTopmostIf()
                && context.Span.IsBetweenSpans(ifStatement))
            {
                if (context.IsAnyRefactoringEnabled(
                    RefactoringIdentifiers.UseCoalesceExpressionInsteadOfIf,
                    RefactoringIdentifiers.UseConditionalExpressionInsteadOfIf,
                    RefactoringIdentifiers.SimplifyIf,
                    RefactoringIdentifiers.SplitIfStatement))
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    var options = new IfAnalysisOptions(
                        useCoalesceExpression: context.IsRefactoringEnabled(RefactoringIdentifiers.UseCoalesceExpressionInsteadOfIf),
                        useConditionalExpression: context.IsRefactoringEnabled(RefactoringIdentifiers.UseConditionalExpressionInsteadOfIf),
                        useBooleanExpression: context.IsRefactoringEnabled(RefactoringIdentifiers.SimplifyIf));

                    foreach (IfRefactoring refactoring in IfRefactoring.Analyze(ifStatement, options, semanticModel, context.CancellationToken))
                    {
                        context.RegisterRefactoring(
                            refactoring.Title,
                            cancellationToken => refactoring.RefactorAsync(context.Document, cancellationToken));
                    }
                }

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.SwapStatementsInIfElse))
                    SwapStatementInIfElseRefactoring.ComputeRefactoring(context, ifStatement);

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceIfElseWithSwitch))
                    await ReplaceIfElseWithSwitchRefactoring.ComputeRefactoringAsync(context, ifStatement).ConfigureAwait(false);

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.SplitIfStatement))
                    SplitIfStatementRefactoring.ComputeRefactoring(context, ifStatement);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReduceIfNesting)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(ifStatement.IfKeyword))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                if (ReduceIfNestingRefactoring.IsFixable(
                    ifStatement,
                    semanticModel,
                    semanticModel.GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_Task),
                    context.CancellationToken,
                    topLevelOnly: false))
                {
                    context.RegisterRefactoring(
                        "Reduce if nesting",
                        cancellationToken => ReduceIfNestingRefactoring.RefactorAsync(context.Document, ifStatement, context.CancellationToken));
                }
            }
        }
    }
}