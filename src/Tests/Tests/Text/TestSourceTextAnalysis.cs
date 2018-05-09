// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;

namespace Roslynator.Tests.Text
{
    internal readonly struct TestSourceTextAnalysis : IEquatable<TestSourceTextAnalysis>
    {
        public TestSourceTextAnalysis(string source, ImmutableArray<LinePositionSpanInfo> spans)
        {
            Source = source;
            Spans = spans;
        }

        public string Source { get; }

        public ImmutableArray<LinePositionSpanInfo> Spans { get; }

        public override bool Equals(object obj)
        {
            return obj is TestSourceTextAnalysis other
                && Equals(other);
        }

        public bool Equals(TestSourceTextAnalysis other)
        {
            return Source == other.Source
                   && Spans.Equals(other.Spans);
        }

        public override int GetHashCode()
        {
            return Hash.Combine(Spans.GetHashCode(), Hash.Create(Source));
        }

        public static bool operator ==(TestSourceTextAnalysis analysis1, TestSourceTextAnalysis analysis2)
        {
            return analysis1.Equals(analysis2);
        }

        public static bool operator !=(TestSourceTextAnalysis analysis1, TestSourceTextAnalysis analysis2)
        {
            return !(analysis1 == analysis2);
        }
    }
}
