// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#pragma warning disable RCS1163, RCS1169, RCS1213

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal static class InitializeFieldFromConstructorRefactoring
    {
        private class Foo
        {
            private string _bar;

            private string _bar2, _bar3;

            public Foo()
            {
            }

            public Foo(object parameter)
            {
                Bar(parameter);
            }

            public Foo(object parameter1, object bar)
                : this(parameter1)
            {
                Bar(bar);
            }

            public void Bar(object parameter)
            {
            }
        }

        private class Foo2
        {
            private string bar;

            private string _bar2, _bar3;

            public Foo2()
            {
            }

            public Foo2(object parameter)
            {
                Bar(parameter);
            }

            public Foo2(object parameter1, object parameter2)
                : this(parameter1)
            {
                Bar(parameter2);
            }

            public void Bar(object parameter)
            {
            }
        }
    }
}
