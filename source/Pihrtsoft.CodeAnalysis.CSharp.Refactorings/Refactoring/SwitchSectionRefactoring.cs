// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class SwitchSectionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, SwitchSectionSyntax switchSection)
        {
            if (AddBracesToSwitchSectionRefactoring.CanRefactor(switchSection))
            {
                context.RegisterRefactoring(
                    "Add braces to switch section",
                    cancellationToken => AddBracesToSwitchSectionRefactoring.RefactorAsync(context.Document, switchSection, cancellationToken));
            }

            if (RemoveBracesFromSwitchSectionRefactoring.CanRefactor(context, switchSection))
            {
                context.RegisterRefactoring(
                    "Remove braces from switch section",
                    cancellationToken => RemoveBracesFromSwitchSectionRefactoring.RefactorAsync(context.Document, switchSection, cancellationToken));
            }
        }
    }
}