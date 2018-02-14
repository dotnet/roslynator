// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

#pragma warning disable CS8321, RCS1016, RCS1074, RCS1106, RCS1118, RCS1163, RCS1176, RCS1213

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class UnnecessaryUnsafeContext
    {
        public unsafe class Foo
        {
            public unsafe  Foo()
            {
                unsafe
                {
                    Method();
                }
            }

            unsafe ~Foo()
            {
                unsafe
                {
                    Method();
                }
            }

            public static unsafe void Method()
            {
                unsafe
                {
                    Method();
                }
            }

            public unsafe void MethodWithLocalFunction()
            {
                unsafe void LocalFunction()
                {
                    unsafe
                    {
                        Method();
                    }
                }
            }

            public static unsafe implicit operator Foo(string value)
            {
                unsafe
                {
                    Method();
                }

                return null;
            }

            public static unsafe Foo operator !(Foo value)
            {
                unsafe
                {
                    Method();
                }

                return value;
            }

            public unsafe delegate void FooDelegate();

            public unsafe event EventHandler Event;

            public unsafe event EventHandler Event2
            {
                add
                {
                    unsafe
                    {
                        Method();
                    }
                }

                remove { }
            }

            public static readonly unsafe Action Field = () =>
            {
                unsafe
                {
                    Method();
                }
            };

            public unsafe int this[int index]
            {
                get
                {
                    unsafe
                    {
                        Method();
                    }

                    return index;
                }
            }

            public unsafe string Property
            {
                get
                {
                    unsafe
                    {
                        Method();
                    }

                    return null;
                }
            }
        }

        public unsafe struct FooStruct
        {
            public static unsafe void Method()
            {
                unsafe
                {
                    Method();
                }
            }

            public unsafe void MethodWithLocalFunction()
            {
                unsafe void LocalFunction()
                {
                    unsafe
                    {
                        Method();
                    }
                }
            }

            public static unsafe implicit operator FooStruct(string value)
            {
                unsafe
                {
                    Method();
                }

                return null;
            }

            public static unsafe FooStruct operator !(FooStruct value)
            {
                unsafe
                {
                    Method();
                }

                return value;
            }

            public unsafe delegate void FooDelegate();

            public unsafe event EventHandler Event;

            public unsafe event EventHandler Event2
            {
                add
                {
                    unsafe
                    {
                        Method();
                    }
                }

                remove { }
            }

            public static readonly unsafe Action Field = () =>
            {
                unsafe
                {
                    Method();
                }
            };

            public unsafe int this[int index]
            {
                get
                {
                    unsafe
                    {
                        Method();
                    }

                    return index;
                }
            }

            public unsafe string Property
            {
                get
                {
                    unsafe
                    {
                        Method();
                    }

                    return null;
                }
            }
        }

        public unsafe interface IFoo
        {
            unsafe void Method();

            unsafe event EventHandler Event;

            unsafe int this[int index] { get; set; }

            unsafe string Property { get; }
        }

        // n

        private static class UnsafeClass
        {
            public unsafe class FooConstructor
            {
                public FooConstructor()
                {
                    char* pCh = null;
                }
            }

            public unsafe class FooMethod
            {
                public void Method()
                {
                    char* pCh = null;
                }
            }

            public unsafe class FooLocalFunction
            {
                public void MethodWithLocalFunction()
                {
                    void LocalFunction()
                    {
                        char* pCh = null;
                    }
                }
            }

            public unsafe class FooConversionOperator
            {
                public static implicit operator FooConversionOperator(string value)
                {
                    char* pCh = null;
                    return null;
                }
            }

            public unsafe class FooOperator
            {
                public static FooOperator operator !(FooOperator value)
                {
                    char* pCh = null;
                    return value;
                }
            }

            public unsafe class FooDestructor
            {
                ~FooDestructor()
                {
                    char* pCh = null;
                }
            }

            public unsafe class FooEvent
            {
                public event EventHandler Event2
                {
                    add { char* pCh = null; }
                    remove { }
                }
            }

            public unsafe class FooField
            {
                public readonly Action Field = () =>
                {
                    char* pCh = null;
                };
            }

            public unsafe class FooIndexer
            {
                public int this[int index]
                {
                    get
                    {
                        char* pCh = null;
                        return index;
                    }
                }
            }

            public unsafe class FooProperty
            {
                public string Property
                {
                    get
                    {
                        char* pCh = null;
                        return null;
                    }
                }
            }
        }

        private struct UnsafeStruct
        {
            public unsafe struct FooMethod
            {
                public void Method()
                {
                    char* pCh = null;
                }
            }

            public unsafe struct FooLocalFunction
            {
                public void MethodWithLocalFunction()
                {
                    void LocalFunction()
                    {
                        char* pCh = null;
                    }
                }
            }

            public unsafe struct FooConversionOperator
            {
                public static implicit operator FooConversionOperator(string value)
                {
                    char* pCh = null;
                    return null;
                }
            }

            public unsafe struct FooOperator
            {
                public static FooOperator operator !(FooOperator value)
                {
                    char* pCh = null;
                    return value;
                }
            }

            public unsafe struct FooEvent
            {
                public event EventHandler Event2
                {
                    add { char* pCh = null; }
                    remove { }
                }
            }

            public unsafe struct FooField
            {
                public static readonly Action Field = () =>
                {
                    char* pCh = null;
                };
            }

            public unsafe struct FooIndexer
            {
                public int this[int index]
                {
                    get
                    {
                        char* pCh = null;
                        return index;
                    }
                }
            }

            public unsafe struct FooProperty
            {
                public string Property
                {
                    get
                    {
                        char* pCh = null;
                        return null;
                    }
                }
            }
        }

        public class PointerType
        {
            public unsafe PointerType()
            {
                char* pCh = null;
            }

            public unsafe void Method()
            {
                char* pCh = null;
            }

            public void MethodWithLocalFunction()
            {
                unsafe void LocalFunction()
                {
                    char* pCh = null;
                }
            }

            public static unsafe implicit operator PointerType(string value)
            {
                char* pCh = null;
                return null;
            }

            public static unsafe PointerType operator !(PointerType value)
            {
                char* pCh = null;
                return value;
            }

            unsafe ~PointerType()
            {
                char* pCh = null;
            }

            public unsafe event EventHandler Event2
            {
                add { char* pCh = null; }
                remove { }
            }

            public readonly unsafe Action Field = () =>
            {
                char* pCh = null;
            };

            public unsafe int this[int index]
            {
                get
                {
                    char* pCh = null;
                    return index;
                }
            }

            public unsafe string Property
            {
                get
                {
                    char* pCh = null;
                    return null;
                }
            }
        }

        public class FixedStatement
        {
            public unsafe FixedStatement()
            {
                fixed (char* pStart = "")
                {
                }
            }

            public unsafe void Method()
            {
                fixed (char* pStart = "")
                {
                }
            }

            public void MethodWithLocalFunction()
            {
                unsafe void LocalFunction()
                {
                    fixed (char* pStart = "")
                    {
                    }
                }
            }

            public static unsafe implicit operator FixedStatement(string value)
            {
                fixed (char* pStart = "")
                {
                }

                return null;
            }

            public static unsafe FixedStatement operator !(FixedStatement value)
            {
                fixed (char* pStart = "")
                {
                }

                return value;
            }

            unsafe ~FixedStatement()
            {
                fixed (char* pStart = "")
                {
                }
            }

            public unsafe event EventHandler Event2
            {
                add
                {
                    fixed (char* pStart = "")
                    {
                    }
                }
                remove { }
            }

            public readonly unsafe Action Field = () =>
            {
                fixed (char* pStart = "")
                {
                }
            };

            public unsafe int this[int index]
            {
                get
                {
                    fixed (char* pStart = "")
                    {
                    }

                    return index;
                }
            }

            public unsafe string Property
            {
                get
                {
                    fixed (char* pStart = "")
                    {
                    }

                    return null;
                }
            }
        }

        public class PointerIndirectionExpression
        {
            private static readonly unsafe char* _pCh;

            public unsafe PointerIndirectionExpression()
            {
                char ch = *_pCh;
            }

            public unsafe void Method()
            {
                char ch = *_pCh;
            }

            public void MethodWithLocalFunction()
            {
                unsafe void LocalFunction()
                {
                    char ch = *_pCh;
                }
            }

            public static unsafe implicit operator PointerIndirectionExpression(string value)
            {
                char ch = *_pCh;
                return null;
            }

            public static unsafe PointerIndirectionExpression operator !(PointerIndirectionExpression value)
            {
                char ch = *_pCh;
                return value;
            }

            unsafe ~PointerIndirectionExpression()
            {
                char ch = *_pCh;
            }

            public unsafe event EventHandler Event2
            {
                add
                {
                    char ch = *_pCh;
                }
                remove { }
            }

            public readonly unsafe Action Field = () =>
            {
                char ch = *_pCh;
            };

            public unsafe int this[int index]
            {
                get
                {
                    char ch = *_pCh;
                    return index;
                }
            }

            public unsafe string Property
            {
                get
                {
                    char ch = *_pCh;
                    return null;
                }
            }
        }

        public class AddressOfExpression
        {
            private static readonly unsafe char* _pCh;
            private static readonly char _ch;

            public unsafe AddressOfExpression()
            {
                char ch = '\0';
                var pCh = &ch;
            }

            public unsafe void Method()
            {
                char ch = '\0';
                var pCh = &ch;
            }

            public void MethodWithLocalFunction()
            {
                unsafe void LocalFunction()
                {
                    char ch = '\0';
                    var pCh = &ch;
                }
            }

            public static unsafe implicit operator AddressOfExpression(string value)
            {
                char ch = '\0';
                var pCh = &ch;
                return null;
            }

            public static unsafe AddressOfExpression operator !(AddressOfExpression value)
            {
                char ch = '\0';
                var pCh = &ch;
                return value;
            }

            unsafe ~AddressOfExpression()
            {
                char ch = '\0';
                var pCh = &ch;
            }

            public unsafe event EventHandler Event2
            {
                add
                {
                    char ch = '\0';
                    var pCh = &ch;
                }
                remove { }
            }

            public readonly unsafe Action Field = () =>
            {
                char ch = '\0';
                var pCh = &ch;
            };

            public unsafe int this[int index]
            {
                get
                {
                    char ch = '\0';
                    var pCh = &ch;
                    return index;
                }
            }

            public unsafe string Property
            {
                get
                {
                    char ch = '\0';
                    var pCh = &ch;
                    return null;
                }
            }
        }

        public class StackAlloc
        {
            public unsafe StackAlloc()
            {
                var pCh = stackalloc char[100];
            }

            public unsafe void Method()
            {
                var pCh = stackalloc char[100];
            }

            public void MethodWithLocalFunction()
            {
                unsafe void LocalFunction()
                {
                    var pCh = stackalloc char[100];
                }
            }

            public static unsafe implicit operator StackAlloc(string value)
            {
                var pCh = stackalloc char[100];
                return null;
            }

            public static unsafe StackAlloc operator !(StackAlloc value)
            {
                var pCh = stackalloc char[100];
                return value;
            }

            unsafe ~StackAlloc()
            {
                var pCh = stackalloc char[100];
            }

            public unsafe event EventHandler Event2
            {
                add
                {
                    var pCh = stackalloc char[100];
                }
                remove { }
            }

            public readonly unsafe Action Field = () =>
            {
                var pCh = stackalloc char[100];
            };

            public unsafe int this[int index]
            {
                get
                {
                    var pCh = stackalloc char[100];
                    return index;
                }
            }

            public unsafe string Property
            {
                get
                {
                    var pCh = stackalloc char[100];
                    return null;
                }
            }
        }

        public class PointerMemberAccessExpression
        {
            private static unsafe Unsafe* _p;

            public unsafe PointerMemberAccessExpression()
            {
                _p->Value = 25;
            }

            public unsafe void Method()
            {
                _p->Value = 25;
            }

            public void MethodWithLocalFunction()
            {
                unsafe void LocalFunction()
                {
                    _p->Value = 25;
                }
            }

            public static unsafe implicit operator PointerMemberAccessExpression(string value)
            {
                _p->Value = 25;
                return null;
            }

            public static unsafe PointerMemberAccessExpression operator !(PointerMemberAccessExpression value)
            {
                _p->Value = 25;
                return value;
            }

            unsafe ~PointerMemberAccessExpression()
            {
                _p->Value = 25;
            }

            public unsafe event EventHandler Event2
            {
                add
                {
                    _p->Value = 25;
                }
                remove { }
            }

            public readonly unsafe Action Field = () => _p->Value = 25;

            public unsafe int this[int index]
            {
                get
                {
                    _p->Value = 25;
                    return index;
                }
            }

            public unsafe string Property
            {
                get
                {
                    _p->Value = 25;
                    return null;
                }
            }
        }

        private static class UnsafeStatement
        {
            public static unsafe void Foo()
            {
                char* pCh = null;

                char ch = *pCh;

                unsafe
                {
                    // pointer type
                    char* pCh2 = null;
                }

                unsafe
                {
                    // fixed statement
                    fixed (char* pStart = "")
                    {
                    }
                }

                unsafe
                {
                    // pointer indirection expression
                    char ch2 = *pCh;
                }

                unsafe
                {
                    // addressof expression
                    pCh = &ch;
                }

                unsafe
                {
                    // stackalloc array creation
                    char* pStart = stackalloc char[100];
                }

                unsafe
                {
                    var x = default(Unsafe);
                    var px = &x;

                    // pointer member access expression
                    px->Value = 25;
                }
            }
        }

        private unsafe struct Unsafe
        {
            public static void Foo()
            {
                // pointer type
                char* pCh = null;

                // fixed statement
                fixed (char* pStart = "")
                {
                }

                // pointer indirection expression
                char ch = *pCh;

                // addressof expression
                pCh = &ch;

                // stackalloc array creation
                char* pStart2 = stackalloc char[100];

                var x = default(Unsafe);
                var px = &x;

                // pointer member access expression
                px->Value = 25;
            }

            public int Value { get; set; }
        }
    }
}
