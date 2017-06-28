// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.CodeFixes.Test
{
    internal static class Unsafe
    {
        private static void Foo(char* p)
        {
            fixed (char* value = "")
            {
            }

            p = default(char*);

            while (*p != '\0')
            {
            }

            char ch = '\0';
            Foo(&ch);

            var block = stackalloc int[100];
        }

        delegate int FooDelegate(void* a, int b);
    }
}
