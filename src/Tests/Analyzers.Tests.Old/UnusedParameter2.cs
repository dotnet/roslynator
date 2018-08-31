// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel;

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static partial class UnusedParameter
    {
        private partial class FooPartial : Foo
        {
            partial void BarPartial<T>(object parameter)
            {
            }

            private void Method2()
            {
                Action<object> action = Method2;
            }
        }
    }
}
