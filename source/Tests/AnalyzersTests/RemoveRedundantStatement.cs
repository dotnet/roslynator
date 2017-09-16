// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

#pragma warning disable CS0168, RCS1002, RCS1118, RCS1176, RCS1177

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class RemoveRedundantStatement
    {
        private static void Foo()
        {
            bool f = false;

            do
            {
                Foo();
                continue;

            } while (f);

            while (f)
            {
                Foo();
                continue;
            }

            var items = new List<string>();

            for (int i = 0; i < items.Count; i++)
            {
                Foo();
                continue;
            }

            foreach (string item in items)
            {
                Foo();
                continue;
            }

            do
            {
                Foo();

                if (f)
                {
                }
                else
                {
                    continue;
                }

            } while (f);

            while (f)
            {
                Foo();

                if (f)
                {
                }
                else
                {
                    continue;
                }
            }

            for (int i = 0; i < items.Count; i++)
            {
                Foo();

                if (f)
                {
                }
                else
                {
                    continue;
                }
            }

            foreach (string item in items)
            {
                Foo();

                if (f)
                {
                }
                else
                {
                    continue;
                }
            }

            return;

            void LocalFunction()
            {
                return;
            }
        }

        private static void Foo2()
        {
            bool f = false;

            if (f)
            {
            }
            else
            {
                return;
            }

            void LocalFunction()
            {
                if (f)
                {
                }
                else
                {
                    return;
                }
            }
        }

        private static IEnumerable<object> Foo3()
        {
            yield return null;

            IEnumerable<object> LocalFunction()
            {
                yield return null;
                yield break;
            }

            yield break;

            IEnumerable<object> LocalFunction2()
            {
                yield return null;
                yield break;
            }
        }

        private static IEnumerable<object> Foo4()
        {
            bool f = false;

            yield return null;

            IEnumerable<object> LocalFunction()
            {
                yield return null;

                if (f)
                {
                }
                else
                {
                    yield break;
                }
            }

            if (f)
            {
            }
            else
            {
                yield break;
            }

            IEnumerable<object> LocalFunction2()
            {
                yield return null;

                if (f)
                {
                }
                else
                {
                    yield break;
                }
            }
        }

        //n

        private static IEnumerable<object> Foo5()
        {
            yield break;

            IEnumerable<object> LocalFunction()
            {
                yield break;
            }
        }

        private static IEnumerable<object> Foo6()
        {
            IEnumerable<object> LocalFunction()
            {
                yield break;
            }

            yield break;
        }
    }
}
