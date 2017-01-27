// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Roslynator.CSharp.Analyzers.Tests
{
#pragma warning disable RCS1002, RCS1016, RCS1118
    public static class ReplaceReturnWithYieldReturn
    {
        private static IEnumerable<string> Foo()
        {
            return "";
        }

        private static IEnumerable<string> Foo2()
        {
            string item = "";

            bool f = false;

            if (f)
                yield return item;

            if (f)
                return "";

            return Foo2();
        }

        private static IEnumerable Foo3()
        {
            string item = "";

            bool f = false;

            if (f)
                yield return item;

            if (f)
                return "";

            if (f)
                return Foo2();
        }
    }
#pragma warning restore RCS1002, RCS1016, RCS1118
}
