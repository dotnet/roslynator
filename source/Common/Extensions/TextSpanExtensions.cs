// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis
{
    public static class TextSpanExtensions
    {
        public static bool IsBetweenSpans(this TextSpan span, SyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            TextSpan nodeSpan = node.Span;
            TextSpan nodeFullSpan = node.FullSpan;

            return span.Start >= nodeFullSpan.Start
                && span.Start <= nodeSpan.Start
                && span.End >= nodeSpan.End
                && span.End <= nodeFullSpan.End;
        }

        public static bool IsBetweenSpans<TNode>(this TextSpan span, SyntaxList<TNode> list) where TNode : SyntaxNode
        {
            TextSpan nodeSpan = list.Span;
            TextSpan nodeFullSpan = list.FullSpan;

            return span.Start >= nodeFullSpan.Start
                && span.Start <= nodeSpan.Start
                && span.End >= nodeSpan.End
                && span.End <= nodeFullSpan.End;
        }

        public static bool IsBetweenSpans<TNode>(this TextSpan span, SeparatedSyntaxList<TNode> list) where TNode : SyntaxNode
        {
            TextSpan nodeSpan = list.Span;
            TextSpan nodeFullSpan = list.FullSpan;

            return span.Start >= nodeFullSpan.Start
                && span.Start <= nodeSpan.Start
                && span.End >= nodeSpan.End
                && span.End <= nodeFullSpan.End;
        }

        public static bool IsBetweenSpans(this TextSpan span, SyntaxToken token)
        {
            TextSpan nodeSpan = token.Span;
            TextSpan nodeFullSpan = token.FullSpan;

            return span.Start >= nodeFullSpan.Start
                && span.Start <= nodeSpan.Start
                && span.End >= nodeSpan.End
                && span.End <= nodeFullSpan.End;
        }

        public static bool IsEmptyOrBetweenSpans(this TextSpan span, SyntaxNode node)
        {
            return span.IsEmpty || IsBetweenSpans(span, node);
        }

        public static bool IsEmptyOrBetweenSpans<TNode>(this TextSpan span, SyntaxList<TNode> node) where TNode : SyntaxNode
        {
            return span.IsEmpty || IsBetweenSpans(span, node);
        }

        public static bool IsEmptyOrBetweenSpans<TNode>(this TextSpan span, SeparatedSyntaxList<TNode> node) where TNode : SyntaxNode
        {
            return span.IsEmpty || IsBetweenSpans(span, node);
        }

        public static bool IsEmptyOrBetweenSpans(this TextSpan span, SyntaxToken token)
        {
            return span.IsEmpty || IsBetweenSpans(span, token);
        }
    }
}
