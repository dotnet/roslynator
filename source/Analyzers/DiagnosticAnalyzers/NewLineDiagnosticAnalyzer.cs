// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
{
#if DEBUG
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class NewLineDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.UseLinefeedAsNewLine,
                    DiagnosticDescriptors.UseCarriageReturnAndLinefeedAsNewLine);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxTreeAction(f => AnalyzeTrailingTrivia(f));
        }

        private void AnalyzeTrailingTrivia(SyntaxTreeAnalysisContext context)
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
                switch (textLine.EndIncludingLineBreak - textLine.End)
                {
                    case 1:
                        {
                            if (sourceText[textLine.End] == '\n')
                            {
                                context.ReportDiagnostic(
                                    DiagnosticDescriptors.UseCarriageReturnAndLinefeedAsNewLine,
                                    Location.Create(context.Tree, new TextSpan(textLine.End, 1)));
                            }

                            break;
                        }
                    case 2:
                        {
                            if (sourceText[textLine.End] == '\r'
                                && sourceText[textLine.End + 1] == '\n')
                            {
                                context.ReportDiagnostic(
                                    DiagnosticDescriptors.UseLinefeedAsNewLine,
                                    Location.Create(context.Tree, new TextSpan(textLine.End, 2)));
                            }

                            break;
                        }
                }
            }
        }
    }
#endif
}
