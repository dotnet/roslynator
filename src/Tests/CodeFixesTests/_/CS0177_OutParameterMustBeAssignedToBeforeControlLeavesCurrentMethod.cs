// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text.RegularExpressions;

#pragma warning disable CS0168

namespace Roslynator.CSharp.CodeFixes.Tests
{
    internal static class CS0177_OutParameterMustBeAssignedToBeforeControlLeavesCurrentMethod
    {
        private class Foo
        {
            void Bar(object p1, out object p2, out object p3)
            {
            }

            void Bar2(object p1, out object p2, out object p3)
            {
                p1 = null;
            }

            public bool Bar3(object p1, out object p2, out object p3)
            {
                return false;
            }

            public bool Bar4(bool p1, out object p2, out object p3)
            {
                p1 = false;

                if (p1)
                    return false;

                return false;
            }

            public bool Bar5(object p1, out object p2, out object p3)
            {
                p1 = null;
                p2 = null;
                return false;
            }

            public object Bar6(object p1, out object p2, out object p3, out object p4) => p1 = p2 = null;

            public void BarWithLocalFunction()
            {
                void Bar(object p1, out object p2, out object p3)
                {
                }

                void Bar2(object p1, out object p2, out object p3)
                {
                    p1 = null;
                }

                bool Bar3(object p1, out object p2, out object p3)
                {
                    return false;
                }

                bool Bar4(object p1, out object p2, out object p3)
                {
                    p1 = null;
                    return false;
                }

                bool Bar5(object p1, out object p2, out object p3)
                {
                    p1 = null;
                    p2 = null;
                    return false;
                }

                object Bar6(object p1, out object p2, out object p3, out object p4) => p1 = p2 = null;
            }

            // n

            public void BarWithLocalFunction2()
            {
                void LocalFunctionWithoutBody(object p1, out object p2, out object p3)
            }

            void BarWithoutBody(object p1, out object p2, out object p3)
        }
    }
}
