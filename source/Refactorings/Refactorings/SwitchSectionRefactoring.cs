// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings
{
    internal static class SwitchSectionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, SwitchSectionSyntax switchSection)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddBracesToSwitchSection)
                && AddBracesToSwitchSectionRefactoring.CanRefactor(switchSection))
            {
                context.RegisterRefactoring(
                    "Add braces to section",
                    cancellationToken => AddBracesToSwitchSectionRefactoring.RefactorAsync(context.Document, switchSection, cancellationToken));
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveBracesFromSwitchSection)
                && RemoveBracesFromSwitchSectionRefactoring.CanRefactor(context, switchSection))
            {
                context.RegisterRefactoring(
                    "Remove braces from section",
                    cancellationToken => RemoveBracesFromSwitchSectionRefactoring.RefactorAsync(context.Document, switchSection, cancellationToken));
            }
        }
    }
}