// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text.RegularExpressions;

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class UseBitwiseOperationInsteadOfCallingHasFlag
    {
        private static void Foo()
        {
            RegexOptions options = RegexOptions.None;

            if (options.HasFlag(RegexOptions.IgnoreCase))
            {
            }
        }
    }
}
