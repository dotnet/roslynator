// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;

namespace Roslynator.CSharp.CodeFixes.Tests
{
    internal static class RemoveConstModifier
    {
        private static void Foo()
        {
            const StringBuilder sb = new StringBuilder();
        }
    }
}
