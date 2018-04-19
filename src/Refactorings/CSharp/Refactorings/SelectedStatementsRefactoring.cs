// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
            return context.IsRefactoringEnabled(RefactoringIdentifiers.WrapInUsingStatement)
                || context.IsRefactoringEnabled(RefactoringIdentifiers.CollapseToInitializer)
                || context.IsRefactoringEnabled(RefactoringIdentifiers.MergeIfStatements)
                || context.IsRefactoringEnabled(RefactoringIdentifiers.MergeLocalDeclarations)
                || context.IsRefactoringEnabled(RefactoringIdentifiers.WrapInCondition)
                || context.IsRefactoringEnabled(RefactoringIdentifiers.WrapInTryCatch)
                || context.IsRefactoringEnabled(RefactoringIdentifiers.UseCoalesceExpressionInsteadOfIf)
                || context.IsRefactoringEnabled(RefactoringIdentifiers.UseConditionalExpressionInsteadOfIf)
                || context.IsRefactoringEnabled(RefactoringIdentifiers.SimplifyIf)
                || context.IsRefactoringEnabled(RefactoringIdentifiers.CheckExpressionForNull)
                || context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceWhileWithFor)
                || context.IsRefactoringEnabled(RefactoringIdentifiers.WrapInElseClause);
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, StatementListSelection selectedStatements)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.WrapInUsingStatement))
            {
                var refactoring = new WrapStatements.WrapInUsingStatementRefactoring();
                await refactoring.ComputeRefactoringAsync(context, selectedStatements).ConfigureAwait(false);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.CollapseToInitializer))
                await CollapseToInitializerRefactoring.ComputeRefactoringsAsync(context, selectedStatements).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.MergeIfStatements))
                MergeIfStatementsRefactoring.ComputeRefactorings(context, selectedStatements);

            if (context.IsAnyRefactoringEnabled(
                RefactoringIdentifiers.UseCoalesceExpressionInsteadOfIf,
                RefactoringIdentifiers.UseConditionalExpressionInsteadOfIf,
                RefactoringIdentifiers.SimplifyIf))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                IfAnalysisOptions options = IfStatementRefactoring.GetIfAnalysisOptions(context);

                foreach (IfAnalysis analysis in IfAnalysis.Analyze(selectedStatements, options, semanticModel, context.CancellationToken))
                {
                    string refactoringId = IfStatementRefactoring.GetRefactoringIdentifier(analysis);

                    if (context.IsRefactoringEnabled(refactoringId))
                    {
                        context.RegisterRefactoring(
                            analysis.Title,
                            cancellationToken => IfRefactoring.RefactorAsync(context.Document, analysis, cancellationToken),
                            equivalenceKey: refactoringId);
                    }
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.MergeLocalDeclarations))
                await MergeLocalDeclarationsRefactoring.ComputeRefactoringsAsync(context, selectedStatements).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.MergeAssignmentExpressionWithReturnStatement))
                MergeAssignmentExpressionWithReturnStatementRefactoring.ComputeRefactorings(context, selectedStatements);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.CheckExpressionForNull))
                await CheckExpressionForNullRefactoring.ComputeRefactoringAsync(context, selectedStatements).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceWhileWithFor))
                await ReplaceWhileWithForRefactoring.ComputeRefactoringAsync(context, selectedStatements).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.WrapInElseClause))
                WrapInElseClauseRefactoring.ComputeRefactoring(context, selectedStatements);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.WrapInCondition))
            {
                context.RegisterRefactoring(
                    WrapInIfStatementRefactoring.Title,
                    ct => WrapInIfStatementRefactoring.Instance.RefactorAsync(context.Document, selectedStatements, ct),
                    RefactoringIdentifiers.WrapInCondition);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.WrapInTryCatch))
            {
                context.RegisterRefactoring(
                    WrapInTryCatchRefactoring.Title,
                    ct => WrapInTryCatchRefactoring.Instance.RefactorAsync(context.Document, selectedStatements, ct),
                    RefactoringIdentifiers.WrapInTryCatch);
            }
        }
    }
}
