// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveEmptyRegionRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, RegionDirectiveTriviaSyntax region)
        {
            if (region.IsKind(SyntaxKind.RegionDirectiveTrivia))
            {
                List<DirectiveTriviaSyntax> relatedDirectives = region.GetRelatedDirectives();

                if (relatedDirectives.Count == 2
                    && relatedDirectives[1].IsKind(SyntaxKind.EndRegionDirectiveTrivia))
                {
                    DirectiveTriviaSyntax endRegion = relatedDirectives[1];

                    if (endRegion.IsKind(SyntaxKind.EndRegionDirectiveTrivia))
                    {
                        SyntaxTrivia trivia = region.ParentTrivia;

                        SyntaxTriviaList list = trivia.GetContainingList();

                        if (list.Any())
                        {
                            EndRegionDirectiveTriviaSyntax endRegion2 = FindEndRegion(list, list.IndexOf(trivia));

                            if (endRegion == endRegion2)
                            {
                                context.ReportDiagnostic(
                                    DiagnosticDescriptors.RemoveEmptyRegion,
                                    region.GetLocation(),
                                    endRegion.GetLocation());
                            }
                        }
                    }
                }
            }
        }

        private static EndRegionDirectiveTriviaSyntax FindEndRegion(SyntaxTriviaList list, int index)
        {
            for (int i = index + 1; i < list.Count; i++)
            {
                SyntaxTrivia trivia = list[i];

                switch (trivia.Kind())
                {
                    case SyntaxKind.WhitespaceTrivia:
                    case SyntaxKind.EndOfLineTrivia:
                        {
                            continue;
                        }
                    case SyntaxKind.EndRegionDirectiveTrivia:
                        {
                            if (trivia.HasStructure)
                                return (EndRegionDirectiveTriviaSyntax)trivia.GetStructure();

                            return null;
                        }
                    default:
                        {
                            return null;
                        }
                }
            }

            return null;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            RegionDirectiveTriviaSyntax regionDirective,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await SyntaxRemover.RemoveRegionAsync(document, regionDirective, cancellationToken).ConfigureAwait(false);
        }
    }
}
