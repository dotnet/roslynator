// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

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
                    cancellationToken => context.Document.RemovePreprocessorDirectivesAsync(PreprocessorDirectiveKinds.Region | PreprocessorDirectiveKinds.EndRegion, cancellationToken));
            }
        }

        public static void ComputeRefactorings(RefactoringContext context, RegionDirectiveTriviaSyntax regionDirective)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveRegion)
                && context.IsRootCompilationUnit)
            {
                RegionInfo region = SyntaxInfo.RegionInfo(regionDirective);

                if (region.Success)
                {
                    context.RegisterRefactoring(
                        "Remove region",
                        cancellationToken => context.Document.RemoveRegionAsync(region, cancellationToken));
                }
            }
        }

        public static void ComputeRefactorings(RefactoringContext context, EndRegionDirectiveTriviaSyntax endRegionDirective)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveRegion)
                && context.IsRootCompilationUnit)
            {
                RegionInfo region = SyntaxInfo.RegionInfo(endRegionDirective);

                if (region.Success)
                {
                    context.RegisterRefactoring(
                        "Remove region",
                        cancellationToken => context.Document.RemoveRegionAsync(region, cancellationToken));
                }
            }
        }
    }
}
