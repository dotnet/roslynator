// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveAllPreprocessorDirectivesRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveAllPreprocessorDirectives))
            {
                context.RegisterRefactoring(
                    "Remove all directives",
                    cancellationToken => context.Document.RemovePreprocessorDirectivesAsync(PreprocessorDirectiveKinds.All, cancellationToken));
            }
        }
    }
}