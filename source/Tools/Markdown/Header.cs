// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Markdown
{
    public struct Header : IMarkdown
    {
        internal Header(string text, int level = 1)
        {
            Text = text;
            Level = level;
        }

        public string Text { get; }

        public int Level { get; }

        public MarkdownBuilder AppendTo(MarkdownBuilder mb)
        {
            return mb.AppendHeader(Level, Text);
        }
    }
}
