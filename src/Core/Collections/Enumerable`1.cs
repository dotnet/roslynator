// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;

namespace Roslynator.Collections
{
    internal class Enumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerator<T> _enumerator = Enumerator<T>.Instance;

        public IEnumerator<T> GetEnumerator()
        {
            return _enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
