// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal static class SplitIfStatementsRefactoring
    {
        private static bool Foo()
        {
            bool x = false;
            bool y = false;

            if (x || y)
                return true;

            bool f1 = false;
            bool f2 = false;
            bool f3 = false;
            bool f4 = false;

            // a
            if (f1 || f2 || f3 || f4 && f4)
            {
                Foo();
            } // b

            if (f1)
                if (f1 || f2 || f3 || f4 && f4)
                {
                    Foo();
                }

            return false;
        }
    }
}
