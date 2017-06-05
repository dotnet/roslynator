// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text;

namespace Roslynator.CSharp.Refactorings.Test
{
    internal static class UseStringBuilderInsteadOfConcatenation
    {
        public static void Foo(string x)
        {
            var s = x + "x" + x + @"x" + $"{x}x" + $@"{x}x" + $"{x,1}x" + $"{x:f}x" + $"{x,1:f}x";

            s = x + "x";

            s += x + "x";

            while (true)
                s = x + "x";
        }
    }
}
