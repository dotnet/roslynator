// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

#pragma warning disable RCS1012

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class UseEventArgsEmpty
    {
        private static void Foo()
        {
            var x = new EventArgs();
        }
    }
}
