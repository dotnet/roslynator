// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#pragma warning disable RCS1213

namespace Roslynator.CSharp.CodeFixes.Tests
{
    internal static class CS1751_CannotSpecifyDefaultValueForParameterArray
    {
        private static void Foo(params object[] values = null)
        {
        }
    }
}
