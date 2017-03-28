// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Roslynator.CSharp.Refactorings.Test
{
    internal class ReplaceForEachWithForRefactoring
    {
        public void SomeMethod()
        {
            string s = null;
            foreach (var ch in s)
            {
                char ch2 = ch; 
            }

            var arr = new string[] { null };
            foreach (var item in arr)
            {
                string item2 = item;
            }

            var ia = ImmutableArray.Create<string>("");
            foreach (var item in ia)
            {
                string item2 = item;
            }

            var items = new List<string>();
            foreach (string item in items)
            {
                string item2 = item;
            }

            MatchCollection matches = null;
            foreach (Match item in matches)
            {
                Match item2 = item;
            }

            Foo foo = null;
            foreach (var item in foo)
            {
                string item2 = item;
            }
        }

        private class Foo : IEnumerable<string>
        {
            public int this[int index]
            {
                get { return 0; }
            }

            public IEnumerator<string> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }
    }
}
