// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class RegionDirectiveTriviaRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, SyntaxNode node)
        {
            if (node.IsAnyKind(SyntaxKind.RegionDirectiveTrivia, SyntaxKind.EndRegionDirectiveTrivia)
                && context.Root.IsKind(SyntaxKind.CompilationUnit))
            {
                context.RegisterRefactoring(
                    "Remove all regions",
                    cancellationToken => RemoveAllRegionsRefactoring.RefactorAsync(context.Document, cancellationToken));
            }
        }
    }
}
