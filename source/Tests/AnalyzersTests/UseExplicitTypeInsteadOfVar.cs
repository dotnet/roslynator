// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

#pragma warning disable RCS1012

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class UseExplicitTypeInsteadOfVar
    {
        public static void Foo()
        {
            var a = "a";

            const var b = "c";

            var x = "x", y = "y", z = "y";

            string value = null;
            if (DateTime.TryParse(value, out var result))
            {
            }

            var items = new List<string>();
            items.Add("");

            foreach (var item in items)
            {
            }
        }
    }
}
