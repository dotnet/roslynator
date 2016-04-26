// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class WhitespaceDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.RemoveTrailingWhitespace,
                    DiagnosticDescriptors.RemoveRedundantEmptyLine);
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

            bool previousLineIsEmpty = false;

            foreach (TextLine textLine in sourceText.Lines)
            {
                bool lineIsEmpty = false;

                if (textLine.Span.Length == 0)
                {
                    SyntaxTrivia endOfLine = root.FindTrivia(textLine.End);

                    if (endOfLine.IsKind(SyntaxKind.EndOfLineTrivia))
                        lineIsEmpty = true;

                    if (previousLineIsEmpty && lineIsEmpty)
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.RemoveRedundantEmptyLine,
                            endOfLine.GetLocation());
                    }
                }

                previousLineIsEmpty = lineIsEmpty;

                if (textLine.Span.Length == 0)
                    continue;

                int end = textLine.End - 1;

                if (!char.IsWhiteSpace(sourceText[end]))
                    continue;

                int start = end;

                while (start > textLine.Span.Start && char.IsWhiteSpace(sourceText[start - 1]))
                    start--;

                TextSpan span = TextSpan.FromBounds(start, end + 1);

                if (root.FindTrivia(start).IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.RemoveTrailingWhitespace,
                        Location.Create(context.Tree, span));
                }
            }
        }
    }
}
