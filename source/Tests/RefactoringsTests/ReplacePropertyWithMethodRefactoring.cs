// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings.Tests
{
    internal class ReplacePropertyWithMethodRefactoring
    {
        public string Value
        {
            get { return string.Empty; }
        }

        public string IsValue
        {
            get { return string.Empty; }
        }

        public string HasValue
        {
            get { return _value2; }
            set { _value2 = value; }
        }

        public string Value2
        {
            get { return _value2; }
            set { _value2 = value; }
        }

        public string Value3 { get; } = new string(' ', 1);

        private string _propertyName;
        private string _value2;
#if true
        public string PropertyName
        {
            get
            {
                return _propertyName;
            }
            protected internal set
            {
                _propertyName = value;
            }
        }
#endif
    }
}

