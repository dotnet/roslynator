// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class RemoveStatementRefactoring
    {
        public bool GetValue()
        {
            bool condition = false;

            if (condition)
            {
            }

            string s = null;

            switch (s)
            {
                case "":
                    if (condition)
                    {
                        return false;
                    }
                    break;
                default:
                    break;
            }

            // n

            if (condition)
                if (condition)
                {
                    return false;
                }

            return true;
        }
    }
}
