// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Rename
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal readonly struct DiffSpan
    {
        public DiffSpan(TextSpan span, int diff)
        {
            Span = span;
            Diff = diff;
        }

        public TextSpan Span { get; }

        public int Diff { get; }

        public int Start => Span.Start;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"{Start}  {Diff}";

        public DiffSpan Offset(int value) => new(Span.Offset(value), Diff);
    }
}
