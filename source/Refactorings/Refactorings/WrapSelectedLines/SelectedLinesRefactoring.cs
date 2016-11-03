// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings.WrapSelectedLines
{
    internal abstract class SelectedLinesRefactoring
    {
        public abstract ImmutableArray<TextChange> GetTextChanges(IEnumerable<TextLine> selectedLines);

        public static async Task<SelectedLinesInfo> GetSelectedLinesInfoAsync(RefactoringContext context, SyntaxNode node)
        {
            TextSpan span = context.Span;

            if (IsValidSpan(context.Root, span))
            {
                SourceText sourceText = await context.Document.GetTextAsync(context.CancellationToken).ConfigureAwait(false);

                return new SelectedLinesInfo(sourceText.Lines, span);
            }

            return null;
        }

        private static bool IsValidSpan(SyntaxNode root, TextSpan span)
        {
            if (!span.IsEmpty)
            {
                int start = span.Start;

                if (start == 0
                    || root
                        .FindTrivia(start - 1, findInsideTrivia: true)
                        .IsKind(SyntaxKind.EndOfLineTrivia)
                    || root
                        .FindToken(start - 1, findInsideTrivia: true)
                        .IsKind(SyntaxKind.XmlTextLiteralNewLineToken))
                {
                    if (!root.FindTrivia(span.End).IsKind(SyntaxKind.MultiLineCommentTrivia))
                        return true;
                }
            }

            return false;
        }

        public async Task<Document> RefactorAsync(
            Document document,
            SelectedLinesInfo info,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SourceText sourceText = await document.GetTextAsync(cancellationToken).ConfigureAwait(false);

            ImmutableArray<TextChange> textChanges = GetTextChanges(info.SelectedLines());

            SourceText newSourceText = sourceText.WithChanges(textChanges);

            return document.WithText(newSourceText);
        }
    }
}
