// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;

namespace Roslynator.Tests.Text
{
    public readonly struct TextSpanParserResult : IEquatable<TextSpanParserResult>
    {
        public TextSpanParserResult(string text, ImmutableArray<LinePositionSpanInfo> spans)
        {
            Text = text;
            Spans = spans;
        }

        public string Text { get; }

        public ImmutableArray<LinePositionSpanInfo> Spans { get; }

        public override bool Equals(object obj)
        {
            return obj is TextSpanParserResult other
                && Equals(other);
        }

        public bool Equals(TextSpanParserResult other)
        {
            return Text == other.Text
                   && Spans.Equals(other.Spans);
        }

        public override int GetHashCode()
        {
            return Hash.Combine(Spans.GetHashCode(), Hash.Create(Text));
        }

        public static bool operator ==(in TextSpanParserResult analysis1, in TextSpanParserResult analysis2)
        {
            return analysis1.Equals(analysis2);
        }

        public static bool operator !=(in TextSpanParserResult analysis1, in TextSpanParserResult analysis2)
        {
            return !(analysis1 == analysis2);
        }
    }
}
