// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings.WrapSelectedLines
{
    internal abstract class SelectedLinesRefactoring
    {
        public abstract ImmutableArray<TextChange> GetTextChanges(IEnumerable<TextLine> selectedLines);

        public static async Task<bool> CanRefactorAsync(RefactoringContext context, SyntaxNode node)
        {
            if (node
                .DescendantTrivia(context.Span, descendIntoTrivia: true)
                .Any(f => f.IsKind(SyntaxKind.EndOfLineTrivia)))
            {
                SourceText sourceText = await context.Document.GetTextAsync(context.CancellationToken).ConfigureAwait(false);

                if (GetSelectedLines(sourceText, context.Span).Any())
                    return true;
            }

            return false;
        }

        public async Task<Document> RefactorAsync(
            Document document,
            SyntaxNode node,
            TextSpan span,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<TextChange> textChanges = GetTextChanges(GetSelectedLines(sourceText, span));

            SourceText newSourceText = sourceText.WithChanges(textChanges);

            return document.WithText(newSourceText);
        }

        private static IEnumerable<TextLine> GetSelectedLines(SourceText sourceText, TextSpan span)
        {
            foreach (TextLine line in sourceText
                .Lines
                .SkipWhile(f => span.Start > f.Start)
                .TakeWhile(f => span.End >= f.EndIncludingLineBreak))
            {
                yield return line;
            }
        }
    }
}
