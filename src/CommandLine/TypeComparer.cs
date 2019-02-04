// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Roslynator.CommandLine
{
    internal abstract class TypeComparer : IComparer<Type>, IEqualityComparer<Type>, IComparer, IEqualityComparer
    {
        public static TypeComparer NamespaceThenName { get; } = new NamespaceThenNameComparer();

        public abstract int Compare(Type x, Type y);

        public abstract bool Equals(Type x, Type y);

        public abstract int GetHashCode(Type obj);

        public int Compare(object x, object y)
        {
            if (x == y)
                return 0;

            if (x == null)
                return -1;

            if (y == null)
                return 1;

            if (x is Type a
                && y is Type b)
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

            if (x is Type a
                && y is Type b)
            {
                return Equals(a, b);
            }

            throw new ArgumentException("", nameof(x));
        }

        public int GetHashCode(object obj)
        {
            if (obj == null)
                return 0;

            if (obj is Type type)
                return GetHashCode(type);

            throw new ArgumentException("", nameof(obj));
        }

        private class NamespaceThenNameComparer : TypeComparer
        {
            public override int Compare(Type x, Type y)
            {
                if (object.ReferenceEquals(x, y))
                    return 0;

                if (x == null)
                    return -1;

                if (y == null)
                    return 1;

                int result = CompareNamespace(x.Namespace, y.Namespace);

                if (result != 0)
                    return result;

                return string.CompareOrdinal(x.Name, y.Name);
            }

            private static int CompareNamespace(string a, string b)
            {
                int prevIndex1 = -1;
                int prevIndex2 = -1;

                while (true)
                {
                    int startIndex1 = prevIndex1 + 1;
                    int startIndex2 = prevIndex2 + 1;

                    int index1 = a.IndexOf('.', startIndex1);
                    int index2 = b.IndexOf('.', startIndex2);

                    if (index1 == -1)
                    {
                        if (index2 == -1)
                        {
                            return string.CompareOrdinal(a, startIndex1, b, startIndex2, a.Length - startIndex1);
                        }
                        else
                        {
                            return -1;
                        }
                    }
                    else if (index2 == -1)
                    {
                        return 1;
                    }

                    int result = string.CompareOrdinal(a, startIndex1, b, startIndex2, index1 - startIndex1);

                    if (result != 0)
                        return result;

                    prevIndex1 = index1;
                    prevIndex2 = index2;
                }
            }

            public override bool Equals(Type x, Type y)
            {
                if (object.ReferenceEquals(x, y))
                    return true;

                if (x == null)
                    return false;

                if (y == null)
                    return false;

                return string.Equals(x.FullName, y.FullName, StringComparison.Ordinal);
            }

            public override int GetHashCode(Type obj)
            {
                if (obj == null)
                    throw new ArgumentNullException(nameof(obj));

                return StringComparer.Ordinal.GetHashCode(obj.FullName);
            }
        }
    }
}
