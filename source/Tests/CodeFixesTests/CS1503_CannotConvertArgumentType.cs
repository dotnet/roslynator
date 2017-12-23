// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    internal static class CS1503_CannotConvertArgumentType
    {
        public static void Foo(StringBuilder sb)
        {
            Foo(GetStringBuilder);

            Foo(CS1503_CannotConvertArgumentType.GetStringBuilder);
        }

        public static StringBuilder GetStringBuilder()
        {
            return new StringBuilder();
        }
    }
}
