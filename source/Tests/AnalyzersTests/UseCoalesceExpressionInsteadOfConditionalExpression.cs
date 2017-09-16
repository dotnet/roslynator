// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;

#pragma warning disable RCS1051, RCS1118, RCS1176

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class UseCoalesceExpressionInsteadOfConditionalExpression
    {
        public static void Foo()
        {
            string s = "";

            int i = 0;

            int? ni = null;

            s = s == null ? "" : s;

            s = s != null ? s : "";

            s = (s == null) ? ("") : (s);

            s = (s != null) ? (s) : ("");

            i = (ni == null) ? 1 : ni.Value;

            i = (ni != null) ? ni.Value : 1;

            i = (!ni.HasValue) ? 1 : ni.Value;

            i = (ni.HasValue) ? ni.Value : 1;
        }

        //n

        public static unsafe void Foo2()
        {
            string s = "";

            s = (s != null) ? "" : s;

            s = (s == null) ? s : "";

            int* i = null;

            i = (i == null) ? default(int*) : i;

            i = (i != null) ? i : default(int*);
        }
    }
}
