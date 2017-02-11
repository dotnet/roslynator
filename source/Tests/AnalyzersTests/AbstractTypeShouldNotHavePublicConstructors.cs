// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class AbstractTypeShouldNotHavePublicConstructors
    {
        public abstract class Foo
        {
            public Foo()
            {
            }

            internal Foo(object parameter)
            {
            }

            protected Foo(object parameter1, object parameter2)
            {
            }
        }

        public class Foo2
        {
            public Foo2()
            {
            }

            internal Foo2(object parameter)
            {
            }

            protected Foo2(object parameter1, object parameter2)
            {
            }
        }
    }
}
