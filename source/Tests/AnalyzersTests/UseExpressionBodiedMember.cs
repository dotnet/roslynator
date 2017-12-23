// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System;

#pragma warning disable CS0168, RCS1079, RCS1085, RCS1163

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class UseExpressionBodiedMember
    {
        private class Foo
        {
            public Foo()
            {
                FooMethod();
            }

            ~Foo()
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
                get { return ""; }
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

            public static explicit operator Foo(string value)
            {
                return new Foo();
            }

            public static explicit operator string(Foo value)
            {
                return "";
            }
        }

        private class FooThrow
        {
            public FooThrow()
            {
                throw new NotImplementedException();
            }

            ~FooThrow()
            {
                throw new NotImplementedException();
            }

            public string FooMethod()
            {
                throw new NotImplementedException();
            }

            public void FooVoidMethod()
            {
                throw new NotImplementedException();
            }

            public void MethodWithLocalFunction()
            {
                object LocalFunction()
                {
                    throw new NotImplementedException();
                }
            }

            public void MethodWithVoidLocalFunction()
            {
                void LocalFunction()
                {
                    throw new NotImplementedException();
                }
            }

            public string FooProperty
            {
                get { throw new NotImplementedException(); }
            }

            private readonly string _fooProperty2;

            public string FooProperty2
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public string this[int index]
            {
                get { throw new NotImplementedException(); }
            }

            public string this[string index]
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public static explicit operator FooThrow(string value)
            {
                throw new NotImplementedException();
            }

            public static explicit operator string(FooThrow value)
            {
                throw new NotImplementedException();
            }
        }

        //n

        private class Foo2
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
                    return "";
                }
            }

            public string FooProperty2
            {
                [DebuggerStepThrough]
                get { return ""; }
            }

            public string this[int index]
            {
                get
                {
                    Foo();
                    return null;
                }
            }

            public static explicit operator Foo2(string value)
            {
                Foo();
                return new Foo2();
            }

            public static explicit operator string(Foo2 value)
            {
                Foo();
                return "";
            }

            private static void Foo()
            {
            }
        }
    }
}
