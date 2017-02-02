// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Roslynator.CSharp.Analyzers.Tests
{
#pragma warning disable RCS1111
    public static class RemoveUnreachableCode
    {
        private static void Foo()
        {
            var regexOptions = RegexOptions.None;

            switch (regexOptions)
            {
                case RegexOptions.CultureInvariant:
                    break;
                case RegexOptions.ECMAScript:
                    {
                        return;
                        break;
                    }
                case RegexOptions.ExplicitCapture:
                    Foo();
                    return;
                    break;
            }
        }
    }
}
