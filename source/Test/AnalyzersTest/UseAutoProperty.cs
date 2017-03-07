// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analyzers.Test
{
#pragma warning disable RCS1016 // Use expression-bodied member.
    internal class UseAutoProperty
    {
        private string _property = null;
        private string _property2 = null;
        private readonly string _readOnlyProperty = null;

        public UseAutoProperty()
        {
            _property = null;
            _property2 = null;
            _readOnlyProperty = null;
        }

        private void Foo() => _property = null;

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
    }
#pragma warning restore RCS1016 // Use expression-bodied member.
}
