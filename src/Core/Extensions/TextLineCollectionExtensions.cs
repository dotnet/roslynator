// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.Text;

namespace Roslynator
{
    internal static class TextLineCollectionExtensions
    {
        public static int GetLineCount(this TextLineCollection textLines, TextSpan span)
        {
            return textLines.GetLinePositionSpan(span).GetLineCount();
        }
    }
}