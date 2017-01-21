// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Roslynator.CSharp.Refactorings.Tests
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
        }
    }
}
