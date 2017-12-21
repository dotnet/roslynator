// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Markdown
{
    public struct OrderedListItem : IMarkdown
    {
        internal OrderedListItem(int number, string text)
        {
            Number = number;
            Text = text;
        }

        public int Number { get; }

        public string Text { get; }

        public MarkdownBuilder AppendTo(MarkdownBuilder mb)
        {
            return mb.AppendOrderedListItem(Number, Text);
        }
    }
}
