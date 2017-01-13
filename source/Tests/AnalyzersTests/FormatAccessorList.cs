// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analyzers.Tests
{
#pragma warning disable RCS1016, RCS1085 // Use expression-bodied member.
    internal class AvoidMultilineExpre
    {
        private string _property = null;
        private string _property2 = null;
        private readonly string _readOnlyProperty = null;
        private readonly string _readOnlyProperty2 = null;

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
    }

#pragma warning disable RCS1016, RCS1085 // Use expression-bodied member.
    internal class FormatAccessorList
    {
        private string _property = null;
        private string _property2 = null;
        private readonly string _readOnlyProperty = null;
        private readonly string _readOnlyProperty2 = null;

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
    }
#pragma warning restore RCS1016, RCS1085 // Use expression-bodied member.
}
