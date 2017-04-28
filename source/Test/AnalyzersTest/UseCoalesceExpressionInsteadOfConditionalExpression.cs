// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;

#pragma warning disable RCS1051, RCS1118, RCS1176

namespace Roslynator.CSharp.Analyzers.Test
{
    public static class UseCoalesceExpressionInsteadOfConditionalExpression
    {
        public static void Foo()
        {
            string s = "";

            string x = (s == null) ? "" : s;

            string x2 = (s != null) ? s : "";
        }

        public static void Foo2()
        {
            string s = "";

            string x = s == null ? "" : s;

            string x2 = s != null ? s : "";
        }

        public static void Foo3()
        {
            string s = "";

            string x = (s != null) ? "" : s;

            string x2 = (s == null) ? s : "";
        }
    }
}
