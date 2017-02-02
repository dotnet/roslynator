// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Roslynator.CSharp.Refactorings.ReplaceIfWithStatement;
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
                || context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceIfStatementWithReturnStatement)
                || context.IsRefactoringEnabled(RefactoringIdentifiers.CheckExpressionForNull)
                || context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceWhileWithFor);
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, SelectedStatementCollection selectedStatements)
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

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceIfStatementWithReturnStatement))
                    ReplaceIfAndReturnWithReturnRefactoring.ComputeRefactoring(context, selectedStatements);

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.MergeLocalDeclarations))
                    await MergeLocalDeclarationsRefactoring.ComputeRefactoringsAsync(context, selectedStatements).ConfigureAwait(false);

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.MergeAssignmentExpressionWithReturnStatement))
                    MergeAssignmentExpressionWithReturnStatementRefactoring.ComputeRefactorings(context, selectedStatements);

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.CheckExpressionForNull))
                    await CheckExpressionForNullRefactoring.ComputeRefactoringAsync(context, selectedStatements).ConfigureAwait(false);

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceWhileWithFor))
                    await ReplaceWhileWithForRefactoring.ComputeRefactoringAsync(context, selectedStatements).ConfigureAwait(false);

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
