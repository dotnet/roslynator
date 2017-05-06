// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#pragma warning disable RCS1016, RCS1081, RCS1118

namespace Roslynator.CSharp.Analyzers.Test
{
    internal partial class UseAutoProperty
    {
        private string _property = "";
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
    }
}
