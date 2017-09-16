// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Text.RegularExpressions;

#pragma warning disable CS0168, CS0219, RCS1081

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class UseVarInsteadOfExplicitType
    {
        public static void Foo()
        {
            var items = new List<string>();
            List<string> items2 = items;

            foreach (string item in items)
            {
            }

            // N

            object o = "";
            const string c = "";
            string value1, value2;
            dynamic x = new object();
            dynamic x2 = c;

            foreach (Match match in Regex.Matches("input", "pattern"))
            {
            }
        }
    }
}
