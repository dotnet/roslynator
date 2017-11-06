// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#pragma warning disable CS0162, CS0219, RCS1118, RCS1176

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class RemoveRedundantAssignment
    {
        private static bool Foo()
        {
            bool f = false;
            bool g = false;

            f = false;
            return f;
        }

        private static bool Foo(bool f)
        {
            f = false;
            return f;
        }

        //n

        private static bool Foo(out bool f, out bool g)
        {
            g = false;

            f = false;
            return f;

            g = false;
            return g;
        }
    }
}
