// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal readonly struct Interval : IEquatable<Interval>
    {
        public Interval(int min, int max)
        {
            if (max < min)
                throw new ArgumentOutOfRangeException(nameof(max), "");

            Min = min;
            Max = max;
        }

        public int Min { get; }

        public int Max { get; }

        public bool Contains(int value) => Contains(new Interval(value, value));

        public bool Contains<T>(ImmutableArray<T> items) => Contains(new Interval(items.Length, items.Length));

        public bool Contains<T>(SyntaxList<T> items) where T : SyntaxNode
        {
            return Contains(new Interval(0, items.Count));
        }

        public bool Contains<T>(SeparatedSyntaxList<T> items) where T : SyntaxNode
        {
            return Contains(new Interval(0, items.Count));
        }

        public bool Contains(Interval interval)
        {
            if (Min >= 0
                && interval.Min >= 0
                && interval.Min < Min)
            {
                return false;
            }

            if (Max >= 0
                && interval.Max >= 0
                && interval.Max > Max)
            {
                return false;
            }

            return true;
        }

        public Interval ShiftLeft(int value) => new(Min - value, Max - value);

        public Interval ShiftRight(int value) => new(Min + value, Max + value);

        public static bool operator ==(Interval left, Interval right) => left.Equals(right);

        public static bool operator !=(Interval left, Interval right) => !left.Equals(right);

        public bool Equals(Interval other) => Min == other.Min && Max == other.Max;

        public override bool Equals(object obj) => obj is Interval span && Equals(span);

        public override int GetHashCode() => Hash.Combine(Min, Max);

        public override string ToString() => $"[{Min}..{Max}]";
    }
}
