// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#pragma warning disable CS0219, RCS1016, RCS1036, RCS1060, RCS1081, RCS1118

using System;
using System.Runtime.InteropServices;

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal partial class UseAutoProperty
    {
        private string _property3 = "";

        public string Property3
        {
            get { return _property3; }
            set { _property3 = value; }
        }
    }

    internal partial class UseAutoProperty
    {
        private string _property = "";
        private string _property2 = null;
        private static string _staticProperty = "";

        static UseAutoProperty()
        {
            string ReadOnlyStaticProperty = null;

            // UseAutoProperty.ReadOnlyStaticProperty
            _readOnlyStaticProperty = null;
            _staticProperty = null;

            UseAutoProperty._readOnlyStaticProperty = null;
        }

        public UseAutoProperty()
        {
            string Property = null;

            //this.Property
            _property = null;
            this._property2 = null;
            _property3 = null;
            _staticProperty = null;
            _readOnlyProperty = null;
            _readOnlyProperty2 = null;

            var x = new UseAutoProperty();
            x._property = null;
            UseAutoProperty._staticProperty = null;
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

        public string @string
        {
            get => _property2;
            set => _property2 = value;
        }

        public static string StaticProperty
        {
            get { return _staticProperty; }
            set { _staticProperty = value; }
        }

        public string ReadOnlyProperty
        {
            get { return _readOnlyProperty; }
        }

        public string ReadOnlyProperty2 => _readOnlyProperty2;

        public static string ReadOnlyStaticProperty
        {
            get { return _readOnlyStaticProperty; }
        }

        private readonly string _readOnlyProperty = "", _value;
        private readonly string _readOnlyProperty2;
        private static readonly string _readOnlyStaticProperty;

        private static class FooOverride
        {
            private class FooBase
            {
                public virtual bool Value { get; set; }
            }

            private class FooDerived : FooBase
            {
                private bool _value;

                public override bool Value
                {
                    get { return _value; }
                    set { _value = value; }
                }
            }
        }

        //n

        private static class FooOverrideWithNotImplementedAccessor
        {
            private class FooBase
            {
                public virtual bool Value { get; set; }
                public virtual bool Value2 { get; set; }
            }

            private class FooDerived : FooBase
            {
                private readonly bool _value;
                private readonly bool _value2;

                public override bool Value => _value;

                public override bool Value2
                {
                    get { return _value2; }
                }
            }
        }

        private string _propertyRef = "";
        private string _propertyOut = "";

        public string PropertyRef
        {
            get { return _propertyRef; }
            set { _propertyRef = value; }
        }

        public string PropertyOut
        {
            get { return _propertyOut; }
            set { _propertyOut = value; }
        }

        private string RefMethod(ref string p1)
        {
            RefMethod(ref _propertyRef);

            return p1;
        }

        private bool OutMethod(out string p1)
        {
            p1 = null;
            return false;
        }

        private class UseAutoPropertyRefOut
        {
            private string _propertyRef = "";
            private string _propertyOut = "";

            public string PropertyRef
            {
                get { return _propertyRef; }
                set { _propertyRef = value; }
            }

            public string PropertyOut
            {
                get { return _propertyOut; }
                set { _propertyOut = value; }
            }

            internal void VoidMethod()
            {
                RefMethod(ref _propertyRef);
                OutMethod(out _propertyOut);
            }

            private string RefMethod(ref string p1)
            {
                return p1;
            }

            private bool OutMethod(out string p1)
            {
                p1 = null;
                return false;
            }
        }

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

        private class FooFieldWithNonSerializedAttribute
        {
            [NonSerialized]
            private string _value;

            public string Value
            {
                get { return _value; }
                set { _value = value; }
            }
        }
    }
}
