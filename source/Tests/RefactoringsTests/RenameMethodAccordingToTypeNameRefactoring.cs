// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class RenameMethodAccordingToTypeNameRefactoring
    {

        public Entity GetValue()
        {








            return null;
        }

        public async Task<Entity> SomeMethod2()
        {
        }

        public async Task SomeMethod3()
        {
        }

        public async void SomeMethod3()
        {
        }

        public class Entity
        {
        }
    }
}
