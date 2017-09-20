// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class InsertInterpolationIntoStringLiteralRefactoring
    {
        public void Foo(string s)
        {
            s = "x\tx\u1234x\U00001234x\x9x\x09x\x009x{}xFoox\u0046oo";
            s = @"x\tx\u1234x\U00001234x\x9x\x09x\x009x{}x""xFoox\u0046oo";

            s = $"x\tx\u1234x\U00001234x\x9x\x09x\x009x{{}}x{s}xFoox\u0046oo";
            s = $@"x\tx\u1234x\U00001234x\x9x\x09x\x009x{{}}x""x{s}xFoox\u0046oo";
        }
    }
}
