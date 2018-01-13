// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Roslynator.CSharp.Comparers
{
    internal class EnumMemberValueComparer : IComparer<object>
    {
        private EnumMemberValueComparer()
        {
        }

        public static readonly EnumMemberValueComparer Instance = new EnumMemberValueComparer();

        public int Compare(object x, object y)
        {
            if (object.ReferenceEquals(x, y))
                return 0;

            if (x == null)
                return -1;

            if (y == null)
                return 1;

            if (x is sbyte)
            {
                if (y is sbyte)
                    return ((sbyte)x).CompareTo((sbyte)y);
            }
            else if (x is byte)
            {
                if (y is byte)
                    return ((byte)x).CompareTo((byte)y);
            }
            else if (x is ushort)
            {
                if (y is ushort)
                    return ((ushort)x).CompareTo((ushort)y);
            }
            else if (x is short)
            {
                if (y is short)
                    return ((short)x).CompareTo((short)y);
            }
            else if (x is uint)
            {
                if (y is uint)
                    return ((uint)x).CompareTo((uint)y);
            }
            else if (x is int)
            {
                if (y is int)
                    return ((int)x).CompareTo((int)y);
            }
            else if (x is ulong)
            {
                if (y is ulong)
                    return ((ulong)x).CompareTo((ulong)y);
            }
            else if (x is long)
            {
                if (y is long)
                    return ((long)x).CompareTo((long)y);
            }

            Debug.Fail($"{nameof(EnumMemberValueComparer)} cannot compare {x.GetType()} with {y.GetType()}");

            return 0;
        }

        public static bool IsListSorted(IList<object> values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            int count = values.Count;

            if (count > 1)
            {
                for (int i = 0; i < count - 1; i++)
                {
                    if (Instance.Compare(values[i], values[i + 1]) > 0)
                        return false;
                }
            }

            return true;
        }
    }
}
