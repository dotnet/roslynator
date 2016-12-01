// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal static class UseExpressionBodiedMemberRefactoring
    {
        private class Entity
        {
            public string FooMethod()
            {
                return null;
            }

            public void FooVoidMethod()
            {
                FooMethod();
            }

            public string FooProperty
            {
                get { return string.Empty; }
            }

            public string this[int index]
            {
                get { return null; }
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
