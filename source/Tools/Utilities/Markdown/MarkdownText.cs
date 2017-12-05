// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text;

namespace Roslynator.Utilities.Markdown
{
    public struct MarkdownText : IAppendable
    {
        public MarkdownText(string text, bool shouldEscape = false)
        {
            Text = text;
            ShouldEscape = shouldEscape;
        }

        public string Text { get; }

        public bool ShouldEscape { get; }

        public StringBuilder Append(StringBuilder sb)
        {
            if (ShouldEscape)
            {
                return sb.AppendEscape(Text);
            }
            else
            {
                return sb.Append(Text);
            }
        }

        public override string ToString()
        {
            if (ShouldEscape)
            {
                return Text?.EscapeMarkdown();
            }
            else
            {
                return Text;
            }
        }
    }
}
