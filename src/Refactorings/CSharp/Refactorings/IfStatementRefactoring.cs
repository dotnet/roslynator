// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
        private static IfAnalysisOptions DefaultIfAnalysisOptions { get; } = new(
            useCoalesceExpression: true,
            useConditionalExpression: true,
            useBooleanExpression: true,
            useExpression: false);

        public static IfAnalysisOptions GetIfAnalysisOptions(RefactoringContext context)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.UseCoalesceExpressionInsteadOfIf)
                && context.IsRefactoringEnabled(RefactoringDescriptors.ConvertIfToConditionalExpression)
                && context.IsRefactoringEnabled(RefactoringDescriptors.SimplifyIf))
            {
                return DefaultIfAnalysisOptions;
            }

            return new IfAnalysisOptions(
                useCoalesceExpression: context.IsRefactoringEnabled(RefactoringDescriptors.UseCoalesceExpressionInsteadOfIf),
                useConditionalExpression: context.IsRefactoringEnabled(RefactoringDescriptors.ConvertIfToConditionalExpression),
                useBooleanExpression: context.IsRefactoringEnabled(RefactoringDescriptors.SimplifyIf),
                useExpression: false);
        }

        internal static RefactoringDescriptor GetRefactoringDescriptor(IfAnalysis analysis)
        {
            switch (analysis.Kind)
            {
                case IfAnalysisKind.IfElseToAssignmentWithCoalesceExpression:
                case IfAnalysisKind.IfElseToReturnWithCoalesceExpression:
                case IfAnalysisKind.IfElseToYieldReturnWithCoalesceExpression:
                case IfAnalysisKind.IfReturnToReturnWithCoalesceExpression:
                    return RefactoringDescriptors.UseCoalesceExpressionInsteadOfIf;
                case IfAnalysisKind.IfElseToAssignmentWithConditionalExpression:
                case IfAnalysisKind.AssignmentAndIfToAssignmentWithConditionalExpression:
                case IfAnalysisKind.LocalDeclarationAndIfElseAssignmentWithConditionalExpression:
                case IfAnalysisKind.IfElseToReturnWithConditionalExpression:
                case IfAnalysisKind.IfElseToYieldReturnWithConditionalExpression:
                case IfAnalysisKind.IfReturnToReturnWithConditionalExpression:
                    return RefactoringDescriptors.ConvertIfToConditionalExpression;
                case IfAnalysisKind.IfElseToReturnWithBooleanExpression:
                case IfAnalysisKind.IfElseToYieldReturnWithBooleanExpression:
                case IfAnalysisKind.IfReturnToReturnWithBooleanExpression:
                    return RefactoringDescriptors.SimplifyIf;
                case IfAnalysisKind.IfElseToAssignmentWithExpression:
                case IfAnalysisKind.IfElseToAssignmentWithCondition:
                case IfAnalysisKind.IfElseToReturnWithExpression:
                case IfAnalysisKind.IfElseToYieldReturnWithExpression:
                case IfAnalysisKind.IfReturnToReturnWithExpression:
                    return default;
            }

            Debug.Fail(analysis.Kind.ToString());

            return default;
        }

        public static async Task ComputeRefactoringsAsync(RefactoringContext context, IfStatementSyntax ifStatement)
        {
            SyntaxToken ifKeyword = ifStatement.IfKeyword;

            bool isTopmostIf = ifStatement.IsTopmostIf();

            if (context.Span.IsEmptyAndContainedInSpan(ifKeyword)
                || context.Span.IsBetweenSpans(ifStatement))
            {
                if (isTopmostIf
                    && context.IsAnyRefactoringEnabled(
                        RefactoringDescriptors.UseCoalesceExpressionInsteadOfIf,
                        RefactoringDescriptors.ConvertIfToConditionalExpression,
                        RefactoringDescriptors.SimplifyIf))
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    IfAnalysisOptions options = GetIfAnalysisOptions(context);

                    foreach (IfAnalysis analysis in IfAnalysis.Analyze(ifStatement, options, semanticModel, context.CancellationToken))
                    {
                        RefactoringDescriptor refactoring = GetRefactoringDescriptor(analysis);

                        if (context.IsRefactoringEnabled(refactoring))
                        {
                            context.RegisterRefactoring(
                                analysis.Title,
                                ct => IfRefactoring.RefactorAsync(context.Document, analysis, ct),
                                refactoring);
                        }
                    }
                }

                if (context.IsAnyRefactoringEnabled(RefactoringDescriptors.InvertIf, RefactoringDescriptors.InvertIfElse)
                    && context.Span.IsEmptyAndContainedInSpan(ifKeyword))
                {
                    InvertIfRefactoring.ComputeRefactoring(context, ifStatement);
                }

                if (context.IsRefactoringEnabled(RefactoringDescriptors.ConvertIfToSwitch)
                    && isTopmostIf)
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    ConvertIfToSwitchRefactoring.ComputeRefactoring(context, ifStatement, semanticModel);
                }

                if (context.IsRefactoringEnabled(RefactoringDescriptors.SplitIf))
                    SplitIfStatementRefactoring.ComputeRefactoring(context, ifStatement);

                if (context.IsRefactoringEnabled(RefactoringDescriptors.MergeIfWithParentIf)
                    && isTopmostIf
                    && context.Span.IsEmptyAndContainedInSpan(ifKeyword))
                {
                    MergeIfWithParentIfRefactoring.ComputeRefactoring(context, ifStatement);
                }
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.InvertIf)
                && context.Span.IsEmptyAndContainedInSpan(ifKeyword))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                ReduceIfNestingAnalysisResult analysis = ReduceIfNestingAnalysis.Analyze(
                    ifStatement,
                    semanticModel,
                    options: ReduceIfNestingOptions.AllowNestedFix
                        | ReduceIfNestingOptions.AllowIfInsideIfElse
                        | ReduceIfNestingOptions.AllowLoop
                        | ReduceIfNestingOptions.AllowSwitchSection,
                    cancellationToken: context.CancellationToken);

                if (analysis.Success)
                {
                    context.RegisterRefactoring(
                        "Invert if",
                        ct => ReduceIfNestingRefactoring.RefactorAsync(context.Document, ifStatement, analysis.JumpKind, false, ct),
                        RefactoringDescriptors.InvertIf);

                    if (ReduceIfNestingAnalysis.IsFixableRecursively(ifStatement, analysis.JumpKind))
                    {
                        context.RegisterRefactoring(
                            "Invert if (recursively)",
                            ct => ReduceIfNestingRefactoring.RefactorAsync(context.Document, ifStatement, analysis.JumpKind, true, ct),
                            RefactoringDescriptors.InvertIf,
                            "Recursive");
                    }
                }
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.SplitIfElse)
                && context.Span.IsEmptyAndContainedInSpan(ifKeyword))
            {
                SplitIfElseRefactoring.ComputeRefactoring(context, ifStatement);
            }
        }
    }
}
