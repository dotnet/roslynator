// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#pragma warning disable CS0219, RCS1176

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class RemoveRedundantAssignment
    {
        private static bool Foo()
        {
            bool f = false;
            bool g = false;

            f = false;
            f = g;

            g = false;
            return g;
        }

        private static bool Foo(bool f, bool g)
        {
            f = false;
            f = g;

            g = false;
            return g;
        }

        //n

        private static bool Foo(out bool f, out bool g)
        {
            f = false;
            f = false;

            g = false;
            return g;
        }
    }
}
