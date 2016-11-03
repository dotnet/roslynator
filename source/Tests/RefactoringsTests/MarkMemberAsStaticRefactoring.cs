// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal static class MarkMemberAsStaticRefactoring
    {
        public static class SomeClass
        {
            public void Do()
            {
            }

            public void Do2()
            {
            }
        }

        private string _value;

        public void MethodName()
        {
        }

        public MarkMemberAsStaticRefactoring()
        {

        }

        public string PropertyName { get; set; }

        public event EventHandler EventName;

        public event EventHandler EventName2
        {
            add { }
            remove { }
        }

        private class ClassName
        {
            public void MethodName()
            {
            }
        }
    }
}
