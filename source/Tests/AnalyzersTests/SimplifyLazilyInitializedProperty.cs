// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

#pragma warning disable RCS1002, RCS1016, RCS1040, RCS1163

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class SimplifyLazilyInitializedProperty
    {
        public class Foo
        {
            private object _value;

            public object LazyMethod()
            {
                if (_value == null)
                {
                    _value = Initialize();
                }

                return _value;
            }

            public object LazyProperty
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

        public class FooNullable
        {
            private int? _value;

            public int? LazyMethod()
            {
                if (!_value.HasValue)
                {
                    _value = Initialize();
                }

                return _value;
            }

            public int? LazyProperty
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

        // n

        public class Foo2
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
