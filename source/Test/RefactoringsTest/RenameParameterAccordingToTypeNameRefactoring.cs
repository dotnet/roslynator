// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Test
{
    internal class RenameParameterAccordingToTypeNameRefactoring
    {
        public void Foo(Entity value)
        {
        }

        public void Foo2(Entity value)
        {
            Entity entity = value;
        }

        public void Foo(Entity entity, Entity value)
        {
            entity = value;
        }

        public class Entity
        {
        }
    }
}
