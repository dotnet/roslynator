// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal static class ReduceIfNesting
    {
        private static readonly bool _condition;
        private static readonly bool _condition2;
        private static readonly bool _condition3;

        private static void Foo()
        {
            if (_condition)
            {
                Foo();

                if (_condition2)
                {
                    Foo2();

                    if (_condition3)
                    {
                        Foo3();
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
    }
}
