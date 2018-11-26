// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal abstract class DiagnosticComparer : IComparer<Diagnostic>, IEqualityComparer<Diagnostic>, IComparer, IEqualityComparer
    {
        public static DiagnosticComparer Id { get; } = new DiagnosticIdComparer();

        public static DiagnosticComparer SpanStart { get; } = new DiagnosticSpanStartComparer();

        public abstract int Compare(Diagnostic x, Diagnostic y);

        public abstract bool Equals(Diagnostic x, Diagnostic y);

        public abstract int GetHashCode(Diagnostic obj);

        public int Compare(object x, object y)
        {
            if (x == y)
                return 0;

            if (x == null)
                return -1;

            if (y == null)
                return 1;

            if (x is Diagnostic a
                && y is Diagnostic b)
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

            if (x is Diagnostic a
                && y is Diagnostic b)
            {
                return Equals(a, b);
            }

            return x.Equals(y);
        }

        public int GetHashCode(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            if (obj is Diagnostic diagnostic)
                return GetHashCode(diagnostic);

            return obj.GetHashCode();
        }

        private class DiagnosticIdComparer : DiagnosticComparer
        {
            public override int Compare(Diagnostic x, Diagnostic y)
            {
                if (object.ReferenceEquals(x, y))
                    return 0;

                if (x == null)
                    return -1;

                if (y == null)
                    return 1;

                return string.Compare(x.Id, y.Id, StringComparison.Ordinal);
            }

            public override bool Equals(Diagnostic x, Diagnostic y)
            {
                if (object.ReferenceEquals(x, y))
                    return true;

                if (x == null)
                    return false;

                if (y == null)
                    return false;

                return string.Equals(x.Id, y.Id, StringComparison.Ordinal);
            }

            public override int GetHashCode(Diagnostic obj)
            {
                if (obj == null)
                    throw new ArgumentNullException(nameof(obj));

                return StringComparer.Ordinal.GetHashCode(obj.Id);
            }
        }

        private class DiagnosticSpanStartComparer : DiagnosticComparer
        {
            public override int Compare(Diagnostic x, Diagnostic y)
            {
                if (object.ReferenceEquals(x, y))
                    return 0;

                if (x == null)
                    return -1;

                if (y == null)
                    return 1;

                return Comparer<int>.Default.Compare(x.Location.SourceSpan.Start, y.Location.SourceSpan.Start);
            }

            public override bool Equals(Diagnostic x, Diagnostic y)
            {
                if (object.ReferenceEquals(x, y))
                    return true;

                if (x == null)
                    return false;

                if (y == null)
                    return false;

                return x.Location.SourceSpan.Start == y.Location.SourceSpan.Start;
            }

            public override int GetHashCode(Diagnostic obj)
            {
                if (obj == null)
                    throw new ArgumentNullException(nameof(obj));

                return obj.Location.SourceSpan.Start.GetHashCode();
            }
        }
    }
}
