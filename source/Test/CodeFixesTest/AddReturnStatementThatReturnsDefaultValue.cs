// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Roslynator.CSharp.CodeFixes.Test
{
    internal static class AddReturnStatementThatReturnsDefaultValue
    {
        private class Foo
        {
            public string MethodName()
            {
            }

            public static int? MethodNullable()
            {
            }

            public static DateTime MethodStruct()
            {
            }

            public static string MethodWithStatement()
            {
                string s = null;
            }

            public static int MethodInt32()
            {
            }

            public string PropertyName
            {
                get { }
            }

            public string this[int index]
            {
                get { }
            }

            public void Bar()
            {
                var items = Enumerable.Empty<string>();

                items = items.Select(f =>
                {
                    bool condition = false;

                    if (condition)
                    {
                        return f;
                    }
                });

                items = items.Select<string, string>(delegate (string f)
                {
                    bool condition = false;

                    if (condition)
                    {
                        return f;
                    }
                });

                string LocalFunction()
                {
                }
            }

            public static explicit operator Foo(string value)
            {
            }

            public static Foo operator !(Foo value)
            {
            }
        }

        //n

        public static void MethodVoid()
        {
        }

        public static IEnumerable MethodEnumerable()
        {
        }

        public static IEnumerable<object> MethodEnumerableOfT()
        {
        }

        public static xxx MethodErrorType()
        {
        }

        public static int MethodExpressionBody() => ;
    }
}
