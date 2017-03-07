// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp.Refactorings.Test
{
    internal static class MarkMemberAsStaticRefactoring
    {
        public static class Foo
        {
            public void Bar()
            {
            }

            public void Bar2()
            {
            }
        }

        private string _fieldName;

        public void MethodName()
        {
        }

        public unsafe MarkMemberAsStaticRefactoring()
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
