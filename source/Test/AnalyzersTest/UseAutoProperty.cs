// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#pragma warning disable RCS1016, RCS1081

namespace Roslynator.CSharp.Analyzers.Test
{
    internal partial class UseAutoProperty
    {
        private string _property = "";

        public UseAutoProperty()
        {
            _property = null;
            _readOnlyProperty = null;
            _readOnlyProperty2 = null;
        }

        public void Foo()
        {
            _property = null;
            _property2 = null;
        }

        public string Property
        {
            get { return _property; }
            set { _property = value; }
        }

        public string ReadOnlyProperty
        {
            get { return _readOnlyProperty; }
        }

        public string ReadOnlyProperty2 => _readOnlyProperty2;

        private readonly string _readOnlyProperty = "", _value;
        private readonly string _readOnlyProperty2;
    }
}
