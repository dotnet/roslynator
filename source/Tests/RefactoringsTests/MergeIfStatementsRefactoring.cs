// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class MergeIfStatementsRefactoring
    {
        public bool Method()
        {
            bool f = false;
            bool f2 = false;
            bool f3 = false;

            if (f)
                return true;

            if (f2)
                return true;

            if (f3)
            {
                return true;
            }

            if (f)
            {
                return true;
            }

            if (f2)
            {
                return false;
            }

            if (f3)
            {
                return true;
            }

            switch (0)
            {
                case 0:
                    if (f)
                        return true;

                    if (f2)
                        return true;

                    break;
            }

            return false;
        }
    }
}
