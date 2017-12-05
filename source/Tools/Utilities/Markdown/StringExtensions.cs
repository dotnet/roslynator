// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Utilities.Markdown
{
    public static class StringExtensions
    {
        public static string EscapeMarkdown(this string value)
        {
            return MarkdownUtility.Escape(value);
        }
    }
}
