// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text.RegularExpressions;

namespace Roslynator.CSharp.Analyzers.Test
{
    internal static class AddBreakStatementToSwitchSection
    {
        public static void Foo()
        {
            var options = RegexOptions.None;

            switch (options)
            {
                case RegexOptions.RightToLeft:
                    {
                        break;
                    }
                case RegexOptions.CultureInvariant:
                    Foo();
                case RegexOptions.ECMAScript:
                case RegexOptions.ExplicitCapture:
                    Foo();
                    Foo();
                case RegexOptions.Singleline:
                    {
                    }
                case RegexOptions.IgnorePatternWhitespace:
                case RegexOptions.Multiline:
                    {
                    }
            }
        }
    }
}
