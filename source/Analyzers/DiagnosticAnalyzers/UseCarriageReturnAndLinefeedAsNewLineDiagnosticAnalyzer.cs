// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseCarriageReturnAndLinefeedAsNewLineDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.UseCarriageReturnAndLinefeedAsNewLine); }
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

                if (textLine.EndIncludingLineBreak - end == 1
                    && textLine.Text[end] == '\n')
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.UseCarriageReturnAndLinefeedAsNewLine,
                        Location.Create(context.Tree, new TextSpan(end, 1)));
                }
            }
        }
    }
}
