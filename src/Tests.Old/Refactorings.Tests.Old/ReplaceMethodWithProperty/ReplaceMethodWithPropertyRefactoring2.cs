// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class ReplaceMethodWithPropertyRefactoring2
    {
        public ReplaceMethodWithPropertyRefactoring2()
        {
            var x = new ReplaceMethodWithPropertyRefactoring();

            var a = x.GetValue();
            var b = x.GetValue();

            var s = x.Foo();
        }
    }
}
