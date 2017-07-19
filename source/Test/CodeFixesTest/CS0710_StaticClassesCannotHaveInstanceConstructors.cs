// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;

namespace Roslynator.CSharp.CodeFixes.Test
{
    internal static class CS0710_StaticClassesCannotHaveInstanceConstructors
    {
        public static class Foo
        {
            public Foo()
            {
            }
        }

        private static class Foo2
        {
            Foo2()
            {
            }

            //n

            Foo2(object parameter)
            {
            }
        }
    }
}
