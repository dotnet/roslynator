// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class UseExpressionBodiedMemberRefactoring
    {
        private class Entity
        {
            public string SomeMethod()
            {
                return null;
            }

            public string PropertyName
            {
                get { return string.Empty; }
            }

            public static explicit operator Entity(string value)
            {
                return new Entity();
            }

            public static explicit operator string(Entity value)
            {
                return string.Empty;
            }
        }
    }
}
