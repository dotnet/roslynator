// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings.FormatSummary
{
    internal static class FormatSummaryOnSingleLineRefactoring
    {
        private static readonly Regex _regex = new Regex(
            @"
            ^
            (
                [\s-[\r\n]]*
                \r?\n
                [\s-[\r\n]]*
                ///
                [\s-[\r\n]]*
            )?
            (?<1>[^\r\n]*)
            (
                [\s-[\r\n]]*
                \r?\n
                [\s-[\r\n]]*
                ///
                [\s-[\r\n]]*
            )?
            $
            ", RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture);

        public static void AnalyzeSingleLineDocumentationCommentTrivia(SyntaxNodeAnalysisContext context)
        {
            var documentationComment = (DocumentationCommentTriviaSyntax)context.Node;

            XmlElementSyntax summaryElement = documentationComment.SummaryElement();

            if (summaryElement != null)
            {
                XmlElementStartTagSyntax startTag = summaryElement?.StartTag;

                if (startTag?.IsMissing == false)
                {
                    XmlElementEndTagSyntax endTag = summaryElement.EndTag;

                    if (endTag?.IsMissing == false
                        && startTag.GetSpanEndLine() < endTag.GetSpanStartLine())
                    {
                        Match match = _regex.Match(
                            summaryElement.ToString(),
                            startTag.Span.End - summaryElement.Span.Start,
                            endTag.Span.Start - startTag.Span.End);

                        if (match.Success)
                        {
                            context.ReportDiagnostic(
                                DiagnosticDescriptors.FormatDocumentationSummaryOnSingleLine,
                                summaryElement);
                        }
                    }
                }
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            DocumentationCommentTriviaSyntax documentationComment,
            CancellationToken cancellationToken)
        {
            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            XmlElementSyntax summaryElement = documentationComment.SummaryElement();

            XmlElementStartTagSyntax startTag = summaryElement.StartTag;
            XmlElementEndTagSyntax endTag = summaryElement.EndTag;

            Match match = _regex.Match(
                summaryElement.ToString(),
                startTag.Span.End - summaryElement.Span.Start,
                endTag.Span.Start - startTag.Span.End);

            var textChange = new TextChange(
                new TextSpan(startTag.Span.End, match.Length),
                match.Groups[1].Value);

            SourceText newSourceText = sourceText.WithChanges(textChange);

            return document.WithText(newSourceText);
        }
    }
}