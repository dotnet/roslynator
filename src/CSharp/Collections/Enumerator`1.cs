// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Roslynator.Collections
{
    internal class Enumerator<T> : Enumerator, IEnumerator<T>
    {
        new public static readonly IEnumerator<T> Instance = new Enumerator<T>();

        protected Enumerator()
        {
        }

        new public T Current
        {
            get { throw new InvalidOperationException(); }
        }

        public void Dispose()
        {
        }
    }
}
