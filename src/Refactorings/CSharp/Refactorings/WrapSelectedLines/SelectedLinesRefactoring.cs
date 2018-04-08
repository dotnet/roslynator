// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Roslynator.Text;

namespace Roslynator.CSharp.Refactorings.WrapSelectedLines
{
    internal abstract class SelectedLinesRefactoring
    {
        public abstract ImmutableArray<TextChange> GetTextChanges(IEnumerable<TextLine> selectedLines);

        public static async Task<TextLineCollectionSelection> GetSelectedLinesAsync(RefactoringContext context)
        {
            if (!IsValidSpan(context.Root, context.Span))
                return null;

            SourceText sourceText = await context.Document.GetTextAsync(context.CancellationToken).ConfigureAwait(false);

            if (!TextLineCollectionSelection.TryCreate(sourceText.Lines, context.Span, out TextLineCollectionSelection selectedLines))
                return null;

            return selectedLines;
        }

        private static bool IsValidSpan(SyntaxNode root, TextSpan span)
        {
            if (!span.IsEmpty)
            {
                int start = span.Start;

                if (start == 0
                    || root.FindTrivia(start - 1, findInsideTrivia: true).IsEndOfLineTrivia()
                    || root.FindToken(start - 1, findInsideTrivia: true).IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    if (!root.FindTrivia(span.End).IsKind(SyntaxKind.MultiLineCommentTrivia))
                        return true;
                }
            }

            return false;
        }

        public Task<Document> RefactorAsync(
            Document document,
            TextLineCollectionSelection selectedLines,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ImmutableArray<TextChange> textChanges = GetTextChanges(selectedLines);

            return document.WithTextChangesAsync(textChanges, cancellationToken);
        }
    }
}
