// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class IntroduceAndInitializeRefactoring
    {
        public class Entity
        {
            public Entity(string value = null)
            {
            }

            public Entity(string value2, int id, int @object)
            {
                if (value2 == null)
                    throw new ArgumentNullException(nameof(value2));
            }

            static Entity(string value)
            {
            }
        }
    }
}
