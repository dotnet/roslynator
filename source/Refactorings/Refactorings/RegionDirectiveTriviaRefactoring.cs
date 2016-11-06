// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RegionDirectiveTriviaRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveAllRegionDirectives)
                && context.Root.IsKind(SyntaxKind.CompilationUnit))
            {
                context.RegisterRefactoring(
                    "Remove all region directives",
                    cancellationToken => SyntaxRemover.RemoveRegionDirectivesAsync(context.Document, cancellationToken));
            }
        }

        public static void ComputeRefactorings(RefactoringContext context, RegionDirectiveTriviaSyntax regionDirective)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveRegion)
                && context.Root.IsKind(SyntaxKind.CompilationUnit))
            {
                context.RegisterRefactoring(
                    "Remove region",
                    cancellationToken => SyntaxRemover.RemoveRegionAsync(context.Document, regionDirective, cancellationToken));
            }
        }
    }
}
