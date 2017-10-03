// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Roslynator
{
    internal static class ListExtensions
    {
        public static T SingleOrDefault<T>(this IReadOnlyList<T> list, bool throwException)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (throwException)
            {
                return list.SingleOrDefault();
            }
            else
            {
                return (list.Count == 1) ? list[0] : default(T);
            }
        }
    }
}
