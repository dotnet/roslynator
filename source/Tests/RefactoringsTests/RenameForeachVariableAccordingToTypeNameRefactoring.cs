// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class RenameForEachVariableAccordingToTypeNameRefactoring
    {
        public void SomeMethod()
        {
            var entities = new List<Entity>();

            foreach (Entity item in entities)
            {
            }
        }

        private class Entity
        {
        }
    }
}
