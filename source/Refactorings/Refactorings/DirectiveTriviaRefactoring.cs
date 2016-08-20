// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings
{
    internal static class DirectiveTriviaRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, DirectiveTriviaSyntax directiveTrivia)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemovePreprocessorDirectiveAndRelatedDirectives)
                && directiveTrivia.IsKind(
                    SyntaxKind.IfDirectiveTrivia,
                    SyntaxKind.ElseDirectiveTrivia,
                    SyntaxKind.ElifDirectiveTrivia,
                    SyntaxKind.EndIfDirectiveTrivia,
                    SyntaxKind.RegionDirectiveTrivia,
                    SyntaxKind.EndRegionDirectiveTrivia))
            {
                List<DirectiveTriviaSyntax> directives = directiveTrivia
                    .GetRelatedDirectives();

                if (directives.Count > 1)
                {
                    string title = "Remove directive and related directive";

                    if (directives.Count > 2)
                        title += "s";

                    context.RegisterRefactoring(
                        title,
                        cancellationToken =>
                        {
                            return RefactorAsync(
                                context.Document,
                                directives.ToImmutableArray(),
                                cancellationToken);
                        });
                }
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ImmutableArray<DirectiveTriviaSyntax> directives,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxNode newRoot = SyntaxRemover.RemoveDirectiveTrivia(root, directives);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}