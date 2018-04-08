// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class FormatEmptyBlock
    {
        public static void Foo()
        { }

        public static void Foo2()
        {
            Action<object> action = f => { };
            Action<object> action2 = (f) => { };
            Action<object> action3 = delegate { };
        }

        public static string FooProperty
        {
            get { }
            set { }
        }
    }
}
