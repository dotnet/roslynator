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
            SyntaxNode root = context.Root;
            TextSpan span = context.Span;

            if (IsValidSpan(root, span))
            {
                SourceText sourceText = await context.Document.GetTextAsync(context.CancellationToken).ConfigureAwait(false);

                if (IsFullLineSelection(sourceText, span))
                    return true;
            }

            return false;
        }

        private static bool IsValidSpan(SyntaxNode root, TextSpan span)
        {
            if (!span.IsEmpty)
            {
                int start = span.Start;
                int end = span.End;

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
            return sourceText
                .Lines
                .SkipWhile(f => span.Start > f.Start)
                .TakeWhile(f => span.End >= f.End);
        }

        private static bool IsFullLineSelection(SourceText sourceText, TextSpan span)
        {
            return IsFullLineSelection(sourceText, span.Start, span.End);
        }

        private static bool IsFullLineSelection(SourceText sourceText, int start, int end)
        {
            using (TextLineCollection.Enumerator en = sourceText.Lines.GetEnumerator())
            {
                while (en.MoveNext())
                {
                    if (start > en.Current.Start)
                    {
                        continue;
                    }
                    else if (start == en.Current.Start)
                    {
                        do
                        {
                            if (end < en.Current.End)
                            {
                                return false;
                            }
                            else if (end == en.Current.End
                                || end == en.Current.EndIncludingLineBreak)
                            {
                                return true;
                            }

                        } while (en.MoveNext());
                    }
                    else
                    {
                        return false;
                    }
                }

                return false;
            }
        }
    }
}
