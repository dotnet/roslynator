// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Text
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal readonly struct IndentationInfo : IEquatable<IndentationInfo>
    {
        public IndentationInfo(SyntaxToken token, TextSpan span)
        {
            Token = token;
            Span = span;
        }

        public SyntaxToken Token { get; }

        public TextSpan Span { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"[{Span.Start}..{Span.End})  {Token}";

        public override string ToString() => (Span.IsEmpty) ? "" : Token.ToString(Span);

        public SyntaxTrivia GetTrivia()
        {
            if (!Span.IsEmpty)
            {
                foreach (SyntaxTrivia trivia in Token.LeadingTrivia)
                {
                    if (trivia.Span == Span)
                        return trivia;
                }
            }

            return default;
        }

        public override bool Equals(object obj)
        {
            return obj is IndentationInfo other
                && Equals(other);
        }

        public bool Equals(IndentationInfo other)
        {
            return Span.Equals(other.Span)
                && Token.Equals(other.Token);
        }

        public override int GetHashCode()
        {
            return Hash.Combine(Span.GetHashCode(), Token.GetHashCode());
        }

        public static bool operator ==(IndentationInfo left, IndentationInfo right) => left.Equals(right);

        public static bool operator !=(IndentationInfo left, IndentationInfo right) => !(left == right);
    }
}
