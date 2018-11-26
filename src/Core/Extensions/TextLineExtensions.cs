// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.Text;

namespace Roslynator
{
    internal static class TextLineExtensions
    {
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
    }
}