// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

#pragma warning disable RCS1002, RCS1007, RCS1016, RCS1040, RCS1098, RCS1163

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class SimplifyLazyInitialization
    {
        private class Foo
        {
            private object _value;
            private readonly Foo _foo;

            public object FooMethod()
            {
                if (_value == null)
                {
                    _value = Initialize();
                }

                return _value;
            }

            public object FooMethod2()
            {
                if (_value == null)
                    _value = Initialize();

                return _value;
            }

            public object FooMethod3()
            {
                if (null ==_value)
                {
                    _value = Initialize();
                }

                return _value;
            }

            public object FooMethod4()
            {
                if (this._value == null)
                {
                    this._value = Initialize();
                }

                return this._value;
            }

            public object FooMethod44()
            {
                if (_foo._value == null)
                {
                    _foo._value = Initialize();
                }

                return _foo._value;
            }

            public object FooMethod5()
            {
                if (_value == null)
                {
                    _value = new object[]
                    {
                        null
                    };
                }

                return _value;
            }

            public object FooProperty
            {
                get
                {
                    if (_value == null)
                        _value = Initialize();

                    return _value;
                }
            }

            public object this[int index]
            {
                get
                {
                    if (_value == null)
                    {
                        _value = Initialize();
                    }

                    return _value;
                }
            }

            private object Initialize()
            {
                return new object();
            }
        }

        private class FooNullable
        {
            private int? _value;

            public int? FooMethod()
            {
                if (!_value.HasValue)
                {
                    _value = Initialize();
                }

                return _value;
            }

            public int? FooMethod2()
            {
                if (_value == null)
                {
                    _value = Initialize();
                }

                return _value;
            }

            public int? FooProperty
            {
                get
                {
                    if (!_value.HasValue)
                        _value = Initialize();

                    return _value;
                }
            }

            public int? this[int index]
            {
                get
                {
                    if (!_value.HasValue)
                    {
                        _value = Initialize();
                    }

                    return _value;
                }
            }

            private int Initialize()
            {
                return 0;
            }
        }

        private class FooNullable2
        {
            private int? _value;
            private readonly FooNullable2 _foo;

            public int FooMethod()
            {
                if (_value == null)
                {
                    _value = Initialize();
                }

                return _value.Value;
            }

            public int FooMethod2()
            {
                if (this._value == null)
                {
                    this._value = Initialize();
                }

                return this._value.Value;
            }

            public int FooMethod22()
            {
                if (_foo._value == null)
                {
                    _foo._value = Initialize();
                }

                return _foo._value.Value;
            }

            public int? FooProperty
            {
                get
                {
                    if (_value == null)
                        _value = Initialize();

                    return _value.Value;
                }
            }

            public int? this[int index]
            {
                get
                {
                    if (_value == null)
                    {
                        _value = Initialize();
                    }

                    return _value.Value;
                }
            }

            private int Initialize()
            {
                return 0;
            }
        }

        // n

        private class Foo2
        {
            private object _value;

            public object LazyMethod()
            {
                if (_value == null)
                {
                    _value = Initialize();
                }
                else
                {
                }

                return _value;
            }

            public object LazyProperty
            {
                get
                {
                    if (_value == null)
                    {
                        _value = Initialize();
                    }
                    else
                    {
                    }

                    return _value;
                }
            }

            public object this[int index]
            {
                get
                {
                    if (_value == null)
                    {
                        _value = Initialize();
                    }
                    else
                    {
                    }

                    return _value;
                }
            }

            private object Initialize()
            {
                return new object();
            }
        }
    }
}
