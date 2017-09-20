// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

#pragma warning disable RCS1097, RCS1118, RCS1176

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class UseElementAccessInsteadOfElementAt
    {
        public static void Foo()
        {
            var items = new List<int>() { 0, 1, 2 };

            int element = items.ElementAt(1);

            var a = new int[] { 0 };
            element = a.ElementAt(1);

            ImmutableArray<int> ia = ImmutableArray.Create(1);
            element = ia.ElementAt(1);

            string s = "";
            char ch = s.ElementAt(1);

            // n

            var dic = new Dictionary<string, string>();
            KeyValuePair<string, string> kvp = dic.ElementAt(1);
        }

        public static void Foo2()
        {
            var items = new List<int>() { 0, 1, 2 };

            int element = items.ToList().ElementAt(1);

            var a = new int[] { 0 };
            element = a.ToArray().ElementAt(1);

            ImmutableArray<int> ia = ImmutableArray.Create(1);
            element = ia.ToImmutableArray().ElementAt(1);

            string s = "";
            char ch = s.ToString().ElementAt(1);

            // n

            var dic = new Dictionary<string, string>();
            KeyValuePair<string, string> kvp = dic.ElementAt(1);
        }
    }
}
