// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Test
{
    internal static class ReplaceIfElseWithIfReturnRefactoring
    {
        private static bool Foo()
        {
            bool f = false;

            if (f)
            {
                return false;
            }
            else if (f)
            {
                Foo2();
                return false;

            }
            else
            {
                Foo2();
                Foo2();
                return false;
            }

            if (f)
                return false;
            else if (f)
                return false;
            else
                return false;

            string s = null;

            if (s == null)
            {
                return false;
            }
            else if (s.Length == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //n

        private static bool Foo2()
        {
            bool f = false;

            if (f)
            {
                return false;
            }

            if (f)
            {
                return false;
            }
            else if (f)
            {
                Foo2();
                return false;

            }

            if (f)
            {
                return false;
            }
            else
            {
                Foo2();
            }

            if (f)
            {
                Foo2();
            }
            else
            {
                return false;
            }

            return false; 
        }
    }
}
