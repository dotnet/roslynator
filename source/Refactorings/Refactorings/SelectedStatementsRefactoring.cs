// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
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
                || context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceIfElseWithSwitch);
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, SelectedStatementsInfo info)
        {
            if (info.IsAnySelected)
            {
                if (context.IsRefactoringEnabled(RefactoringIdentifiers.WrapInUsingStatement)
                    && context.SupportsSemanticModel)
                {
                    var refactoring = new WrapInUsingStatementRefactoring();
                    await refactoring.ComputeRefactoringAsync(context, info).ConfigureAwait(false);
                }

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.CollapseToInitializer))
                    await CollapseToInitializerRefactoring.ComputeRefactoringsAsync(context, info).ConfigureAwait(false);

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.MergeIfStatements))
                    MergeIfStatementsRefactoring.ComputeRefactorings(context, info);

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceIfElseWithSwitch))
                    ReplaceIfElseWithSwitchRefactoring.ComputeRefactoring(context, info);

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.MergeLocalDeclarations)
                    && context.SupportsSemanticModel)
                {
                    await MergeLocalDeclarationsRefactoring.ComputeRefactoringsAsync(context, info).ConfigureAwait(false);
                }

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.MergeAssignmentExpressionWithReturnStatement))
                    MergeAssignmentExpressionWithReturnStatementRefactoring.ComputeRefactorings(context, info);

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.WrapInCondition))
                {
                    context.RegisterRefactoring(
                        "Wrap in condition",
                        cancellationToken =>
                        {
                            var refactoring = new WrapInIfStatementRefactoring();
                            return refactoring.RefactorAsync(context.Document, info, cancellationToken);
                        });
                }

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.WrapInTryCatch))
                {
                    context.RegisterRefactoring(
                        "Wrap in try-catch",
                        cancellationToken =>
                        {
                            var refactoring = new WrapInTryCatchRefactoring();
                            return refactoring.RefactorAsync(context.Document, info, cancellationToken);
                        });
                }
            }
        }
    }
}
