// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SwitchStatementRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, SwitchStatementSyntax switchStatement)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.AddMissingCasesToSwitchStatement))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                AddMissingCasesToSwitchStatementRefactoring.ComputeRefactoring(context, switchStatement, semanticModel);
            }

            SelectedSwitchSectionsRefactoring.ComputeRefactorings(context, switchStatement);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ConvertSwitchToIf))
            {
                if (context.Span.IsEmptyAndContainedInSpan(switchStatement.SwitchKeyword)
                    || context.Span.IsBetweenSpans(switchStatement))
                {
                    ConvertSwitchToIfRefactoring.ComputeRefactoring(context, switchStatement);
                }
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.CopySwitchSection))
                CopySwitchSectionRefactoring.ComputeRefactoring(context, switchStatement);
        }
    }
}
