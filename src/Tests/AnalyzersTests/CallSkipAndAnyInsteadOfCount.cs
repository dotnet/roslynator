// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

#pragma warning disable RCS1023

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class CallSkipAndAnyInsteadOfCount
    {
        public static void Foo(int i)
        {
            IEnumerable<object> x = null;

            if (x.Count() > i) { }
            if (i < x.Count()) { }

            if (x.Count() >= i) { }
            if (i <= x.Count()) { }

            if (x.Count() <= i) { }
            if (i >= x.Count()) { }

            if (x.Count() < i) { }
            if (i > x.Count()) { }

            //n

            if (x.Count() == 1) { }
            if (x.Count() == i) { }
            if (1 == x.Count()) { }
            if (i == x.Count()) { }
            if (x.Count() != 1) { }
            if (x.Count() != i) { }
            if (1 != x.Count()) { }
            if (i != x.Count()) { }
        }
    }
}
