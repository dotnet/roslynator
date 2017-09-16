// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

#pragma warning disable RCS1078, RCS1118

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class UseStringLengthInsteadOfComparisonWithEmptyString
    {
        private static void Foo()
        {
            string s = null;

            if (s == "") { }

            if (s == string.Empty) { }

            if ("" == s) { }

            if (string.Empty == s) { }
        }
    }
}
