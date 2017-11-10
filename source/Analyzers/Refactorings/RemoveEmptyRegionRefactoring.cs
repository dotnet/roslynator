// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveEmptyRegionRefactoring
    {
        public static void AnalyzeRegionDirective(SyntaxNodeAnalysisContext context)
        {
            var region = (RegionDirectiveTriviaSyntax)context.Node;

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

                        if (trivia.TryGetContainingList(out SyntaxTriviaList list))
                        {
                            EndRegionDirectiveTriviaSyntax endRegion2 = FindEndRegion(list, list.IndexOf(trivia));

                            if (endRegion == endRegion2)
                            {
                                context.ReportDiagnostic(
                                    DiagnosticDescriptors.RemoveEmptyRegion,
                                    region.GetLocation(),
                                    additionalLocations: ImmutableArray.Create(endRegion.GetLocation()));

                                context.ReportDiagnostic(DiagnosticDescriptors.RemoveEmptyRegionFadeOut, region.GetLocation());
                                context.ReportDiagnostic(DiagnosticDescriptors.RemoveEmptyRegionFadeOut, endRegion.GetLocation());
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

        public static Task<Document> RefactorAsync(
            Document document,
            RegionDirectiveTriviaSyntax regionDirective,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return document.RemoveRegionAsync(regionDirective, cancellationToken);
        }
    }
}
