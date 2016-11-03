// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class ReplaceMethodInvocationWithElementAccessRefactoring
    {
        public void SomeMethod()
        {
            var items = new List<int>() { 0, 1, 2 };

            var first = items.First();

            var last = items.Last();

            var element = items.ElementAt(1);


            var arr = new int[] { 0 };
            first = arr.First();

            string s = "abc";
            char ch = s.First();
        }
    }
}
