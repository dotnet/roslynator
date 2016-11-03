// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseLinefeedAsNewLineDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.UseLinefeedAsNewLine); }
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

            SourceText sourceText;
            if (!context.Tree.TryGetText(out sourceText))
                return;

            SyntaxNode root;
            if (!context.Tree.TryGetRoot(out root))
                return;

            foreach (TextLine textLine in sourceText.Lines)
            {
                int end = textLine.End;

                if (textLine.EndIncludingLineBreak - end == 2
                    && textLine.Text[end] == '\r'
                    && textLine.Text[end + 1] == '\n')
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.UseLinefeedAsNewLine,
                        Location.Create(context.Tree, new TextSpan(end, 2)));
                }
            }
        }
    }
}
