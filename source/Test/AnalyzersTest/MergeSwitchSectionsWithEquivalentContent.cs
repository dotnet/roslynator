// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Roslynator.CSharp.Analyzers.Test
{
    public static class MergeSwitchSectionsWithEquivalentContent
    {
        private static void Foo()
        {
            var options = RegexOptions.None;

            switch (options)
            {
                case RegexOptions.CultureInvariant:
                    break;
                case RegexOptions.ECMAScript:
                    break;
                case RegexOptions.ExplicitCapture:
                    break;
                case RegexOptions.IgnoreCase:
                    break;
                case RegexOptions.IgnorePatternWhitespace:
                    break;
                case RegexOptions.Multiline:
                    break;
                case RegexOptions.None:
                    break;
                case RegexOptions.RightToLeft:
                    break;
                case RegexOptions.Singleline:
                    {
                        break;
                    }
                default:
                    break;
            }

            switch (options)
            {
                case RegexOptions.CultureInvariant:
                    {
                        break;
                    }
                case RegexOptions.ECMAScript:
                    break;
                case RegexOptions.ExplicitCapture:
                    {
                        Foo();
                        break;
                    }
            }

            switch (options)
            {
                case RegexOptions.CultureInvariant:
                    {
                        Foo();
                        break;
                    }
                case RegexOptions.ECMAScript:
                    break;
                case RegexOptions.ExplicitCapture:
                    {
                        break;
                    }
            }

            switch (options)
            {
                case RegexOptions.CultureInvariant:
                    {
                        Foo();
                        break;
                    }
                case RegexOptions.ECMAScript:
                    break;
                case RegexOptions.ExplicitCapture:
                    break;
                case RegexOptions.IgnorePatternWhitespace:
                    break;
                case RegexOptions.IgnoreCase:
                    {
                        Foo();
                        break;
                    }
            }
        }
    }
}
