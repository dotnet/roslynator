// This code is originally from https://github.com/josefpihrt/orang. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Roslynator
{
    internal static class ListCache<T>
    {
        private const int MaxSize = 256;
        private const int DefaultCapacity = 16;

        [ThreadStatic]
        private static List<T> _cachedInstance;

        public static List<T> GetInstance(int capacity = DefaultCapacity)
        {
            if (capacity <= MaxSize)
            {
                List<T> list = _cachedInstance;

                Debug.Assert(list == null || list.Count == 0, "");

                if (list != null
                    && capacity <= list.Capacity)
                {
                    _cachedInstance = null;
                    return list;
                }
            }

            return new List<T>(capacity);
        }

        public static void Free(List<T> list)
        {
            list.Clear();

            if (list.Capacity <= MaxSize)
                _cachedInstance = list;
        }
    }
}
