// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class WrapInConditionRefactoring
    {
        public string Method()
        {
            string s = null;

            switch (0)
            {
                case 0:
                    string s2 = null;
                    break;
            }

            return s;
        }
    }
}
