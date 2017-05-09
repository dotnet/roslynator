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
            string x = "";

            string x1 = (x == null) ? "" : x;

            string x2 = (x != null) ? x : "";
        }

        public static void Foo2()
        {
            string x = "";

            string x1 = x == null ? "" : x;

            string x2 = x != null ? x : "";
        }

        public static void NullableType()
        {
            int? x = null;

            int? x1 = (x == null) ? default(int?) : x;

            int? x2 = (x != null) ? x : default(int?);
        }

        //n

        public static void Foo3()
        {
            string x = "";

            string x1 = (x != null) ? "" : x;

            string x2 = (x == null) ? x : "";
        }

        public static unsafe void PointerType()
        {
            int* x = null;

            int* x1 = (x == null) ? default(int*) : x;

            int* x2 = (x != null) ? x : default(int*);
        }
    }
}
