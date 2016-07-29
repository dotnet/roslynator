// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class DirectiveTriviaRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, DirectiveTriviaSyntax directiveTrivia)
        {
            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.RemovePreprocessorDirectiveAndRelatedDirectives)
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
                                directives,
                                cancellationToken);
                        });
                }
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            List<DirectiveTriviaSyntax> directives,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            var rewriter = new DirectiveTriviaRemover(directives
                .Select(f => f.ParentTrivia)
                .ToImmutableArray());

            root = rewriter.Visit(root);

            return document.WithSyntaxRoot(root);
        }

        private class DirectiveTriviaRemover : CSharpSyntaxRewriter
        {
            private readonly ImmutableArray<SyntaxTrivia> _trivias;

            public DirectiveTriviaRemover(ImmutableArray<SyntaxTrivia> trivia)
                : base(visitIntoStructuredTrivia: true)
            {
                _trivias = trivia;
            }

            public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
            {
                if (_trivias.Contains(trivia))
                    return CSharpFactory.NewLine;

                return base.VisitTrivia(trivia);
            }
        }
    }
}