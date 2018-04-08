// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class FormatSummaryOnMultipleLinesRefactoring
    {
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
                new TextChange(new TextSpan(summaryElement.EndTag.SpanStart, 0), newText));

            return document.WithText(newSourceText);
        }
    }
}