// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analyzers.Test
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

        private static bool Foo()
        {
            bool condition = false;

            if (condition)
                return true;
            else
                return false;
        }

        private static bool Foo()
        {
            bool condition = false;

            if (condition)
            {
                return true;
            }

            return false;
        }

        private static bool Foo()
        {
            bool condition = false;

            if (condition)
                return true;

            return false;
        }

        private static bool Foo()
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

        private static bool Foo()
        {
            bool condition = false;

            if (condition)
            {
                return false;
            }

            return true;
        }

        private static bool Foo()
        {
            bool condition = false;

            if (condition)
            {
                return false;
            }

            return true;
        }

        private static bool Foo()
        {
            bool condition = false;

            if (condition)
                return false;

            return true;
        }

        private static bool Foo()
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

        private static bool Foo()
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
