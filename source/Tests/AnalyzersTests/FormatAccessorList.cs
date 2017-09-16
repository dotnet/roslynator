// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#pragma warning disable RCS1016, RCS1085

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal class FormatAccessorList
    {
        private string _property;
        private string _property2;
        private readonly string _readOnlyProperty;
        private readonly string _readOnlyProperty2;

        public string Property { get { return _property; } set { _property = value; } }

        public string Property2 { get => _property2; set => _property2 = value; }

        public string ReadOnlyProperty { get { return _readOnlyProperty; } }

        public string ReadOnlyProperty2 { get => _readOnlyProperty2; }

        public string ReadOnlyAutoProperty
        {
            get;
        }

        public string AutoProperty
        {
            get;
            set;
        }

        private interface Foo
        {
            string this[int index]
            {
                get;
                set;
            }

            string this[long index]
            {
                get;
            }
        }
    }
}
