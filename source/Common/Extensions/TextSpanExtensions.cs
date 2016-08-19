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
    }
}
