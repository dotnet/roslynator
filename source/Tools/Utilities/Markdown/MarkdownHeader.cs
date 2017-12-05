// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;

namespace Roslynator.Utilities.Markdown
{
    public struct MarkdownHeader : IAppendable
    {
        public MarkdownHeader(string text, int level)
        {
            if (level < 1
                || level > 6)
            {
                throw new ArgumentOutOfRangeException(nameof(level), level, "");
            }

            Text = text;
            Level = level;
        }

        public string Text { get; }

        public int Level { get; }

        public StringBuilder Append(StringBuilder sb)
        {
            return sb
                .Append('#', Level)
                .Append(" ")
                .AppendLineIf(!string.IsNullOrEmpty(Text), Text, escape: true);
        }

        public override string ToString()
        {
            string s = GetHeaderStart();

            if (!string.IsNullOrEmpty(Text))
                s = s + Text + Environment.NewLine;

            return s;
        }

        private string GetHeaderStart()
        {
            switch (Level)
            {
                case 1:
                    return "# ";
                case 2:
                    return "## ";
                case 3:
                    return "### ";
                case 4:
                    return "#### ";
                case 5:
                    return "##### ";
                case 6:
                    return "###### ";
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
