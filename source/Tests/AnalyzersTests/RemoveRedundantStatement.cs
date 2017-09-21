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
        }

        private static IEnumerable<object> Foo3()
        {
            yield return null;

            yield break;
        }

        private static IEnumerable<object> Foo4()
        {
            bool f = false;

            yield return null;

            if (f)
            {
            }
            else
            {
                yield break;
            }
        }

        //n

        private static string Foo5()
        {
            return;
        }

        private static string Foo6()
        {
            bool f = false;

            if (f)
            {
            }
            else
            {
                return;
            }
        }

        private static IEnumerable<object> Foo7()
        {
            yield break;
        }

        private static IEnumerable<object> Foo8()
        {
            yield break;
        }
    }
}
