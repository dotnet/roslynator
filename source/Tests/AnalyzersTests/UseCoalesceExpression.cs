// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Roslynator.CSharp.Analyzers.Tests
{
#pragma warning disable RCS1002, RCS1016
    public static class UseCoalesceExpression
    {
        private static void Foo(string value, string value2)
        {
            // a
            if (value == null)
            {
                // b
                value = "";
            }

            if (value2 == null)
                value2 = "";

            string x = GetValueOrDefault();

            if (x == null)
            {
                x = "";
            }

            x = GetValueOrDefault();

            if (x == null)
            {
                x = "";
            }

            string y = GetValueOrDefault();

            if (y == null)
                y = "";

            y = GetValueOrDefault();

            // ...
            if (y == null)
                y = "";
        }

        private static void Foo(string value)
        {
            if (value == null)
            {
                value = "";
            }
        }

        private static void Foo2(string value)
        {
            if (value == null)
                value = "";
        }

        private static string GetValueOrDefault()
        {
            return null;
        }
    }
}
