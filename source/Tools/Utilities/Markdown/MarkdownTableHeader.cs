// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.Utilities.Markdown
{
    public struct MarkdownTableHeader
    {
        public MarkdownTableHeader(string name, Alignment alignment = Alignment.Left)
        {
            Name = name;
            Alignment = alignment;
        }

        public string Name { get; }

        public Alignment Alignment { get; }

        public static implicit operator MarkdownTableHeader(string value)
        {
            return new MarkdownTableHeader(value);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
