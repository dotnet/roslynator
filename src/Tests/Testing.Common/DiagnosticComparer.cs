// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Roslynator.Testing;

// Trimmed copy of Roslynator.DiagnosticComparer (src/Core/DiagnosticComparer.cs).
// Duplicated so that the testing framework does not depend on the Roslynator.Core package.
internal abstract class DiagnosticComparer : IComparer<Diagnostic>, IEqualityComparer<Diagnostic>
{
    public static DiagnosticComparer Id { get; } = new DiagnosticIdComparer();

    public static DiagnosticComparer SpanStart { get; } = new DiagnosticSpanStartComparer();

    public abstract int Compare(Diagnostic? x, Diagnostic? y);

    public abstract bool Equals(Diagnostic? x, Diagnostic? y);

    public abstract int GetHashCode(Diagnostic obj);

    private class DiagnosticIdComparer : DiagnosticComparer
    {
        public override int Compare(Diagnostic? x, Diagnostic? y)
        {
            if (object.ReferenceEquals(x, y))
                return 0;

            if (x is null)
                return -1;

            if (y is null)
                return 1;

            return string.CompareOrdinal(x.Id, y.Id);
        }

        public override bool Equals(Diagnostic? x, Diagnostic? y)
        {
            if (object.ReferenceEquals(x, y))
                return true;

            if (x is null)
                return false;

            if (y is null)
                return false;

            return string.Equals(x.Id, y.Id, StringComparison.Ordinal);
        }

        public override int GetHashCode(Diagnostic obj)
        {
            if (obj is null)
                return 0;

            return StringComparer.Ordinal.GetHashCode(obj.Id);
        }
    }

    private class DiagnosticSpanStartComparer : DiagnosticComparer
    {
        public override int Compare(Diagnostic? x, Diagnostic? y)
        {
            if (object.ReferenceEquals(x, y))
                return 0;

            if (x is null)
                return -1;

            if (y is null)
                return 1;

            return x.Location.SourceSpan.Start.CompareTo(y.Location.SourceSpan.Start);
        }

        public override bool Equals(Diagnostic? x, Diagnostic? y)
        {
            if (object.ReferenceEquals(x, y))
                return true;

            if (x is null)
                return false;

            if (y is null)
                return false;

            return x.Location.SourceSpan.Start == y.Location.SourceSpan.Start;
        }

        public override int GetHashCode(Diagnostic obj)
        {
            if (obj is null)
                return 0;

            return obj.Location.SourceSpan.Start.GetHashCode();
        }
    }
}
