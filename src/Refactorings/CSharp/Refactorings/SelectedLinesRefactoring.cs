// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Refactorings.WrapSelectedLines;
using Roslynator.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal abstract class SelectedLinesRefactoring
    {
        public abstract ImmutableArray<TextChange> GetTextChanges(IEnumerable<TextLine> selectedLines);

        public Task<Document> RefactorAsync(
            Document document,
            TextLineCollectionSelection selectedLines,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ImmutableArray<TextChange> textChanges = GetTextChanges(selectedLines);

            return document.WithTextChangesAsync(textChanges, cancellationToken);
        }

        public static async Task ComputeRefactoringsAsync(RefactoringContext context, SyntaxNode node)
        {
            if (context.IsAnyRefactoringEnabled(
                RefactoringDescriptors.WrapLinesInRegion,
                RefactoringDescriptors.WrapLinesInPreprocessorDirective,
                RefactoringDescriptors.RemoveEmptyLines))
            {
                SyntaxNode root = context.Root;
                TextSpan span = context.Span;

                if (!IsFullLineSpan(node, span))
                    return;

                Document document = context.Document;
                SourceText sourceText = await document.GetTextAsync(context.CancellationToken).ConfigureAwait(false);

                if (!TextLineCollectionSelection.TryCreate(sourceText.Lines, span, out TextLineCollectionSelection selectedLines))
                    return;

                if (!IsInMultiLineDocumentationComment(root, span.Start)
                    && !IsInMultiLineDocumentationComment(root, span.End))
                {
                    if (context.IsRefactoringEnabled(RefactoringDescriptors.WrapLinesInRegion))
                    {
                        context.RegisterRefactoring(
                            "Wrap in #region",
                            ct => WrapLinesInRegionRefactoring.Instance.RefactorAsync(document, selectedLines, ct),
                            RefactoringDescriptors.WrapLinesInRegion);
                    }

                    if (context.IsRefactoringEnabled(RefactoringDescriptors.WrapLinesInPreprocessorDirective))
                    {
                        context.RegisterRefactoring(
                            "Wrap in #if",
                            ct => WrapLinesInPreprocessorDirectiveRefactoring.Instance.RefactorAsync(document, selectedLines, ct),
                            RefactoringDescriptors.WrapLinesInPreprocessorDirective);
                    }
                }

                if (context.IsRefactoringEnabled(RefactoringDescriptors.RemoveEmptyLines))
                {
                    var containsEmptyLine = false;

                    foreach (TextLine line in selectedLines)
                    {
                        context.ThrowIfCancellationRequested();

                        if (line.IsEmptyOrWhiteSpace()
                            && root.FindTrivia(line.End, findInsideTrivia: true).IsEndOfLineTrivia())
                        {
                            containsEmptyLine = true;
                            break;
                        }
                    }

                    if (containsEmptyLine)
                    {
                        context.RegisterRefactoring(
                            "Remove empty lines",
                            ct =>
                            {
                                ct.ThrowIfCancellationRequested();

                                IEnumerable<TextChange> textChanges = selectedLines
                                    .Where(line => line.IsEmptyOrWhiteSpace() && root.FindTrivia(line.End, findInsideTrivia: true).IsEndOfLineTrivia())
                                    .Select(line => new TextChange(line.SpanIncludingLineBreak, ""));

                                return document.WithTextChangesAsync(textChanges, ct);
                            },
                            RefactoringDescriptors.RemoveEmptyLines);
                    }
                }
            }
        }

        private static bool IsFullLineSpan(SyntaxNode node, TextSpan span)
        {
            if (!node.FullSpan.Contains(span))
                throw new ArgumentOutOfRangeException(nameof(span), span, "");

            if (!span.IsEmpty)
            {
                if (span.Start == 0
                    || IsStartOrEndOfLine(span.Start, compareWithStart: false, offset: -1))
                {
                    return IsStartOrEndOfLine(span.End, compareWithStart: false, offset: -1)
                        || IsStartOrEndOfLine(span.End, compareWithStart: true);
                }
            }

            return false;

            bool IsStartOrEndOfLine(int position, bool compareWithStart, int offset = 0)
            {
                int positionWithOffset = position + offset;

                SyntaxNode n = node;

                while (!n.FullSpan.Contains(positionWithOffset))
                {
                    n = n.Parent;

                    if (n == null)
                        return false;
                }

                SyntaxTrivia trivia = n.FindTrivia(positionWithOffset);

                if (trivia.IsKind(SyntaxKind.EndOfLineTrivia)
                    && position == ((compareWithStart) ? trivia.Span.Start : trivia.Span.End))
                {
                    return true;
                }

                SyntaxToken token = n.FindToken(positionWithOffset, findInsideTrivia: true);

                return token.IsKind(SyntaxKind.XmlTextLiteralNewLineToken)
                    && position == ((compareWithStart) ? token.Span.Start : token.Span.End);
            }
        }

        private static bool IsInMultiLineDocumentationComment(SyntaxNode root, int position)
        {
            SyntaxToken token = root.FindToken(position, findInsideTrivia: true);

            for (SyntaxNode node = token.Parent; node != null; node = node.Parent)
            {
                if (node.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia))
                    return true;
            }

            return false;
        }
    }
}
