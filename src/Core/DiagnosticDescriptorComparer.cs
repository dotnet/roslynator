// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal abstract class DiagnosticDescriptorComparer : IComparer<DiagnosticDescriptor>, IEqualityComparer<DiagnosticDescriptor>, IComparer, IEqualityComparer
    {
        public static DiagnosticDescriptorComparer Id { get; } = new DiagnosticDescriptorIdComparer();

        public abstract int Compare(DiagnosticDescriptor x, DiagnosticDescriptor y);

        public abstract bool Equals(DiagnosticDescriptor x, DiagnosticDescriptor y);

        public abstract int GetHashCode(DiagnosticDescriptor obj);

        public int Compare(object x, object y)
        {
            if (x == y)
                return 0;

            if (x == null)
                return -1;

            if (y == null)
                return 1;

            if (x is DiagnosticDescriptor a
                && y is DiagnosticDescriptor b)
            {
                return Compare(a, b);
            }

            if (x is IComparable comparable)
                return comparable.CompareTo(y);

            throw new ArgumentException("An object must implement IComparable.", nameof(x));
        }

        new public bool Equals(object x, object y)
        {
            if (x == y)
                return true;

            if (x == null)
                return false;

            if (y == null)
                return false;

            if (x is DiagnosticDescriptor a
                && y is DiagnosticDescriptor b)
            {
                return Equals(a, b);
            }

            return x.Equals(y);
        }

        public int GetHashCode(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            if (obj is DiagnosticDescriptor descriptor)
                return GetHashCode(descriptor);

            return obj.GetHashCode();
        }

        private class DiagnosticDescriptorIdComparer : DiagnosticDescriptorComparer
        {
            public override int Compare(DiagnosticDescriptor x, DiagnosticDescriptor y)
            {
                if (object.ReferenceEquals(x, y))
                    return 0;

                if (x == null)
                    return -1;

                if (y == null)
                    return 1;

                return string.Compare(x.Id, y.Id, StringComparison.Ordinal);
            }

            public override bool Equals(DiagnosticDescriptor x, DiagnosticDescriptor y)
            {
                if (object.ReferenceEquals(x, y))
                    return true;

                if (x == null)
                    return false;

                if (y == null)
                    return false;

                return string.Equals(x.Id, y.Id, StringComparison.Ordinal);
            }

            public override int GetHashCode(DiagnosticDescriptor obj)
            {
                if (obj == null)
                    throw new ArgumentNullException(nameof(obj));

                return StringComparer.Ordinal.GetHashCode(obj.Id);
            }
        }
    }
}
