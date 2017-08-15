// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#pragma warning disable CS0168, RCS1002, RCS1049, RCS1090, RCS1118, RCS1176, RCS1187

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Roslynator.CSharp.Analyzers.Test
{
    internal static class ReduceIfNesting
    {
        private static readonly bool _condition;

        private static void Foo()
        {
            if (_condition)
            {
                Foo();

                if (!_condition)
                {
                    Foo2();

                    if (_condition)
                    {
                        Foo3();
                    }
                }
            }

            void FooLocal()
            {
                if (_condition)
                {
                    Foo();

                    if (!_condition)
                    {
                        Foo2();

                        if (_condition)
                        {
                            Foo3();
                        }
                    }
                }
            }
        }

        private static IEnumerable FooYield()
        {
            if (_condition)
            {
                yield return "1";

                if (!_condition)
                {
                    yield return "2";

                    if (_condition)
                    {
                        yield return "3";
                    }
                }
            }

            IEnumerable FooYieldLocal()
            {
                if (_condition)
                {
                    yield return "1";

                    if (!_condition)
                    {
                        yield return "2";

                        if (_condition)
                        {
                            yield return "3";
                        }
                    }
                }
            }
        }

        private static IEnumerable<object> FooYield2()
        {
            if (_condition)
            {
                yield return "1";

                if (!_condition)
                {
                    yield return "2";

                    if (_condition)
                    {
                        yield return "3";
                    }
                }
            }

            IEnumerable<object> FooYieldLocal()
            {
                if (_condition)
                {
                    yield return "1";

                    if (!_condition)
                    {
                        yield return "2";

                        if (_condition)
                        {
                            yield return "3";
                        }
                    }
                }
            }
        }

        private static async Task FooAsync()
        {
            if (_condition)
            {
                await FooAsync();

                if (!_condition)
                {
                    await Foo2Async();

                    if (!_condition)
                    {
                        await Foo3Async();
                    }
                }
            }

            async Task FooLocalAsync()
            {
                if (_condition)
                {
                    await FooAsync();

                    if (!_condition)
                    {
                        await Foo2Async();

                        if (!_condition)
                        {
                            await Foo3Async();
                        }
                    }
                }
            }
        }

        private static void Foo2()
        {
        }

        private static void Foo3()
        {
        }

        private static async Task Foo2Async()
        {
            await Foo2Async();
        }

        private static async Task Foo3Async()
        {
            await Foo3Async();
        }
    }
}
