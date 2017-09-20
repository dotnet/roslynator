// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal static class WrapInElseClauseRefactoring
    {
        public static string Foo(bool f)
        {
            if (f)
            {
                return "Y";
            }

            return "N";
        }

        public static string Foo2(bool f)
        {
            if (f)
                return "Y";

            return "N";
        }

        public static string Foo3(bool f)
        {
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

        public static string Foo4(bool f)
        {
            if (f)
            {
                return "Y";
            }

            Foo(f);
            return "N";
        }

        public static string Foo5(bool f)
        {
            if (f)
                return "Y";

            Foo(f);
            return "N";
        }

        public static string Foo6(bool f)
        {
            if (f)
            {
                return "Y";
            }
            else if (f)
            {
                return "YY";
            }

            Foo(f);
            return "N";
        }

        //n

        public static string Foo7(bool f)
        {
            if (f)
            {
                Foo(f);
            }

            return "N";
        }
    }
}
