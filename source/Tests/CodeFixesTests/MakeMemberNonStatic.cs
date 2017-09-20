// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.CodeFixes.Tests
{
    internal static class MakeMemberNonStatic
    {
        private class Foo
        {
            private static string Property
            {
                get { return InstanceProperty; }
            }

            private static string Method()
            {
                return InstanceMethod();
            }

            private static string Method2()
            {
                return InstanceField;
            }

            public string InstanceField;

            private string InstanceProperty
            {
                get { return ""; }
            }

            private string InstanceMethod()
            {
                return "";
            }
        }
    }
}
