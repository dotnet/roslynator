// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Roslynator
{
    internal static class EnumerableExtensions
    {
        public static bool IsSorted<T>(this IEnumerable<T> enumerable, IComparer<T> comparer)
        {
            using (IEnumerator<T> en = enumerable.GetEnumerator())
            {
                if (en.MoveNext())
                {
                    T member1 = en.Current;

                    while (en.MoveNext())
                    {
                        T member2 = en.Current;

                        if (comparer.Compare(member1, member2) > 0)
                            return false;

                        member1 = member2;
                    }
                }
            }

            return true;
        }

        public static IEnumerable<T> ReplaceRangeAt<T>(
            this IEnumerable<T> enumerable,
            int startIndex,
            int count,
            IEnumerable<T> newNodes)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, "");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), count, "");

            return ReplaceRange();

            IEnumerable<T> ReplaceRange()
            {
                using (IEnumerator<T> en = enumerable.GetEnumerator())
                {
                    int i = 0;

                    while (i < startIndex)
                    {
                        if (!en.MoveNext())
                            throw new InvalidOperationException();

                        yield return en.Current;
                        i++;
                    }

                    int endIndex = startIndex + count;

                    while (i < endIndex)
                    {
                        if (!en.MoveNext())
                            throw new InvalidOperationException();

                        i++;
                    }

                    foreach (T newNode in newNodes)
                        yield return newNode;

                    while (en.MoveNext())
                        yield return en.Current;
                }
            }
        }

        public static IEnumerable<T> ModifyRange<T>(
            this IEnumerable<T> enumerable,
            int startIndex,
            int count,
            Func<T, T> selector)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, "");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), count, "");

            return ModifyRange();

            IEnumerable<T> ModifyRange()
            {
                using (IEnumerator<T> en = enumerable.GetEnumerator())
                {
                    int i = 0;

                    while (i < startIndex)
                    {
                        if (!en.MoveNext())
                            throw new InvalidOperationException();

                        yield return en.Current;
                        i++;
                    }

                    int endIndex = startIndex + count;

                    while (i < endIndex)
                    {
                        if (!en.MoveNext())
                            throw new InvalidOperationException();

                        yield return selector(en.Current);
                        i++;
                    }

                    while (en.MoveNext())
                        yield return en.Current;
                }
            }
        }
    }
}
