// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Tests.Text
{
    public readonly struct LinePositionSpanInfo : IEquatable<LinePositionSpanInfo>
    {
        public LinePositionSpanInfo(in LinePositionInfo start, in LinePositionInfo end)
        {
            Start = start;
            End = end;
        }

        public LinePositionInfo Start { get; }

        public LinePositionInfo End { get; }

        public TextSpan Span
        {
            get { return TextSpan.FromBounds(Start.Index, End.Index); }
        }

        public LinePositionSpan LineSpan
        {
            get { return new LinePositionSpan(Start.LinePosition, End.LinePosition); }
        }

        public override bool Equals(object obj)
        {
            return obj is LinePositionSpanInfo other
                && Equals(other);
        }

        public bool Equals(LinePositionSpanInfo other)
        {
            return Start.Equals(other.Start)
                   && End.Equals(other.End);
        }

        public override int GetHashCode()
        {
            return Hash.Combine(Start.GetHashCode(), End.GetHashCode());
        }

        public static bool operator ==(in LinePositionSpanInfo info1, in LinePositionSpanInfo info2)
        {
            return info1.Equals(info2);
        }

        public static bool operator !=(in LinePositionSpanInfo info1, in LinePositionSpanInfo info2)
        {
            return !(info1 == info2);
        }
    }
}
