// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal static class GenerateEnumMemberRefactoring
    {
        [Flags]
        public enum Foo
        {
            None = 0,
            Alpha = 1,
            Beta = 2,
            Gamma = 4,
        }
    }
}
