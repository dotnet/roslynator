// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis.If;
using Roslynator.CSharp.Analysis.ReduceIfNesting;
using Roslynator.CSharp.Refactorings.ReduceIfNesting;

namespace Roslynator.CSharp.Refactorings
{
    internal static class IfStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, IfStatementSyntax ifStatement)
        {
            if (ifStatement.IsTopmostIf()
                && (context.Span.IsEmptyAndContainedInSpan(ifStatement.IfKeyword) || context.Span.IsBetweenSpans(ifStatement)))
            {
                if (context.IsAnyRefactoringEnabled(
                    RefactoringIdentifiers.UseCoalesceExpressionInsteadOfIf,
                    RefactoringIdentifiers.UseConditionalExpressionInsteadOfIf,
                    RefactoringIdentifiers.SimplifyIf))
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    var options = new IfAnalysisOptions(
                        useCoalesceExpression: context.IsRefactoringEnabled(RefactoringIdentifiers.UseCoalesceExpressionInsteadOfIf),
                        useConditionalExpression: context.IsRefactoringEnabled(RefactoringIdentifiers.UseConditionalExpressionInsteadOfIf),
                        useBooleanExpression: context.IsRefactoringEnabled(RefactoringIdentifiers.SimplifyIf),
                        useExpression: false);

                    foreach (IfAnalysis analysis in IfAnalysis.Analyze(ifStatement, options, semanticModel, context.CancellationToken))
                    {
                        context.RegisterRefactoring(
                            analysis.Title,
                            cancellationToken => IfRefactoring.RefactorAsync(context.Document, analysis, cancellationToken));
                    }
                }

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.SwapIfElse))
                    SwapIfElseRefactoring.ComputeRefactoring(context, ifStatement);

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceIfWithSwitch))
                    await ReplaceIfWithSwitchRefactoring.ComputeRefactoringAsync(context, ifStatement).ConfigureAwait(false);

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.SplitIfStatement))
                    SplitIfStatementRefactoring.ComputeRefactoring(context, ifStatement);

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.MergeIfWithParentIf)
                    && context.Span.IsEmptyAndContainedInSpan(ifStatement.IfKeyword))
                {
                    MergeIfWithParentIfRefactoring.ComputeRefactoring(context, ifStatement);
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReduceIfNesting)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(ifStatement.IfKeyword))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                ReduceIfNestingAnalysisResult analysis = ReduceIfNestingAnalysis.Analyze(
                    ifStatement,
                    semanticModel,
                    options: ReduceIfNestingOptions.AllowNestedFix
                        | ReduceIfNestingOptions.AllowIfInsideIfElse
                        | ReduceIfNestingOptions.AllowLoop
                        | ReduceIfNestingOptions.AllowSwitchSection,
                    taskSymbol: semanticModel.GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_Task),
                    cancellationToken: context.CancellationToken);

                if (analysis.Success)
                {
                    context.RegisterRefactoring(
                        "Reduce if nesting",
                        cancellationToken => ReduceIfNestingRefactoring.RefactorAsync(context.Document, ifStatement, analysis.JumpKind, false, cancellationToken));

                    if (ReduceIfNestingAnalysis.IsFixableRecursively(ifStatement, analysis.JumpKind))
                    {
                        context.RegisterRefactoring(
                            "Reduce if nesting (recursively)",
                            cancellationToken => ReduceIfNestingRefactoring.RefactorAsync(context.Document, ifStatement, analysis.JumpKind, true, cancellationToken));
                    }
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.SplitIfElse)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(ifStatement.IfKeyword))
            {
                SplitIfElseRefactoring.ComputeRefactoring(context, ifStatement);
            }
        }
    }
}