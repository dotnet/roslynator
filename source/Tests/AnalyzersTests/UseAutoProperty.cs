// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal class UseAutoProperty
    {
        private string _property = null;
        private readonly string _readOnlyProperty = null;

        public UseAutoProperty()
        {
            _property = null;
            _readOnlyProperty = null;
        }

        private void Foo() => _property = null;

        public string Property
        {
            get { return _property; }
            set { _property = value; }
        }

        public string ReadOnlyProperty
        {
            get { return _readOnlyProperty; }
        }
    }
}
