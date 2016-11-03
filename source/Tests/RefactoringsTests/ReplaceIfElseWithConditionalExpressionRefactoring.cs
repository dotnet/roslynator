// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class ReplaceIfElseWithConditionalExpressionRefactoring
    {
        public static bool MethodName()
        {
            bool condition = false;

            if (condition)
                return true;
            else
                return false;

            bool f = false;

            if (condition)
            {
                f = false;
            }
            else
            {
                f = true;
            }
        }

        public static IEnumerable<bool> MethodName2()
        {
            bool condition = false;

            if (condition)
            {
                yield return true;
            }
            else
            {
                yield return false;
            }
        }
    }
}
