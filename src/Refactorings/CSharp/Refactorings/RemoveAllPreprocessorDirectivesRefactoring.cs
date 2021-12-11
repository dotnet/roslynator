// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveAllPreprocessorDirectivesRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.RemoveAllPreprocessorDirectives))
            {
                context.RegisterRefactoring(
                    "Remove all directives",
                    ct => context.Document.RemovePreprocessorDirectivesAsync(PreprocessorDirectiveFilter.All, ct),
                    RefactoringDescriptors.RemoveAllPreprocessorDirectives);
            }
        }
    }
}
