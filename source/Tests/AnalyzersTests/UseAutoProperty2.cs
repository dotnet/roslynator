// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#pragma warning disable RCS1016, RCS1163

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal partial class UseAutoProperty
    {
        public UseAutoProperty(object parameter)
        {
            _property = null;
            _property2 = null;
            _staticProperty = null;
            _readOnlyProperty = null;
            _readOnlyProperty2 = null;
        }

        public void Method2()
        {
            _property = null;
            _property2 = null;

            OutMethod(out _propertyOut);
        }
    }
}
