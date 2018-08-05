// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.Tests.Text
{
    public readonly struct LinePositionInfo : IEquatable<LinePositionInfo>
    {
        public LinePositionInfo(int index, int lineIndex, int columnIndex)
        {
            Index = index;
            LineIndex = lineIndex;
            ColumnIndex = columnIndex;
        }

        public int Index { get; }

        public int LineIndex { get; }

        public int ColumnIndex { get; }

        public LinePosition LinePosition
        {
            get { return new LinePosition(LineIndex, ColumnIndex); }
        }

        public override bool Equals(object obj)
        {
            return obj is LinePositionInfo other
                && Equals(other);
        }

        public bool Equals(LinePositionInfo other)
        {
            return Index == other.Index
                   && LineIndex == other.LineIndex
                   && ColumnIndex == other.ColumnIndex;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(Index, Hash.Combine(LineIndex, ColumnIndex));
        }

        public static bool operator ==(in LinePositionInfo info1, in LinePositionInfo info2)
        {
            return info1.Equals(info2);
        }

        public static bool operator !=(in LinePositionInfo info1, in LinePositionInfo info2)
        {
            return !(info1 == info2);
        }
    }
}
