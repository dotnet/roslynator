// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class UseExpressionBodiedMember
    {
        private class Entity
        {
            public Entity()
            {
                FooMethod();
            }

            ~Entity()
            {
                FooMethod();
            }

            public string FooMethod()
            {
                return null;
            }

            public void FooVoidMethod()
            {
                FooMethod();
            }

            public void MethodWithLocalFunction()
            {
                object LocalFunction()
                {
                    return null;
                }
            }

            public void MethodWithVoidLocalFunction()
            {
                void LocalFunction()
                {
                    LocalFunction();
                }
            }

            public string FooProperty
            {
                get { return string.Empty; }
            }

            private string _fooProperty2;

            public string FooProperty2
            {
                get { return _fooProperty2; }
                set { _fooProperty2 = value; }
            }

            public string this[int index]
            {
                get { return null; }
            }

            public string this[string index]
            {
                get { return _fooProperty2; }
                set { _fooProperty2 = value; }
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

        private class Entity2
        {
            public string FooMethod()
            {
                Foo();
                return null;
            }

            public void FooVoidMethod()
            {
                Foo();
                FooMethod();
            }

            public void FooVoidMethod2()
            {
                FooMethod(
                    );
            }

            public string FooProperty
            {
                get
                {
                    Foo();
                    return string.Empty;
                }
            }

            public string FooProperty2
            {
                [System.Obsolete]
                get
                {
                    return string.Empty;
                }
            }

            public string this[int index]
            {
                get
                {
                    Foo();
                    return null;
                }
            }

            public static explicit operator Entity2(string value)
            {
                Foo();
                return new Entity2();
            }

            public static explicit operator string(Entity2 value)
            {
                Foo();
                return string.Empty;
            }

            private static void Foo()
            {
            }
        }
    }
}
