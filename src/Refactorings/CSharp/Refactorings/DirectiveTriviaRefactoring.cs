// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings;

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
                        PreprocessorDirectiveRemoveOptions.None,
                        ct);
                },
                RefactoringDescriptors.RemovePreprocessorDirective);

            List<DirectiveTriviaSyntax> directives = directive.GetRelatedDirectives();

            if (directives.Count > 1)
            {
                context.RegisterRefactoring(
                    "Remove directive (including content)",
                    ct =>
                    {
                        return context.Document.RemovePreprocessorDirectivesAsync(
                            directives,
                            PreprocessorDirectiveRemoveOptions.IncludeContent,
                            ct);
                    },
                    RefactoringDescriptors.RemovePreprocessorDirective,
                    "IncludingContent");
            }
        }
    }
}
