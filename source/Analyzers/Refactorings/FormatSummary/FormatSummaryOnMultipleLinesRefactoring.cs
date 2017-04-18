// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings.FormatSummary
{
    internal static class FormatSummaryOnMultipleLinesRefactoring
    {
        public static void AnalyzeSingleLineDocumentationCommentTrivia(SyntaxNodeAnalysisContext context)
        {
            var documentationComment = (DocumentationCommentTriviaSyntax)context.Node;

            XmlElementSyntax summaryElement = documentationComment.SummaryElement();

            if (summaryElement?.StartTag?.IsMissing == false
                && summaryElement.EndTag?.IsMissing == false
                && summaryElement.IsSingleLine(includeExteriorTrivia: false, trim: false))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.FormatDocumentationSummaryOnMultipleLines,
                    summaryElement);
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            DocumentationCommentTriviaSyntax documentationComment,
            CancellationToken cancellationToken)
        {
            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            int lineIndex = documentationComment.GetSpanStartLine(cancellationToken);

            XmlElementSyntax summaryElement = documentationComment.SummaryElement();

            string newText = Environment.NewLine
                + new string(' ', documentationComment.FullSpan.Start - sourceText.Lines[lineIndex].Start)
                + "/// ";

            SourceText newSourceText = sourceText.WithChanges(
                new TextChange(new TextSpan(summaryElement.StartTag.Span.End, 0), newText),
                new TextChange(new TextSpan(summaryElement.EndTag.Span.Start, 0), newText));

            return document.WithText(newSourceText);
        }
    }
}