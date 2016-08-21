// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings
{
    internal static class RegionDirectiveTriviaRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveAllRegionDirectives)
                && context.Root.IsKind(SyntaxKind.CompilationUnit))
            {
                context.RegisterRefactoring(
                    "Remove all '#region' directives",
                    cancellationToken => SyntaxRemover.RemoveRegionDirectivesAsync(context.Document, cancellationToken));
            }
        }
    }
}
