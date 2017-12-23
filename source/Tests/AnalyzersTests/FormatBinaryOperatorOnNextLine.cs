// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class FormatBinaryOperatorOnNextLine
    {
        public static void Foo()
        {
            bool x = false;
            bool y = false;
            bool z = false;

            if (x &&
                y &&
                z)
            {

            }
        }
    }
}
