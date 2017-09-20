// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class FormatInitializerRefactoring
    {
        private class FormatInitializerOnMultipleLinesRefactoring
        {
            public void SomeMethod()
            {
                var entity = new Entity() { Name = "Name", Value = 0 };
            }

            private class Entity
            {
                public string Name { get; set; }
                public int Value { get; set; }
            }
        }

        private class FormatInitializerOnSingleLineRefactoring
        {
            public void SomeMethod()
            {

                var entity = new Entity()
                {
                    Name = "Name",
                    Value = 0
                };
            }

            private class Entity
            {
                public string Name { get; set; }
                public int Value { get; set; }
            }
        }
    }
}
