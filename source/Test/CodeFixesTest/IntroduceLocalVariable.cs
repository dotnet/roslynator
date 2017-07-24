// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;

namespace Roslynator.CSharp.CodeFixes.Test
{
    internal static class IntroduceLocalVariable
    {
        private class Foo
        {
            private void Method()
            {
                Property;
            }

            public string Property { get; }
        }
    }
}
