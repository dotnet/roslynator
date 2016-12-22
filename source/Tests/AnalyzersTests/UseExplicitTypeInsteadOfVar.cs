// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class UseExplicitTypeInsteadOfVar
    {
        public static void Foo()
        {
            var a = "a";

            const var b = "c";

            var x = "x", y = "y", z = "y";
        }
    }
}
