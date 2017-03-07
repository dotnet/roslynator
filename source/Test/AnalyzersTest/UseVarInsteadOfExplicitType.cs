// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text.RegularExpressions;

namespace Roslynator.CSharp.Analyzers.Test
{
    internal static class UseVarInsteadOfExplicitType
    {
        public static void Foo()
        {
            object o = new object();
            RegexOptions options = RegexOptions.None;
        }
    }
}
