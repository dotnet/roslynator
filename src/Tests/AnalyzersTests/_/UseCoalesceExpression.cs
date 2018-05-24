// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

#pragma warning disable RCS1002, RCS1016, RCS1176


namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class UseCoalesceExpression
    {
        private static class Foo
        {
            private static void IfStatement()
            {
                string x = GetValueOrDefault();

                // a
                if (x == null)
                {
                    // b
                    x = (true) ? "" : "";
                }

                string y = GetValueOrDefault();

                if (y == null)
                    y = "";
            }

            private static void IfStatement2()
            {
                Derived y = null;
                Derived2 z = null;

                Base x = GetValueOrDefault2();

                // a
                if (x == null)
                {
                    // b
                    x = y;
                }

                Base x2 = GetValueOrDefault2();

                if (x2 == null)
                    x2 = z;
            }

            private static void LocalDeclarationStatement()
            {
                string x = GetValueOrDefault();

                if (x == null)
                {
                    x = (true) ? "" : "";
                }

                string y = GetValueOrDefault();

                if (y == null)
                    y = "";
            }

            private static void LocalDeclarationStatement2()
            {
                Base x = GetValueOrDefault2();

                if (x == null)
                {
                    x = default(Derived2);
                }

                Base x2 = GetValueOrDefault2();

                if (x2 == null)
                    x2 = default(Derived2);
            }

            private static void ExpressionStatement()
            {
                string x = null;
                string y = null;

                x = GetValueOrDefault();

                if (x == null)
                {
                    x = (true) ? "" : "";
                }

                y = GetValueOrDefault();

                // ...
                if (y == null)
                    y = "";
            }

            private static void ExpressionStatement2()
            {
                Base x = null;
                Base x2 = null;

                x = GetValueOrDefault2();

                if (x == null)
                {
                    x = default(Derived2);
                }

                x2 = GetValueOrDefault2();

                // ...
                if (x2 == null)
                    x2 = default(Derived2);
            }

            private static string GetValueOrDefault()
            {
                return null;
            }

            private static Derived GetValueOrDefault2()
            {
                return null;
            }

            private class Base
            {
            }

            private class Derived : Base
            {
            }

            private class Derived2 : Base
            {
            }
        }

        private static class FooNullable
        {
            private static void IfStatement()
            {
                int? x = GetValueOrDefault();

                // a
                if (x == null)
                {
                    // b
                    x = (true) ? 1 : 2;
                }

                int? y = GetValueOrDefault();

                if (y == null)
                    y = 1;
            }

            private static void LocalDeclarationStatement()
            {
                int? x = GetValueOrDefault();

                if (x == null)
                {
                    x = (true) ? 1 : 2;
                }

                int? y = GetValueOrDefault();

                if (y == null)
                    y = 1;
            }

            private static void ExpressionStatement()
            {
                int? x = null;
                int? y = null;

                x = GetValueOrDefault();

                if (x == null)
                {
                    x = (true) ? 1 : 2;
                }

                y = GetValueOrDefault();

                // ...
                if (y == null)
                    y = 1;
            }

            private static int? GetValueOrDefault()
            {
                return 0;
            }
        }
    }
}
