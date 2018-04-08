// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#pragma warning disable RCS1002, RCS1016, RCS1023, RCS1074, RCS1076, RCS1085, RCS1106, RCS1163

using System;
using System.Collections.Generic;

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class AvoidSingleLineBlock
    {
        public class Foo
        {
            private string _value;
            private readonly List<string> _items;

            public Foo() { Bar(); }

            ~Foo() { Bar(); }

            public void Bar(object parameter = null) { if (parameter == null) { throw new ArgumentNullException(nameof(parameter)); } }

            public static explicit operator Foo(string value) { return new Foo(); }

            public static explicit operator string(Foo value) { return null; }

            public static Foo operator !(Foo value) { return new Foo(); }

            public event EventHandler EventName { add { Bar(); } remove { Bar(); } }

            public string Value { get { return _value; } set { _value = value; } }

            public string this[int index] { get { return _items[index]; } set { _items[index] = value; } }
        }

        public class Foo2
        {
            private string _propertyName;
            private readonly List<string> _items;

            public Foo2() { }

            ~Foo2() { }

            public event EventHandler EventName
            {
                add { Bar(); }
                remove { Bar(); }
            }

            public string PropertyName
            {
                get { return _propertyName; }
                set { _propertyName = value; }
            }

            public string this[int index]
            {
                get { return _items[index]; }
                set { _items[index] = value; }
            }

            public void Bar()
            {
                Action<object> action = f => { };
                Action<object> action2 = (f) => { };
                Action<object> action3 = delegate { };
            }

            public static explicit operator Foo2(string value)
            {
                return new Foo2();
            }

            public static explicit operator string(Foo2 value)
            {
                return null;
            }

            public static Foo2 operator !(Foo2 value)
            {
                return new Foo2();
            }

            public enum EnumName { }

            public interface InterfaceName { }

            public struct StructName { }

            public class ClassName { }
        }
    }
}
