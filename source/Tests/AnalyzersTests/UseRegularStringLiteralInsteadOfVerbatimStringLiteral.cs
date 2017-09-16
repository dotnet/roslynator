// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#pragma warning disable RCS1062, RCS1176

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class UseRegularStringLiteralInsteadOfVerbatimStringLiteral
    {
        private static string Foo()
        {
            string s = "";

            s = @"";
            s = @"s";

            s = $@"";
            s = $@"s{s}s";

            // n

            s = @" \ ";
            s = @" "" ";
            s = @"
";

            s = $@" \ {s}";
            s = $@"{s} "" ";
            s = $@"{s}
";

            s = $@"s{
                s}s";

            return s;
        }
    }
}
