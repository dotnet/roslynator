// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Roslynator
{
    internal abstract class DiagnosticIdComparer : IComparer<string>, IEqualityComparer<string>, IComparer, IEqualityComparer
    {
        public static DiagnosticIdComparer Prefix { get; } = new DiagnosticIdPrefixComparer();

        public abstract int Compare(string x, string y);

        public abstract bool Equals(string x, string y);

        public abstract int GetHashCode(string obj);

        public int Compare(object x, object y)
        {
            if (x == y)
                return 0;

            if (x == null)
                return -1;

            if (y == null)
                return 1;

            if (x is string a
                && y is string b)
            {
                return Compare(a, b);
            }

            throw new ArgumentException("", nameof(x));
        }

        new public bool Equals(object x, object y)
        {
            if (x == y)
                return true;

            if (x == null)
                return false;

            if (y == null)
                return false;

            if (x is string a
                && y is string b)
            {
                return Equals(a, b);
            }

            throw new ArgumentException("", nameof(x));
        }

        public int GetHashCode(object obj)
        {
            if (obj == null)
                return 0;

            if (obj is string descriptor)
                return GetHashCode(descriptor);

            throw new ArgumentException("", nameof(obj));
        }

        private class DiagnosticIdPrefixComparer : DiagnosticIdComparer
        {
            public override int Compare(string x, string y)
            {
                if (object.ReferenceEquals(x, y))
                    return 0;

                if (x == null)
                    return -1;

                if (y == null)
                    return 1;

                int length1 = DiagnosticIdPrefix.GetPrefixLength(x);
                int length2 = DiagnosticIdPrefix.GetPrefixLength(y);

                if (length1 == length2)
                    return string.Compare(x, 0, y, 0, length1, StringComparison.Ordinal);

                int length = Math.Min(length1, length2);

                int result = string.Compare(x, 0, y, 0, length, StringComparison.Ordinal);

                if (result != 0)
                    return result;

                return length1.CompareTo(length2);
            }

            public override bool Equals(string x, string y)
            {
                if (object.ReferenceEquals(x, y))
                    return true;

                if (x == null)
                    return false;

                if (y == null)
                    return false;

                int length1 = DiagnosticIdPrefix.GetPrefixLength(x);
                int length2 = DiagnosticIdPrefix.GetPrefixLength(y);

                return length1 == length2
                    && string.Compare(x, 0, y, 0, length1, StringComparison.Ordinal) == 0;
            }

            public override int GetHashCode(string obj)
            {
                if (obj == null)
                    throw new ArgumentNullException(nameof(obj));

                int length = DiagnosticIdPrefix.GetPrefixLength(obj);

                return StringComparer.Ordinal.GetHashCode(obj.Substring(0, length));
            }
        }
    }
}
