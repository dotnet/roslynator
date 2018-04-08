// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Roslynator.Collections
{
    internal class ReadOnlyList<T> : ReadOnlyCollection<T>, IList<T>, IReadOnlyList<T>
    {
        new public static readonly ReadOnlyList<T> Instance = new ReadOnlyList<T>();

        protected ReadOnlyList()
        {
        }

        public int IndexOf(T item)
        {
            return -1;
        }

        public void Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public T this[int index]
        {
            get { throw new ArgumentOutOfRangeException(nameof(index)); }
            set { throw new NotSupportedException(); }
        }
    }
}
