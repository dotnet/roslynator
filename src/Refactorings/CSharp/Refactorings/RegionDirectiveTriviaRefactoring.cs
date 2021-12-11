// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RegionDirectiveTriviaRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.RemoveAllRegionDirectives)
                && context.IsRootCompilationUnit)
            {
                context.RegisterRefactoring(
                    "Remove all region directives",
                    ct => context.Document.RemovePreprocessorDirectivesAsync(PreprocessorDirectiveFilter.Region | PreprocessorDirectiveFilter.EndRegion, ct),
                    RefactoringDescriptors.RemoveAllRegionDirectives);
            }
        }

        public static void ComputeRefactorings(RefactoringContext context, RegionDirectiveTriviaSyntax regionDirective)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.RemoveRegion)
                && context.IsRootCompilationUnit)
            {
                RegionInfo region = SyntaxInfo.RegionInfo(regionDirective);

                if (region.Success)
                {
                    context.RegisterRefactoring(
                        "Remove region",
                        ct => context.Document.RemoveRegionAsync(region, ct),
                        RefactoringDescriptors.RemoveRegion);
                }
            }
        }

        public static void ComputeRefactorings(RefactoringContext context, EndRegionDirectiveTriviaSyntax endRegionDirective)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.RemoveRegion)
                && context.IsRootCompilationUnit)
            {
                RegionInfo region = SyntaxInfo.RegionInfo(endRegionDirective);

                if (region.Success)
                {
                    context.RegisterRefactoring(
                        "Remove region",
                        ct => context.Document.RemoveRegionAsync(region, ct),
                        RefactoringDescriptors.RemoveRegion);
                }
            }
        }
    }
}
