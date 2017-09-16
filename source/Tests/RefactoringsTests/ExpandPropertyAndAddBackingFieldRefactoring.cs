// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class ExpandPropertyAndAddBackingFieldRefactoring
    {
        private string _property = null;
        private string _readOnlyProperty = null;

        public ExpandPropertyAndAddBackingFieldRefactoring()
        {
            Property = null;
            ReadOnlyProperty = null;
            ReadOnlyProperty = null;
        }

        private void Foo()
        {
            Property = null;
        }

        public string Property { get; set; } = null;

        public string ReadOnlyProperty { get; } = null;
    }
}
