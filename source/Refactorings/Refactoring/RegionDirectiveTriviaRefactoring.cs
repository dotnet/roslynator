// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class RegionDirectiveTriviaRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context)
        {
            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.RemoveAllRegionDirectives)
                && context.Root.IsKind(SyntaxKind.CompilationUnit))
            {
                context.RegisterRefactoring(
                    "Remove all region directives",
                    cancellationToken => RemoveAllRegionsRefactoring.RefactorAsync(context.Document, cancellationToken));
            }
        }
    }
}
