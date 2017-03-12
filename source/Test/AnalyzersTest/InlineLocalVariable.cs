// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Roslynator.CSharp.Analyzers.Test
{
#pragma warning disable RCS1118
    public static class InlineLocalVariable
    {
        private static void Foo()
        {
            // ...
            string s = "";

            string x = s;

            // ...
            const string s2 = "";

            x = s2;

            string s3 = "";
            #region
            x = s3;
            #endregion
        }

        private static void Foo2()
        {
            string s = "";
            string x = s;
        }

        private static void FooForEach()
        {
            IEnumerable<int> items = Enumerable.Range(0, 10);
            foreach (int item in items)
            {
            }
        }

        private static bool FooSwitch()
        {
            string x = "";

            switch (x)
            {
                case "a":
                    return true;
                case "b":
                    return false;
            }

            return false;
        }

        private static void Foo3()
        {
            string x = null;

            string s2 = "";
            x = s2;
        }

        private static void Foo4()
        {
            string s = "";
            string x = s;

            string x2 = s;
        }

        private static void Foo44()
        {
            IEnumerable<int> items = Enumerable.Range(0, 10);
            foreach (int item in items)
            {
            }

            items = null;
        }

        private static void Foo5()
        {
            string x = null;

            string s2 = "";
            x = s2;
            x = s2;
        }
    }
#pragma warning restore RCS1118
}
