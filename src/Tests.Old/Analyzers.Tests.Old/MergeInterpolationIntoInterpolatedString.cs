// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class MergeInterpolationIntoInterpolatedString
    {
        public static void Foo()
        {
            string s = null;

            s = $"a{"b"}c";

            s = $@"a{@"b"}c";

            s = $"a{@"b"}c";

            s = $@"a{"b"}c";
        }
    }
}
