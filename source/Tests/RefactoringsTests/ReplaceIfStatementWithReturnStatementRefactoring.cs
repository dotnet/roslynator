// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal static class ReplaceIfStatementWithReturnStatementRefactoring
    {
        private static bool Foo()
        {
            bool condition = false;
            bool x = false;

            if (condition)
            {
                return true;
            }
            else
            {
                return true;
            }

            if (condition)
            {
                return true;
            }
            else
            {
                return false;
            }

            if (condition)
            {
                return true;
            }
            else
            {
                return x;
            }

            if (condition)
            {
                return false;
            }
            else
            {
                return true;
            }

            if (condition)
            {
                return false;
            }
            else
            {
                return false;
            }

            if (condition)
            {
                return false;
            }
            else
            {
                return x;
            }

            if (condition)
            {
                return true;
            }
            else
            {
                return x;
            }

            if (condition)
            {
                return x;
            }
            else
            {
                return true;
            }

            if (condition)
            {
                return GetValue();
            }
            else
            {
                return false;
            }

            if (condition)
            {
                return x;
            }
            else
            {
                return !x;
            }
        }

        private static bool Foo2()
        {
            bool condition = false;
            bool x = false;

            if (condition)
            {
                return true;
            }

            return false;

            if (condition)
            {
                return true;
            }

            return x;

            if (condition)
            {
                return false;
            }

            return true;

            if (condition)
            {
                return false;
            }

            return false;

            if (condition)
            {
                return false;
            }
            else
            {
                return x;
            }

            if (condition)
            {
                return true;
            }

            return x;

            if (condition)
            {
                return x;
            }

            return true;

            if (condition)
            {
                return GetValue();
            }

            return false;

            if (condition)
            {
                return x;
            }

            return !x;
        }

        public static IEnumerable<bool> Foo3()
        {
            bool condition = false;
            bool x = false;
            bool y = false;

            if (condition)
            {
                yield return true;
            }
            else
            {
                yield return false;
            }

            if (condition)
            {
                yield return x;
            }
            else
            {
                yield return y;
            }
        }

        private static bool GetValue()
        {
            return false;
        }
    }
}
