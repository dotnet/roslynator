// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
{
#if DEBUG
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SyntaxTreeDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(DiagnosticDescriptors.UseSpacesInsteadOfTab);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxTreeAction(f => AnalyzeSyntaxTree(f));
        }

        private void AnalyzeSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            SyntaxNode root;
            if (!context.Tree.TryGetRoot(out root))
                return;

            foreach (SyntaxTrivia trivia in root.DescendantTrivia())
            {
                if (trivia.IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    string s = trivia.ToString();

                    for (int i = 0; i < s.Length; i++)
                    {
                        if (s[i] == '\t')
                        {
                            int index = i;

                            do
                            {
                                i++;
                            } while (i < s.Length && s[i] == '\t');

                            context.ReportDiagnostic(
                                DiagnosticDescriptors.UseSpacesInsteadOfTab,
                                Location.Create(context.Tree, new TextSpan(trivia.SpanStart + index, i - index)));
                        }
                    }
                }
            }
        }
    }
#endif
}
