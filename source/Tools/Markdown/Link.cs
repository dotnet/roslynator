// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Markdown
{
    public struct Link : IMarkdown
    {
        internal Link(string text, string url)
        {
            Text = text;
            Url = url;
        }

        public string Text { get; }

        public string Url { get; }

        public MarkdownBuilder AppendTo(MarkdownBuilder mb)
        {
            return mb.AppendLink(Text, Url);
        }
    }
}
