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
        public static bool MethodName(string value)
        {
            bool f = /*1*/(/*2*/
                           /*3*/false/*4*/
                                     /*5*/)/*6*/;

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

            (MethodName(""));

            MethodName((""));

            var arr = new string[] { (null) };

            var dic = new Dictionary<object, object>() { ([0] = null) };

            var items = new List<string>() { (null) };

            foreach (string item in (items))
            {
            }

            foreach ((string, string) item in (Tuple.Values))
            {
            }

            string s = $"{("")}";
            s = $"{((f) ? "a" : "b")}";

            int i = 0;

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
        }

        public static string MethodName2() => (null);

        public static IEnumerable<object> MethodName3()
        {
            yield return (null);
        }

        private static async Task FooAsync()
        {
            await (FooAsync().ConfigureAwait(false));

            await ((Task)FooAsync());
        }

        //n

        private static async Task FooAsync(Task task) => await (task = Task.Run(default(Action)));
    }
}
