// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Roslynator.CSharp.CodeFixes.Test
{
    internal static class RemoveReturnKeyword
    {
        public static void Foo()
        {
            if (true)
                return Foo();

            return Foo();
        }

        public static async Task FooAsync()
        {
            if (true)
                return Foo();

            return Foo();
        }
    }
}
