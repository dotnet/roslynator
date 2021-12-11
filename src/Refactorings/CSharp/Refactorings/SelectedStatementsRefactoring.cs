// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.Analysis.If;
using Roslynator.CSharp.Refactorings.WrapStatements;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SelectedStatementsRefactoring
    {
        public static bool IsAnyRefactoringEnabled(RefactoringContext context)
        {
            return context.IsRefactoringEnabled(RefactoringDescriptors.WrapStatementsInUsingStatement)
                || context.IsRefactoringEnabled(RefactoringDescriptors.UseObjectInitializer)
                || context.IsRefactoringEnabled(RefactoringDescriptors.MergeIfStatements)
                || context.IsRefactoringEnabled(RefactoringDescriptors.ConvertStatementsToIfElse)
                || context.IsRefactoringEnabled(RefactoringDescriptors.MergeLocalDeclarations)
                || context.IsRefactoringEnabled(RefactoringDescriptors.WrapStatementsInCondition)
                || context.IsRefactoringEnabled(RefactoringDescriptors.WrapLinesInTryCatch)
                || context.IsRefactoringEnabled(RefactoringDescriptors.UseCoalesceExpressionInsteadOfIf)
                || context.IsRefactoringEnabled(RefactoringDescriptors.ConvertIfToConditionalExpression)
                || context.IsRefactoringEnabled(RefactoringDescriptors.SimplifyIf)
                || context.IsRefactoringEnabled(RefactoringDescriptors.CheckExpressionForNull)
                || context.IsRefactoringEnabled(RefactoringDescriptors.ConvertWhileToFor);
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, StatementListSelection selectedStatements)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.WrapStatementsInUsingStatement))
            {
                var refactoring = new WrapStatementsInUsingStatementRefactoring();
                await refactoring.ComputeRefactoringAsync(context, selectedStatements).ConfigureAwait(false);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.UseObjectInitializer))
                await UseObjectInitializerRefactoring.ComputeRefactoringsAsync(context, selectedStatements).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.MergeIfStatements))
                MergeIfStatementsRefactoring.ComputeRefactorings(context, selectedStatements);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ConvertStatementsToIfElse))
                ConvertStatementsToIfElseRefactoring.ComputeRefactorings(context, selectedStatements);

            if (context.IsAnyRefactoringEnabled(
                RefactoringDescriptors.UseCoalesceExpressionInsteadOfIf,
                RefactoringDescriptors.ConvertIfToConditionalExpression,
                RefactoringDescriptors.SimplifyIf))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                IfAnalysisOptions options = IfStatementRefactoring.GetIfAnalysisOptions(context);

                foreach (IfAnalysis analysis in IfAnalysis.Analyze(selectedStatements, options, semanticModel, context.CancellationToken))
                {
                    RefactoringDescriptor refactoring = IfStatementRefactoring.GetRefactoringDescriptor(analysis);

                    if (context.IsRefactoringEnabled(refactoring))
                    {
                        context.RegisterRefactoring(
                            analysis.Title,
                            ct => IfRefactoring.RefactorAsync(context.Document, analysis, ct),
                            refactoring);
                    }
                }
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.MergeLocalDeclarations))
                await MergeLocalDeclarationsRefactoring.ComputeRefactoringsAsync(context, selectedStatements).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.RemoveUnnecessaryAssignment))
                RemoveUnnecessaryAssignmentRefactoring.ComputeRefactorings(context, selectedStatements);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.CheckExpressionForNull))
                await CheckExpressionForNullRefactoring.ComputeRefactoringAsync(context, selectedStatements).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ConvertWhileToFor))
                await ConvertWhileToForRefactoring.ComputeRefactoringAsync(context, selectedStatements).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.WrapStatementsInCondition))
            {
                context.RegisterRefactoring(
                    WrapInIfStatementRefactoring.Title,
                    ct => WrapInIfStatementRefactoring.Instance.RefactorAsync(context.Document, selectedStatements, ct),
                    RefactoringDescriptors.WrapStatementsInCondition);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.WrapLinesInTryCatch))
            {
                context.RegisterRefactoring(
                    WrapLinesInTryCatchRefactoring.Title,
                    ct => WrapLinesInTryCatchRefactoring.Instance.RefactorAsync(context.Document, selectedStatements, ct),
                    RefactoringDescriptors.WrapLinesInTryCatch);
            }
        }
    }
}
