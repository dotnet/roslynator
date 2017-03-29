// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

#pragma warning disable RCS1118, RCS1176

namespace Roslynator.CSharp.Analyzers.Test
{
    internal static class UseElementAccessInsteadOfFirst
    {
        public static void SomeMethod()
        {
            var items = new List<int>() { 0, 1, 2 };

            int first = items.First();

            var a = new int[] { 0 };
            first = a.First();

            ImmutableArray<int> ia = ImmutableArray.Create(1);
            first = ia.First();

            string s = "";
            char ch = s.First();

            // n

            var dic = new Dictionary<string, string>();
            KeyValuePair<string, string> kvp = dic.First();
        }
    }
}
