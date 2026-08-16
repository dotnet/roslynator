// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Roslynator.Testing;

// Trimmed copy of Roslynator.DiagnosticDescriptorComparer (src/Core/DiagnosticDescriptorComparer.cs).
// Duplicated so that the testing framework does not depend on the Roslynator.Core package.
internal abstract class DiagnosticDescriptorComparer : IEqualityComparer<DiagnosticDescriptor>
{
    public static DiagnosticDescriptorComparer Id { get; } = new DiagnosticDescriptorIdComparer();

    public abstract bool Equals(DiagnosticDescriptor? x, DiagnosticDescriptor? y);

    public abstract int GetHashCode(DiagnosticDescriptor obj);

    private class DiagnosticDescriptorIdComparer : DiagnosticDescriptorComparer
    {
        public override bool Equals(DiagnosticDescriptor? x, DiagnosticDescriptor? y)
        {
            if (object.ReferenceEquals(x, y))
                return true;

            if (x is null)
                return false;

            if (y is null)
                return false;

            return string.Equals(x.Id, y.Id, StringComparison.Ordinal);
        }

        public override int GetHashCode(DiagnosticDescriptor obj)
        {
            if (obj is null)
                return 0;

            return StringComparer.Ordinal.GetHashCode(obj.Id);
        }
    }
}
