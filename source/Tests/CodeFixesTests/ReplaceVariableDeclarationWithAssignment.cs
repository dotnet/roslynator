// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

#pragma warning disable CS0219

namespace Roslynator.CSharp.CodeFixes.Tests
{
    internal static class ReplaceVariableDeclarationWithAssignment
    {
        private static void Foo()
        {
            string s = null;

            string s = "";

            if (true)
            {
                string s = "";
            }
        }

        //n

        private static void Foo2()
        {
            if (true)
            {
                string s = "";
            }

            string s = null;
        }
    }
}
