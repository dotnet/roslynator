// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class WhitespaceAnalyzer : BaseDiagnosticAnalyzer
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

            base.Initialize(context);

            context.RegisterSyntaxTreeAction(AnalyzeTrailingTrivia);
        }

        private static void AnalyzeTrailingTrivia(SyntaxTreeAnalysisContext context)
        {
            if (!context.Tree.TryGetText(out SourceText sourceText))
                return;

            if (!context.Tree.TryGetRoot(out SyntaxNode root))
                return;

            var emptyLines = default(TextSpan);
            bool previousLineIsEmpty = false;
            int i = 0;

            foreach (TextLine textLine in sourceText.Lines)
            {
                bool lineIsEmpty = false;

                if (textLine.Span.Length == 0)
                {
                    SyntaxTrivia endOfLine = root.FindTrivia(textLine.End);

                    if (endOfLine.IsEndOfLineTrivia())
                    {
                        lineIsEmpty = true;

                        if (previousLineIsEmpty)
                        {
                            if (emptyLines.IsEmpty)
                            {
                                emptyLines = endOfLine.Span;
                            }
                            else
                            {
                                emptyLines = TextSpan.FromBounds(emptyLines.Start, endOfLine.Span.End);
                            }
                        }
                    }
                    else
                    {
                        emptyLines = default(TextSpan);
                    }
                }
                else
                {
                    if (!emptyLines.IsEmpty)
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.RemoveRedundantEmptyLine,
                            Location.Create(context.Tree, emptyLines));
                    }

                    emptyLines = default(TextSpan);

                    int end = textLine.End - 1;

                    if (char.IsWhiteSpace(sourceText[end]))
                    {
                        int start = end;

                        while (start > textLine.Span.Start && char.IsWhiteSpace(sourceText[start - 1]))
                            start--;

                        TextSpan whitespace = TextSpan.FromBounds(start, end + 1);

                        if (root.FindTrivia(start).IsWhitespaceTrivia())
                        {
                            if (previousLineIsEmpty && start == textLine.Start)
                            {
                                whitespace = TextSpan.FromBounds(
                                    sourceText.Lines[i - 1].End,
                                    whitespace.End);
                            }

                            context.ReportDiagnostic(
                                DiagnosticDescriptors.RemoveTrailingWhitespace,
                                Location.Create(context.Tree, whitespace));
                        }
                    }
                }

                previousLineIsEmpty = lineIsEmpty;
                i++;
            }
        }
    }
}
