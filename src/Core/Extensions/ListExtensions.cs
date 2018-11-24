// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Roslynator
{
    internal static class ListExtensions
    {
        public static T SingleOrDefault<T>(this IReadOnlyList<T> list, bool shouldThrow)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (shouldThrow)
            {
                return list.SingleOrDefault();
            }
            else
            {
                return (list.Count == 1) ? list[0] : default(T);
            }
        }

        public static T SingleOrDefault<T>(this IReadOnlyList<T> list, Func<T, bool> predicate, bool shouldThrow)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (shouldThrow)
                return list.SingleOrDefault(predicate);

            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                if (predicate(list[i]))
                {
                    for (int j = i + 1; j < count; j++)
                    {
                        if (predicate(list[j]))
                            return default(T);
                    }

                    return list[i];
                }
            }

            return default(T);
        }

        public static bool IsSorted<T>(this IReadOnlyList<T> values, IComparer<T> comparer)
        {
            int count = values.Count;

            if (count > 1)
            {
                for (int i = 0; i < count - 1; i++)
                {
                    if (comparer.Compare(values[i], values[i + 1]) > 0)
                        return false;
                }
            }

            return true;
        }
    }
}
