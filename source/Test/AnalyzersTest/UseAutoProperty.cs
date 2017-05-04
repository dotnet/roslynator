// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#pragma warning disable RCS1016, RCS1081

using System.Runtime.InteropServices;

namespace Roslynator.CSharp.Analyzers.Test
{
    internal partial class UseAutoProperty
    {
        private string _property = "";

        public UseAutoProperty()
        {
            string Property = null;

            _property = null;
            this._property2 = null;
            _readOnlyProperty = null;
            _readOnlyProperty2 = null;

            var x = new UseAutoProperty();
            x._property = null;
        }

        public void Method()
        {
            _property = null;
            _property2 = null;
        }

        public string Property
        {
            get { return _property; }
            set { _property = value; }
        }

        public string Property2
        {
            get => _property2;
            set => _property2 = value;
        }

        public string ReadOnlyProperty
        {
            get { return _readOnlyProperty; }
        }

        public string ReadOnlyProperty2 => _readOnlyProperty2;

        private readonly string _readOnlyProperty = "", _value;
        private readonly string _readOnlyProperty2;

        //n

        private class Foo : IFoo
        {
            private string _property;

            string IFoo.Property
            {
                get { return _property; }
                set { _property = value; }
            }
        }

        private interface IFoo
        {
            string Property { get; set; }
        }

        [StructLayout(LayoutKind.Explicit)]
        private class FooClass
        {
            [FieldOffset(0)]
            private string _property;

            public string Property
            {
                get { return _property; }
                set { _property = value; }
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct FooStruct
        {
            [FieldOffset(0)]
            private string _property;

            public string Property
            {
                get { return _property; }
                set { _property = value; }
            }
        }
    }
}
