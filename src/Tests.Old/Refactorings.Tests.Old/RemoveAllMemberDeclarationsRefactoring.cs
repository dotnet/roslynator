// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class RemoveAllMemberDeclarationsRefactoring
    {
        public class Entity
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }
    }
}
