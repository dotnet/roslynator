// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class TabWhiteSpaceDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.AvoidUsageOfTab); }
        }

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

            foreach (SyntaxTrivia trivia in root.DescendantTrivia(descendIntoTrivia: true))
            {
                if (trivia.IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    string text = trivia.ToString();

                    for (int i = 0; i < text.Length; i++)
                    {
                        if (text[i] == '\t')
                        {
                            int index = i;

                            do
                            {
                                i++;

                            } while (i < text.Length && text[i] == '\t');

                            context.ReportDiagnostic(
                                DiagnosticDescriptors.AvoidUsageOfTab,
                                Location.Create(context.Tree, new TextSpan(trivia.SpanStart + index, i - index)));
                        }
                    }
                }
            }
        }
    }
}
