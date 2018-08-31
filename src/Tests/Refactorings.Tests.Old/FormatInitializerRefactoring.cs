// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class FormatInitializerRefactoring
    {
        private class Foo
        {
            private class Entity
            {
                public string Name { get; set; }
                public int Value { get; set; }
            }

            public void Bar()
            {
                var entity = new Entity() { Name = "Name", Value = 0 };

                var entity2 = new Entity()
                {
                    Name = "Name",
                    Value = 0
                };
            }

            // n

            public void Bar2()
            {
                var entity = new Entity()
                {
                    Name = "Name", //x
                    Value = 0
                };
            }
        }
    }
}
