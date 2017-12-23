// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Markdown
{
    public struct MarkdownText : IMarkdown
    {
        internal MarkdownText(string text, EmphasisOptions options, bool escape)
        {
            Text = text;
            Options = options;
            Escape = escape;
        }

        public string Text { get; }

        public EmphasisOptions Options { get; }

        public bool Escape { get; }

        public MarkdownBuilder AppendTo(MarkdownBuilder mb)
        {
            return mb.Append(Text, Options, Escape);
        }
    }
}
