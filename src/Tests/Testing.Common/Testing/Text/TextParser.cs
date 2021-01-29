// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Testing.Text
{
    internal abstract class TextParser
    {
        public abstract TextParserResult GetSpans(string s, IComparer<LinePositionSpanInfo> comparer = null);

        public abstract (TextSpan span, string text) ReplaceEmptySpan(string s, string replacement);

        public abstract (TextSpan span, string text1, string text2) ReplaceEmptySpan(string s, string replacement1, string replacement2);
    }
}
