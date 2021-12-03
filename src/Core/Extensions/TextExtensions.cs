// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator
{
    internal static class TextExtensions
    {
        public static void Add(this IList<TextChange> items, TextSpan span, string newText)
        {
            items.Add(new TextChange(span, newText));
        }

        public static void Add(this ImmutableArray<TextChange> items, TextSpan span, string newText)
        {
            items.Add(new TextChange(span, newText));
        }

        public static SourceText WithChange(this SourceText sourceText, TextSpan span, string newText)
        {
            return sourceText.WithChanges(new TextChange(span, newText));
        }

        public static int GetLineCount(this TextLineCollection textLines, TextSpan span)
        {
            return textLines.GetLinePositionSpan(span).GetLineCount();
        }

        public static bool IsEmptyOrWhiteSpace(this TextLine textLine)
        {
            return IsEmptyOrWhiteSpace(textLine, textLine.Span);
        }

        public static bool IsEmptyOrWhiteSpace(this TextLine textLine, TextSpan span)
        {
            for (int i = span.Start; i < span.End; i++)
            {
                if (!char.IsWhiteSpace(textLine.Text[i]))
                    return false;
            }

            return true;
        }

        public static int GetLineCount(this LinePositionSpan linePositionSpan)
        {
            return linePositionSpan.End.Line - linePositionSpan.Start.Line + 1;
        }
    }
}
