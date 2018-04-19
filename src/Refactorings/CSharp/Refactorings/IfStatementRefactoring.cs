// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;
using Roslynator.CSharp.Analysis.If;
using Roslynator.CSharp.Analysis.ReduceIfNesting;
using Roslynator.CSharp.Refactorings.ReduceIfNesting;

namespace Roslynator.CSharp.Refactorings
{
    internal static class IfStatementRefactoring
    {
        private static IfAnalysisOptions DefaultIfAnalysisOptions { get; } = new IfAnalysisOptions(
            useCoalesceExpression: true,
            useConditionalExpression: true,
            useBooleanExpression: true,
            useExpression: false);

        public static IfAnalysisOptions GetIfAnalysisOptions(RefactoringContext context)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.UseCoalesceExpressionInsteadOfIf)
                && context.IsRefactoringEnabled(RefactoringIdentifiers.UseConditionalExpressionInsteadOfIf)
                && context.IsRefactoringEnabled(RefactoringIdentifiers.SimplifyIf))
            {
                return DefaultIfAnalysisOptions;
            }

            return new IfAnalysisOptions(
                useCoalesceExpression: context.IsRefactoringEnabled(RefactoringIdentifiers.UseCoalesceExpressionInsteadOfIf),
                useConditionalExpression: context.IsRefactoringEnabled(RefactoringIdentifiers.UseConditionalExpressionInsteadOfIf),
                useBooleanExpression: context.IsRefactoringEnabled(RefactoringIdentifiers.SimplifyIf),
                useExpression: false);
        }

        internal static string GetRefactoringIdentifier(IfAnalysis analysis)
        {
            switch (analysis.Kind)
            {
                case IfAnalysisKind.IfElseToAssignmentWithCoalesceExpression:
                case IfAnalysisKind.IfElseToReturnWithCoalesceExpression:
                case IfAnalysisKind.IfElseToYieldReturnWithCoalesceExpression:
                case IfAnalysisKind.IfReturnToReturnWithCoalesceExpression:
                    return RefactoringIdentifiers.UseCoalesceExpressionInsteadOfIf;
                case IfAnalysisKind.IfElseToAssignmentWithConditionalExpression:
                case IfAnalysisKind.AssignmentAndIfElseToAssignmentWithConditionalExpression:
                case IfAnalysisKind.LocalDeclarationAndIfElseAssignmentWithConditionalExpression:
                case IfAnalysisKind.IfElseToReturnWithConditionalExpression:
                case IfAnalysisKind.IfElseToYieldReturnWithConditionalExpression:
                case IfAnalysisKind.IfReturnToReturnWithConditionalExpression:
                    return RefactoringIdentifiers.UseConditionalExpressionInsteadOfIf;
                case IfAnalysisKind.IfElseToReturnWithBooleanExpression:
                case IfAnalysisKind.IfElseToYieldReturnWithBooleanExpression:
                case IfAnalysisKind.IfReturnToReturnWithBooleanExpression:
                    return RefactoringIdentifiers.SimplifyIf;
                case IfAnalysisKind.IfElseToAssignmentWithExpression:
                case IfAnalysisKind.IfElseToAssignmentWithCondition:
                case IfAnalysisKind.IfElseToReturnWithExpression:
                case IfAnalysisKind.IfElseToYieldReturnWithExpression:
                case IfAnalysisKind.IfReturnToReturnWithExpression:
                    return null;
            }

            Debug.Fail(analysis.Kind.ToString());

            return null;
        }

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

                    IfAnalysisOptions options = GetIfAnalysisOptions(context);

                    foreach (IfAnalysis analysis in IfAnalysis.Analyze(ifStatement, options, semanticModel, context.CancellationToken))
                    {
                        string refactoringId = GetRefactoringIdentifier(analysis);

                        if (context.IsRefactoringEnabled(refactoringId))
                        {
                            context.RegisterRefactoring(
                                analysis.Title,
                                cancellationToken => IfRefactoring.RefactorAsync(context.Document, analysis, cancellationToken),
                                equivalenceKey: refactoringId);
                        }
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
                        cancellationToken => ReduceIfNestingRefactoring.RefactorAsync(context.Document, ifStatement, analysis.JumpKind, false, cancellationToken),
                        RefactoringIdentifiers.ReduceIfNesting);

                    if (ReduceIfNestingAnalysis.IsFixableRecursively(ifStatement, analysis.JumpKind))
                    {
                        context.RegisterRefactoring(
                            "Reduce if nesting (recursively)",
                            cancellationToken => ReduceIfNestingRefactoring.RefactorAsync(context.Document, ifStatement, analysis.JumpKind, true, cancellationToken),
                            RefactoringIdentifiers.ReduceIfNesting);
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