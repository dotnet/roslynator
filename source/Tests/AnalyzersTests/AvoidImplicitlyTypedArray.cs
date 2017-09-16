// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class AvoidImplicitlyTypedArray
    {
        public static void MethodName()
        {
            var items = new[] { "" };
            var items2 = new[] { new { Value = "" } };
            var items3 = new[] { abcde };
        }

        private static readonly string[][] _values = new[]
        {
            /**/new[] { "" },
        };
    }
}
