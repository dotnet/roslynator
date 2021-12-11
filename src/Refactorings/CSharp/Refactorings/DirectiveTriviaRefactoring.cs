// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class DirectiveTriviaRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, DirectiveTriviaSyntax directive)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.RemovePreprocessorDirective)
                && directive.IsKind(
                    SyntaxKind.IfDirectiveTrivia,
                    SyntaxKind.ElseDirectiveTrivia,
                    SyntaxKind.ElifDirectiveTrivia,
                    SyntaxKind.EndIfDirectiveTrivia,
                    SyntaxKind.RegionDirectiveTrivia,
                    SyntaxKind.EndRegionDirectiveTrivia))
            {
                context.RegisterRefactoring(
                    "Remove directive",
                    ct =>
                    {
                        return context.Document.RemovePreprocessorDirectivesAsync(
                            directive.GetRelatedDirectives().ToImmutableArray(),
                            ct);
                    },
                    RefactoringDescriptors.RemovePreprocessorDirective);
            }
        }
    }
}
