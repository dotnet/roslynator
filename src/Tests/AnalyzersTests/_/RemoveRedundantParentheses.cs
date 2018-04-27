// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#pragma warning disable CS0162, CS1522, RCS1065, RCS1176, RCS1177

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class RemoveRedundantParentheses
    {
        private static readonly object _lockObject = new object();

        [Obsolete((""))]
        public static bool Bar(string value)
        {
            bool f = false;

            int i = 0;

            var foo = new Foo();

            while ((true))
            {
            }

            do
            {
            } while ((true));

            using (((IDisposable)null))
            {
            }

            lock ((_lockObject))
            {
            }

            if ((true))
            {
            }

            switch ((true))
            {
            }

            return (true);

            (Bar(""));

            Bar((""));

            var arr = new string[] { (null) };

            var dic = new Dictionary<object, object>() { ([0] = null) };

            var items = new List<string>() { (null) };

            string s = $"{("")}";

            (f) = (false);
            (i) += (0);
            (i) -= (0);
            (i) *= (0);
            (i) /= (0);
            (i) %= (0);
            (f) &= (false);
            (f) ^= (false);
            (f) |= (false);
            (i) <<= (0);
            (i) >>= (0);

            f = !(f);
            f = !(s.StartsWith(""));
            f = !(foo.Value);
            f = !(foo[0]);
        }

        public static string Bar2() => (null);

        public static IEnumerable<object> Bar3()
        {
            yield return (null);
        }

        private static async Task FooAsync()
        {
            await (FooAsync().ConfigureAwait(false));

            await ((Task)FooAsync());
        }

        //n

        public static void Bar4(IEnumerable<string> items)
        {
            foreach (string item in (items))
            {
            }

            foreach ((string, string) item in (Tuple.Values))
            {
            }

            string s = $"{((true) ? "a" : "b")}";
        }

        private static async Task FooAsync(Task task) => await (task = Task.Run(default(Action)));

        private class Foo
        {
            public bool Value { get; }

            public bool this[int i]
            {
                get { return i == 0; }
            }
        }
    }
}
