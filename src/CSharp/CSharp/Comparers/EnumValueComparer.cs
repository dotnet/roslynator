// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Roslynator.CSharp.Comparers
{
    internal static class EnumValueComparer
    {
        public static IComparer<object> GetInstance(SpecialType enumSpecialType)
        {
            switch (enumSpecialType)
            {
                case SpecialType.System_SByte:
                    return SByteValueComparer.Instance;
                case SpecialType.System_Byte:
                    return ByteValueComparer.Instance;
                case SpecialType.System_Int16:
                    return ShortValueComparer.Instance;
                case SpecialType.System_UInt16:
                    return UShortValueComparer.Instance;
                case SpecialType.System_Int32:
                    return IntValueComparer.Instance;
                case SpecialType.System_UInt32:
                    return UIntValueComparer.Instance;
                case SpecialType.System_Int64:
                    return LongValueComparer.Instance;
                case SpecialType.System_UInt64:
                    return ULongValueComparer.Instance;
                default:
                    throw new ArgumentException("", nameof(enumSpecialType));
            }
        }

        private class SByteValueComparer : IComparer<object>
        {
            private SByteValueComparer()
            {
            }

            public static readonly SByteValueComparer Instance = new SByteValueComparer();

            public int Compare(object x, object y)
            {
                if (object.ReferenceEquals(x, y))
                    return 0;

                if (x == null)
                    return -1;

                if (y == null)
                    return 1;

                if (x is sbyte xvalue
                    && y is sbyte yvalue)
                {
                    return xvalue.CompareTo(yvalue);
                }

                return 0;
            }
        }

        private class ByteValueComparer : IComparer<object>
        {
            private ByteValueComparer()
            {
            }

            public static readonly ByteValueComparer Instance = new ByteValueComparer();

            public int Compare(object x, object y)
            {
                if (object.ReferenceEquals(x, y))
                    return 0;

                if (x == null)
                    return -1;

                if (y == null)
                    return 1;

                if (x is byte xvalue
                    && y is byte yvalue)
                {
                    return xvalue.CompareTo(yvalue);
                }

                return 0;
            }
        }

        private class ShortValueComparer : IComparer<object>
        {
            private ShortValueComparer()
            {
            }

            public static readonly ShortValueComparer Instance = new ShortValueComparer();

            public int Compare(object x, object y)
            {
                if (object.ReferenceEquals(x, y))
                    return 0;

                if (x == null)
                    return -1;

                if (y == null)
                    return 1;

                if (x is short xvalue
                    && y is short yvalue)
                {
                    return xvalue.CompareTo(yvalue);
                }

                return 0;
            }
        }

        private class UShortValueComparer : IComparer<object>
        {
            private UShortValueComparer()
            {
            }

            public static readonly UShortValueComparer Instance = new UShortValueComparer();

            public int Compare(object x, object y)
            {
                if (object.ReferenceEquals(x, y))
                    return 0;

                if (x == null)
                    return -1;

                if (y == null)
                    return 1;

                if (x is ushort xvalue
                    && y is ushort yvalue)
                {
                    return xvalue.CompareTo(yvalue);
                }

                return 0;
            }
        }

        private class IntValueComparer : IComparer<object>
        {
            private IntValueComparer()
            {
            }

            public static readonly IntValueComparer Instance = new IntValueComparer();

            public int Compare(object x, object y)
            {
                if (object.ReferenceEquals(x, y))
                    return 0;

                if (x == null)
                    return -1;

                if (y == null)
                    return 1;

                if (x is int xvalue
                    && y is int yvalue)
                {
                    return xvalue.CompareTo(yvalue);
                }

                return 0;
            }
        }

        private class UIntValueComparer : IComparer<object>
        {
            private UIntValueComparer()
            {
            }

            public static readonly UIntValueComparer Instance = new UIntValueComparer();

            public int Compare(object x, object y)
            {
                if (object.ReferenceEquals(x, y))
                    return 0;

                if (x == null)
                    return -1;

                if (y == null)
                    return 1;

                if (x is uint xvalue
                    && y is uint yvalue)
                {
                    return xvalue.CompareTo(yvalue);
                }

                return 0;
            }
        }

        private class LongValueComparer : IComparer<object>
        {
            private LongValueComparer()
            {
            }

            public static readonly LongValueComparer Instance = new LongValueComparer();

            public int Compare(object x, object y)
            {
                if (object.ReferenceEquals(x, y))
                    return 0;

                if (x == null)
                    return -1;

                if (y == null)
                    return 1;

                if (x is long xvalue
                    && y is long yvalue)
                {
                    return xvalue.CompareTo(yvalue);
                }

                return 0;
            }
        }

        private class ULongValueComparer : IComparer<object>
        {
            private ULongValueComparer()
            {
            }

            public static readonly ULongValueComparer Instance = new ULongValueComparer();

            public int Compare(object x, object y)
            {
                if (object.ReferenceEquals(x, y))
                    return 0;

                if (x == null)
                    return -1;

                if (y == null)
                    return 1;

                if (x is ulong xvalue
                    && y is ulong yvalue)
                {
                    return xvalue.CompareTo(yvalue);
                }

                return 0;
            }
        }
    }
}
