// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal static class ReplaceIfElseWithAssignment
    {
        public static bool Foo()
        {
            bool condition = false;
            bool x = false;

            if (condition)
            {
                x = true;
            }
            else
            {
                x = false;
            }

            return x;
        }
    }
}
