// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.CodeFixes.Test
{
    internal static class ChangeAccessibility
    {
        private sealed class SealedFoo
        {
            protected void Bar()
            {
            }

            public string Property { get; protected set; }
        }

        private static class StaticFoo
        {
            protected static void Bar()
            {
            }

            protected static void Bar2()
            {
            }
        }
    }
}
