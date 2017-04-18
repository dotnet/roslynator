// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RegionDirectiveTriviaRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveAllRegionDirectives)
                && context.IsRootCompilationUnit)
            {
                context.RegisterRefactoring(
                    "Remove all region directives",
                    cancellationToken => context.Document.RemoveDirectivesAsync(DirectiveRemoveOptions.Region, cancellationToken));
            }
        }

        public static void ComputeRefactorings(RefactoringContext context, RegionDirectiveTriviaSyntax regionDirective)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveRegion)
                && context.IsRootCompilationUnit)
            {
                context.RegisterRefactoring(
                    "Remove region",
                    cancellationToken => context.Document.RemoveRegionAsync(regionDirective, cancellationToken));
            }
        }

        public static void ComputeRefactorings(RefactoringContext context, EndRegionDirectiveTriviaSyntax endRegionDirective)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveRegion)
                && context.IsRootCompilationUnit)
            {
                context.RegisterRefactoring(
                    "Remove region",
                    cancellationToken => context.Document.RemoveRegionAsync(endRegionDirective, cancellationToken));
            }
        }
    }
}
