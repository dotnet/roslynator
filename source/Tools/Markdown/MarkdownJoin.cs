// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Roslynator.Markdown
{
    public struct MarkdownJoin : IMarkdown
    {
        internal MarkdownJoin(string separator, IEnumerable<object> values, bool escape)
        {
            Separator = separator;
            Values = values;
            Escape = escape;
        }

        public string Separator { get; }

        public IEnumerable<object> Values { get; }

        public bool Escape { get; }

        public MarkdownBuilder AppendTo(MarkdownBuilder mb)
        {
            bool isFirst = true;

            foreach (object value in Values)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    mb.Append(Separator, escape: Escape);
                }

                mb.Append(value, escape: Escape);
            }

            return mb;
        }
    }
}
