// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class RemoveEnumMemberValueRefactoring
    {
        private enum Foo
        {
            None = 0,
            One = 1,
            Two,
            Three,
            Four = 4
        }
    }
}
