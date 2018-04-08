// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    //XPERF:
    internal static class RemoveEmptyLinesRefactoring
    {
        public static async Task<bool> CanRefactorAsync(RefactoringContext context, SyntaxNode node)
        {
            if (!node
                .DescendantTrivia(context.Span, descendIntoTrivia: true)
                .Any(f => f.IsEndOfLineTrivia()))
            {
                return false;
            }

            SourceText sourceText = await context.Document.GetTextAsync(context.CancellationToken).ConfigureAwait(false);

            return GetEmptyLines(sourceText, context.Root, context.Span).Any();
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            TextSpan span,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            IEnumerable<TextChange> textChanges = GetEmptyLines(sourceText, root, span)
                .Select(line => new TextChange(line.SpanIncludingLineBreak, ""));

            SourceText newSourceText = sourceText.WithChanges(textChanges);

            return document.WithText(newSourceText);
        }

        private static IEnumerable<TextLine> GetEmptyLines(SourceText sourceText, SyntaxNode root, TextSpan span)
        {
            foreach (TextLine line in sourceText
                .Lines
                .SkipWhile(f => f.Start < span.Start)
                .TakeWhile(f => f.EndIncludingLineBreak <= span.End))
            {
                if (line.Span.Length == 0
                    || StringUtility.IsWhitespace(line.ToString()))
                {
                    SyntaxTrivia endOfLine = root.FindTrivia(line.End, findInsideTrivia: true);

                    if (endOfLine.IsEndOfLineTrivia())
                        yield return line;
                }
            }
        }
    }
}
