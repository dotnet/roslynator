// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp.Refactorings.If;
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

        public static async Task ComputeRefactoringAsync(RefactoringContext context, StatementsSelection selectedStatements)
        {
            if (selectedStatements.Any())
            {
                if (context.IsRefactoringEnabled(RefactoringIdentifiers.WrapInUsingStatement))
                {
                    var refactoring = new WrapInUsingStatementRefactoring();
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

                    var options = new IfAnalysisOptions(
                        useCoalesceExpression: context.IsRefactoringEnabled(RefactoringIdentifiers.UseCoalesceExpressionInsteadOfIf),
                        useConditionalExpression: context.IsRefactoringEnabled(RefactoringIdentifiers.UseConditionalExpressionInsteadOfIf),
                        useBooleanExpression: context.IsRefactoringEnabled(RefactoringIdentifiers.SimplifyIf));

                    foreach (IfRefactoring refactoring in IfRefactoring.Analyze(selectedStatements, options, semanticModel, context.CancellationToken))
                    {
                        context.RegisterRefactoring(
                            refactoring.Title,
                            cancellationToken => refactoring.RefactorAsync(context.Document, cancellationToken));
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
                        "Wrap in condition",
                        cancellationToken =>
                        {
                            var refactoring = new WrapInIfStatementRefactoring();
                            return refactoring.RefactorAsync(context.Document, selectedStatements, cancellationToken);
                        });
                }

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.WrapInTryCatch))
                {
                    context.RegisterRefactoring(
                        "Wrap in try-catch",
                        cancellationToken =>
                        {
                            var refactoring = new WrapInTryCatchRefactoring();
                            return refactoring.RefactorAsync(context.Document, selectedStatements, cancellationToken);
                        });
                }
            }
        }
    }
}
