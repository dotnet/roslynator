// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator.Testing;

// Copy of Roslynator.DiagnosticDeepEqualityComparer (src/Core/DiagnosticDeepEqualityComparer.cs),
// with GetHashCode reimplemented without Roslynator.Hash.
// Duplicated so that the testing framework does not depend on the Roslynator.Core package.
internal sealed class DiagnosticDeepEqualityComparer : IEqualityComparer<Diagnostic>
{
    public static DiagnosticDeepEqualityComparer Instance { get; } = new();

    internal static bool Equals(ImmutableArray<Diagnostic> first, ImmutableArray<Diagnostic> second)
    {
        return first.Length == second.Length
            && first.Intersect(second, Instance).Count() == first.Length;
    }

    private DiagnosticDeepEqualityComparer()
    {
    }

    public bool Equals(Diagnostic? x, Diagnostic? y)
    {
        if (object.ReferenceEquals(x, y))
            return true;

        if (x is null)
            return false;

        if (y is null)
            return false;

        if (!x.Descriptor.Equals(y.Descriptor))
            return false;

        if (!x.Location.GetLineSpan().Equals(y.Location.GetLineSpan()))
            return false;

        if (x.Severity != y.Severity)
            return false;

        if (x.WarningLevel != y.WarningLevel)
            return false;

        return true;
    }

    public int GetHashCode(Diagnostic obj)
    {
        if (obj is null)
            return 0;

        int hashCode = obj.Descriptor.GetHashCode();
        hashCode = (hashCode * 397) ^ obj.Location.GetLineSpan().GetHashCode();
        hashCode = (hashCode * 397) ^ (int)obj.Severity;
        hashCode = (hashCode * 397) ^ obj.WarningLevel;
        return hashCode;
    }
}
