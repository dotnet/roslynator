// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RegionDirectiveTriviaDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.RemoveEmptyRegion); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeRegionDirectiveTrivia(f), SyntaxKind.RegionDirectiveTrivia);
        }

        private void AnalyzeRegionDirectiveTrivia(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

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
    }
}
