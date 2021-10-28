// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#pragma warning disable IDE0040, RCS1213

namespace Roslynator.CSharp.CodeFixes.Tests
{
    internal static class CS1519_InvalidTokenInClassStructOrInterfaceMemberDeclaration
    {
        private class Foo
        {
            public void void Bar()
            {
            }

            public void string Bar2()
            {
            }
        }
    }
}
