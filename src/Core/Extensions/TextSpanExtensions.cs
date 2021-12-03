// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator
{
    internal static class TextSpanExtensions
    {
        public static TextSpan TrimFromStart(this TextSpan span, int length)
        {
            return new TextSpan(span.Start + length, span.Length - length);
        }

        public static TextSpan Offset(this TextSpan span, int value)
        {
            return new TextSpan(span.Start + value, span.Length);
        }

        public static TextSpan WithLength(this TextSpan span, int length)
        {
            return new TextSpan(span.Start, length);
        }

        public static bool IsContainedIn(this TextSpan self, TextSpan span)
        {
            return span.Contains(self);
        }

        public static bool IsContainedInSpan(this TextSpan span, SyntaxToken token1, SyntaxToken token2)
        {
            return token1.Span.Contains(span) || token2.Span.Contains(span);
        }

        public static bool IsBetweenSpans(this TextSpan span, SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return span.IsBetweenSpans(node.Span, node.FullSpan);
        }

        public static bool IsBetweenSpans<TNode>(this TextSpan span, SyntaxList<TNode> list) where TNode : SyntaxNode
        {
            return span.IsBetweenSpans(list.Span, list.FullSpan);
        }

        public static bool IsBetweenSpans<TNode>(this TextSpan span, SeparatedSyntaxList<TNode> list) where TNode : SyntaxNode
        {
            return span.IsBetweenSpans(list.Span, list.FullSpan);
        }

        public static bool IsBetweenSpans(this TextSpan span, SyntaxToken token)
        {
            return span.IsBetweenSpans(token.Span, token.FullSpan);
        }

        private static bool IsBetweenSpans(this TextSpan span, TextSpan innerSpan, TextSpan outerSpan)
        {
            return span.Start >= outerSpan.Start
                && span.Start <= innerSpan.Start
                && span.End >= innerSpan.End
                && span.End <= outerSpan.End;
        }

        public static bool IsContainedInSpanOrBetweenSpans(this TextSpan span, SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            TextSpan innerSpan = node.Span;

            return innerSpan.Contains(span) || span.IsBetweenSpans(innerSpan, node.FullSpan);
        }

        public static bool IsContainedInSpanOrBetweenSpans<TNode>(this TextSpan span, SyntaxList<TNode> list) where TNode : SyntaxNode
        {
            TextSpan innerSpan = list.Span;

            return innerSpan.Contains(span) || span.IsBetweenSpans(innerSpan, list.FullSpan);
        }

        public static bool IsContainedInSpanOrBetweenSpans<TNode>(this TextSpan span, SeparatedSyntaxList<TNode> list) where TNode : SyntaxNode
        {
            TextSpan innerSpan = list.Span;

            return innerSpan.Contains(span) || span.IsBetweenSpans(innerSpan, list.FullSpan);
        }

        public static bool IsContainedInSpanOrBetweenSpans(this TextSpan span, SyntaxToken token)
        {
            TextSpan innerSpan = token.Span;

            return innerSpan.Contains(span) || span.IsBetweenSpans(innerSpan, token.FullSpan);
        }

        public static bool IsEmptyAndContainedInSpanOrBetweenSpans(this TextSpan span, SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return IsEmptyAndContainedInSpanOrBetweenSpans(span, node.Span, node.FullSpan);
        }

        public static bool IsEmptyAndContainedInSpanOrBetweenSpans<TNode>(this TextSpan span, SyntaxList<TNode> list) where TNode : SyntaxNode
        {
            return IsEmptyAndContainedInSpanOrBetweenSpans(span, list.Span, list.FullSpan);
        }

        public static bool IsEmptyAndContainedInSpanOrBetweenSpans<TNode>(this TextSpan span, SeparatedSyntaxList<TNode> list) where TNode : SyntaxNode
        {
            return IsEmptyAndContainedInSpanOrBetweenSpans(span, list.Span, list.FullSpan);
        }

        public static bool IsEmptyAndContainedInSpanOrBetweenSpans(this TextSpan span, SyntaxToken token)
        {
            return IsEmptyAndContainedInSpanOrBetweenSpans(span, token.Span, token.FullSpan);
        }

        private static bool IsEmptyAndContainedInSpanOrBetweenSpans(this TextSpan span, TextSpan innerSpan, TextSpan outerSpan)
        {
            if (span.IsEmpty)
            {
                return innerSpan.Contains(span);
            }
            else
            {
                return span.IsBetweenSpans(innerSpan, outerSpan);
            }
        }

        public static bool IsEmptyAndContainedInSpan(this TextSpan span, SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return span.IsEmpty
                && node.Span.Contains(span);
        }

        public static bool IsEmptyAndContainedInSpan<TNode>(this TextSpan span, SyntaxList<TNode> list) where TNode : SyntaxNode
        {
            return span.IsEmpty
                && list.Span.Contains(span);
        }

        public static bool IsEmptyAndContainedInSpan<TNode>(this TextSpan span, SeparatedSyntaxList<TNode> list) where TNode : SyntaxNode
        {
            return span.IsEmpty
                && list.Span.Contains(span);
        }

        public static bool IsEmptyAndContainedInSpan(this TextSpan span, SyntaxToken token)
        {
            return span.IsEmpty
                && token.Span.Contains(span);
        }

        public static bool IsEmptyAndContainedInSpan(this TextSpan span, SyntaxToken token1, SyntaxToken token2)
        {
            return IsEmptyAndContainedInSpan(span, token1)
                || IsEmptyAndContainedInSpan(span, token2);
        }

        public static bool IsSingleLine(
            this TextSpan span,
            SyntaxTree syntaxTree,
            CancellationToken cancellationToken = default)
        {
            return syntaxTree.GetLineSpan(span, cancellationToken).IsSingleLine();
        }

        public static bool IsMultiLine(
            this TextSpan span,
            SyntaxTree syntaxTree,
            CancellationToken cancellationToken = default)
        {
            return syntaxTree.GetLineSpan(span, cancellationToken).IsMultiLine();
        }
    }
}
