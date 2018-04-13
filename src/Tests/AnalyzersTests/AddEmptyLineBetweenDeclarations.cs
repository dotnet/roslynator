// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#pragma warning disable RCS1023, RCS1100 

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class AddEmptyLineBetweenDeclarations
    {
        private static class Foo
        {
            public static void M1()
            {
            }
            public static void M2()
            {
            }
        }
        private static class Foo2
        {
            public static void M1()
            {
            }
            /// <summary>
            /// x
            /// </summary>
            public static void M2()
            {
            }
        }
        private static class Foo3
        {
            public static void M1() { }
            /// <summary>
            /// x
            /// </summary>
            public static void M2() { }
        }
        private enum EnumName
        {
            A = 0,
            /// <summary>
            /// x
            /// </summary>
            B = 1,
        }

        //n

        private static class Foo4
        {
            public static void M1() { }

            /// <summary>
            /// x
            /// </summary>
            public static void M2() { }
        }

        private static class Foo5
        {
            public static void M1()
            {
            }

            /// <summary>
            /// x
            /// </summary>
            public static void M2()
            {
            }
        }

        private enum EnumName2
        {
            A = 0,
            B = 1,
        }

        private enum EnumName3
        {
            A = 0,

            /// <summary>
            /// x
            /// </summary>
            B = 1,
        }
    }
}
