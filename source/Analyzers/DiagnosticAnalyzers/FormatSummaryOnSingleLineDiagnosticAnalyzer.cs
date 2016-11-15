// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Refactorings.FormatSummary;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class FormatSummaryOnSingleLineDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.FormatDocumentationSummaryOnSingleLine); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeDocumentationComment(f), SyntaxKind.SingleLineDocumentationCommentTrivia);
        }

        private void AnalyzeDocumentationComment(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var documentationComment = (DocumentationCommentTriviaSyntax)context.Node;

            XmlElementSyntax summaryElement = FormatSummaryRefactoring.GetSummaryElement(documentationComment);

            if (summaryElement != null)
            {
                XmlElementStartTagSyntax startTag = summaryElement?.StartTag;

                if (startTag?.IsMissing == false)
                {
                    XmlElementEndTagSyntax endTag = summaryElement.EndTag;

                    if (endTag?.IsMissing == false
                        && startTag.GetSpanEndLine() < endTag.GetSpanStartLine())
                    {
                        Match match = FormatSummaryRefactoring.Regex.Match(
                            summaryElement.ToString(),
                            startTag.Span.End - summaryElement.Span.Start,
                            endTag.Span.Start - startTag.Span.End);

                        if (match.Success)
                        {
                            context.ReportDiagnostic(
                                DiagnosticDescriptors.FormatDocumentationSummaryOnSingleLine,
                                summaryElement.GetLocation());
                        }
                    }
                }
            }
        }
    }
}
