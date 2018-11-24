// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Roslynator
{
    internal static class Empty
    {
        public static IEnumerator<T> Enumerator<T>()
        {
            return Collections.Enumerator<T>.Instance;
        }

        public static IEnumerable<T> Enumerable<T>()
        {
            return Collections.ReadOnlyList<T>.Instance;
        }

        public static ICollection<T> Collection<T>()
        {
            return Collections.ReadOnlyList<T>.Instance;
        }

        public static IList<T> List<T>()
        {
            return Collections.ReadOnlyList<T>.Instance;
        }

        public static IReadOnlyList<T> ReadOnlyList<T>()
        {
            return Collections.ReadOnlyList<T>.Instance;
        }
    }
}
