// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.CodeFixes.Tests
{
    internal static class CS1597_SemicolonAfterMethodOrAccessorBlockIsNotValid
    {
        private class Foo
        {
            private string _property;

            public string Property
            {
                get { return _property; }; // CS1597
                set { _property = value; }; // CS1597
            }

            public void Method()
            {
            }; // CS1597
        }
    }
}
