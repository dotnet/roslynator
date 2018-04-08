// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator
{
    //http://en.wikipedia.org/wiki/Fowler%E2%80%93Noll%E2%80%93Vo_hash_function

    internal static class Hash
    {
        public const int OffsetBasis = unchecked((int)2166136261);

        public const int Prime = 16777619;

        public static int Create(int value)
        {
            return Combine(value, OffsetBasis);
        }

        public static int Create(bool value)
        {
            return (value) ? 1 : 0;
        }

        public static int Create<T>(T value) where T : class
        {
            return Combine(value, OffsetBasis);
        }

        public static int Combine(int value, int hash)
        {
            return unchecked((hash * Prime) + value);
        }

        public static int Combine(bool value, int hash)
        {
            return Combine(hash, (value) ? 1 : 0);
        }

        public static int Combine<T>(T value, int hash) where T : class
        {
            hash = unchecked(hash * Prime);

            return (value != null) ? unchecked(hash + value.GetHashCode()) : hash;
        }
    }
}
