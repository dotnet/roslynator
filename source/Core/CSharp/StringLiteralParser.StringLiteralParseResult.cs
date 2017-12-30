// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp
{
    internal static partial class StringLiteralParser
    {
        private struct StringLiteralParseResult
        {
            private StringLiteralParseResult(string text, bool success)
            {
                Text = text;
                Success = success;
            }

            public StringLiteralParseResult(string text)
            {
                Text = text;
                Success = true;
            }

            public string Text { get; }

            public bool Success { get; }
        }
    }
}
