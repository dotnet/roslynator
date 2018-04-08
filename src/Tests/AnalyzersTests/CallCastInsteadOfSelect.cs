// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

#pragma warning disable RCS1016, RCS1021, RCS1176, RCS1196

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class CallCastInsteadOfSelect
    {
        private static void Bar()
        {
            var items = new List<string>();

            IEnumerable<object> q = items.Select(f => (object)f);

            q = Enumerable.Select(items, f => (object)f);

            q = items.Select((f) => (object)f);

            q = items.Select(f =>
            {
                return (object)f;
            });

            //n

            IEnumerable<Foo> foos = Enumerable.Repeat(new Foo2(), 1).Select(b => (Foo)b);

            byte[] x1 = Enumerable.Range(0, 1).Select(i => (byte)i).ToArray();
            long[] x2 = Enumerable.Range(0, 1).Select(i => (long)i).ToArray();

            x1 = Enumerable.Select(Enumerable.Range(0, 1), i => (byte)i).ToArray();
            x2 = Enumerable.Select(Enumerable.Range(0, 1), i => (long)i).ToArray();
        }

        private class Foo
        {
        }

        private class Foo2
        {
            public static explicit operator Foo(Foo2 value)
            {
                return new Foo();
            }
        }
    }
}
