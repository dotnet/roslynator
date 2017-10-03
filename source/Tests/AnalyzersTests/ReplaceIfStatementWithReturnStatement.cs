// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#pragma warning disable RCS1002, RCS1004, RCS1007, RCS1111, RCS1118, RCS1126, RCS1128, RCS1176

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class ReplaceIfStatementWithReturnStatement
    {
        private static bool Foo()
        {
            bool condition = false;

            if (condition)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool Foo2()
        {
            bool condition = false;

            if (condition)
                return true;
            else
                return false;
        }

        private static bool Foo3()
        {
            bool condition = false;

            if (condition)
            {
                return true;
            }

            return false;
        }

        private static bool Foo4()
        {
            bool condition = false;

            if (condition)
                return true;

            return false;
        }

        private static bool Foo5()
        {
            bool condition = false;

            if (condition)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static bool Foo6()
        {
            bool condition = false;

            if (condition)
            {
                return false;
            }

            return true;
        }

        private static bool Foo7()
        {
            bool condition = false;

            if (condition)
            {
                return false;
            }

            return true;
        }

        private static bool Foo8()
        {
            bool condition = false;

            if (condition)
                return false;

            return true;
        }

        private static bool Foo9()
        {
            switch ("")
            {
                default:
                    bool condition = false;

                    if (condition)
                    {
                        return true;
                    }

                    return false;
            }
        }

        //n

        private static bool Foo10()
        {
            bool condition = false;

            if (condition)
            {
                return true;
            }
            else
            {
                return true;
            }
        }

        private static bool Foo11()
        {
            bool condition = false;

            if (condition)
            {
                return false;
            }

            return false;
        }
    }
}
