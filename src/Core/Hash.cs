// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

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

        public static int CombineValues<T>(IEnumerable<T> values, IEqualityComparer<T> comparer = null, int maxItemsToHash = int.MaxValue)
        {
            if (values == null)
                return 0;

            if (comparer == null)
                comparer = EqualityComparer<T>.Default;

            int hash = 0;

            int count = 0;
            foreach (T value in values)
            {
                if (count >= maxItemsToHash)
                    break;

                if (!comparer.Equals(value, default(T)))
                    hash = Combine(comparer.GetHashCode(value), hash);

                count++;
            }

            return hash;
        }

        public static int CombineValues<T>(T[] values, IEqualityComparer<T> comparer = null, int maxItemsToHash = int.MaxValue)
        {
            if (values == null)
                return 0;

            if (comparer == null)
                comparer = EqualityComparer<T>.Default;

            int hash = 0;

            int maxSize = Math.Min(maxItemsToHash, values.Length);

            for (int i = 0; i < maxSize; i++)
            {
                T value = values[i];

                if (!comparer.Equals(value, default(T)))
                    hash = Combine(comparer.GetHashCode(value), hash);
            }

            return hash;
        }

        public static int CombineValues<T>(ImmutableArray<T> values, IEqualityComparer<T> comparer = null, int maxItemsToHash = int.MaxValue)
        {
            if (values.IsDefaultOrEmpty)
                return 0;

            if (comparer == null)
                comparer = EqualityComparer<T>.Default;

            int hash = 0;

            int maxSize = Math.Min(maxItemsToHash, values.Length);

            for (int i = 0; i < maxSize; i++)
            {
                T value = values[i];

                if (!comparer.Equals(value, default(T)))
                    hash = Combine(comparer.GetHashCode(value), hash);
            }

            return hash;
        }

        public static int CombineValues(
            IEnumerable<string> values,
            StringComparer stringComparer,
            int maxItemsToHash = int.MaxValue)
        {
            if (values == null)
                return 0;

            int hash = 0;

            int count = 0;
            foreach (string value in values)
            {
                if (count >= maxItemsToHash)
                    break;

                if (value != null)
                    hash = Combine(stringComparer.GetHashCode(value), hash);

                count++;
            }

            return hash;
        }

        public static int CombineValues(
            string[] values,
            StringComparer stringComparer,
            int maxItemsToHash = int.MaxValue)
        {
            if (values == null)
                return 0;

            int hash = 0;

            int maxSize = Math.Min(maxItemsToHash, values.Length);

            for (int i = 0; i < maxSize; i++)
            {
                string value = values[i];

                if (value != null)
                    hash = Combine(stringComparer.GetHashCode(values[i]), hash);
            }

            return hash;
        }

        public static int CombineValues(
            ImmutableArray<string> values,
            StringComparer stringComparer,
            int maxItemsToHash = int.MaxValue)
        {
            if (values.IsDefaultOrEmpty)
                return 0;

            int hash = 0;

            int maxSize = Math.Min(maxItemsToHash, values.Length);

            for (int i = 0; i < maxSize; i++)
            {
                string value = values[i];

                if (value != null)
                    hash = Combine(stringComparer.GetHashCode(value), hash);
            }

            return hash;
        }
    }
}
