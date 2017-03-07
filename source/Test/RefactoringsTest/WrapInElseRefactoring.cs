// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Test
{
    internal static class WrapInElseRefactoring
    {
        public static string Foo()
        {
            bool f = false;

            if (f)
            {
                return "Y";
            }

            return "N";
        }

        public static string Foo2()
        {
            bool f = false;

            if (f)
                return "Y";

            return "N";
        }

        public static string Foo3()
        {
            bool f = false;

            if (f)
            {
                return "Y";
            }
            else if (f)
            {
                return "YY";
            }

            return "N";
        }

        public static string Foo4()
        {
            bool f = false;

            if (f)
            {
                Foo4();
            }

            return "N";
        }
    }
}
