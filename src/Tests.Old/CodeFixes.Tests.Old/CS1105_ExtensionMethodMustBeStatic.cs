// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    internal static class CS1105_ExtensionMethodMustBeStatic
    {
        private void Foo(this string value)
        {
        }    
    }
}
