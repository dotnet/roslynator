// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class UseCompoundAssignment
    {
        private class Entity
        {
            public Entity Foo()
            {
                int i = 0;
                i = i + 10;
                i = i - 10;
                i = i * 10;
                i = i / 10;
                i = i % 10;
                i = i << 10;
                i = i >> 10;
                i = i | 10;
                i = i & 10;
                i = i ^ 10;

                Property = Property + 10;

                return new Entity() { Property = Property + 10 };
            }

            public int Property { get; set; }
        }
    }
}