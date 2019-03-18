// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Roslynator
{
    internal static class HashSetExtensions
    {
        public static bool ContainsAny<T>(this HashSet<T> items, T item1, T item2)
        {
            return items.Contains(item1)
                || items.Contains(item2);
        }

        public static bool ContainsAny<T>(this HashSet<T> items, T item1, T item2, T item3)
        {
            return items.Contains(item1)
                || items.Contains(item2)
                || items.Contains(item3);
        }

        public static bool ContainsAny<T>(this HashSet<T> items, T item1, T item2, T item3, T item4)
        {
            return items.Contains(item1)
                || items.Contains(item2)
                || items.Contains(item3)
                || items.Contains(item4);
        }
    }
}
