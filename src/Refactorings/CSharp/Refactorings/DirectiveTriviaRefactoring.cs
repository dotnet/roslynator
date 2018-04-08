// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class DirectiveTriviaRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, DirectiveTriviaSyntax directiveTrivia)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveDirectiveAndRelatedDirectives)
                && directiveTrivia.IsKind(
                    SyntaxKind.IfDirectiveTrivia,
                    SyntaxKind.ElseDirectiveTrivia,
                    SyntaxKind.ElifDirectiveTrivia,
                    SyntaxKind.EndIfDirectiveTrivia,
                    SyntaxKind.RegionDirectiveTrivia,
                    SyntaxKind.EndRegionDirectiveTrivia))
            {
                List<DirectiveTriviaSyntax> directives = directiveTrivia.GetRelatedDirectives();

                if (directives.Count > 1)
                {
                    string title = "Remove directive and related directive";

                    if (directives.Count > 2)
                        title += "s";

                    context.RegisterRefactoring(
                        title,
                        cancellationToken =>
                        {
                            return context.Document.RemovePreprocessorDirectivesAsync(
                                directives.ToImmutableArray(),
                                cancellationToken);
                        });
                }
            }
        }
    }
}