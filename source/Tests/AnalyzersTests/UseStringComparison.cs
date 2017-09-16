// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;

#pragma warning disable RCS1118

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class UseStringComparison
    {
        private static void Foo()
        {
            string s = null;
            string s2 = null;

            if (s.ToLower() == s2.ToLower()) { }

            if (s.ToUpper() == s2.ToUpper()) { }

            if (s.ToLower() != s2.ToLower()) { }

            if (s.ToUpper() != s2.ToUpper()) { }

            if (s.ToLower() == "x") { }

            if ("X" == s2.ToUpper()) { }

            if (string.Equals(s.ToLower(), s2.ToLower())) { }

            if (string.Equals(s.ToUpper(), s2.ToUpper())) { }

            if (string.Equals(s.ToLower(), "x")) { }

            if (string.Equals("X", s2.ToUpper())) { }

            if (s.ToLower().Equals(s2.ToLower())) { }

            if (s.ToUpper().Equals(s2.ToUpper())) { }

            if (s.ToLower().Equals("x")) { }

            if (s.ToUpper().Equals("X")) { }
        }
    }
}
