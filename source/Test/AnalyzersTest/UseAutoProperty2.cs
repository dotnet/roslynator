// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#pragma warning disable RCS1016, RCS1163

namespace Roslynator.CSharp.Analyzers.Test
{
    internal partial class UseAutoProperty
    {
        private string _property2 = null;

        public UseAutoProperty(object parameter)
        {
            _property = null;
            _property2 = null;
            _readOnlyProperty = null;
            _readOnlyProperty2 = null;
        }

        public void Foo2()
        {
            _property = null;
            _property2 = null;
        }
    }
}
