// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class SwapExpressionsInBinaryExpressionRefactoring
    {
        public void SomeMethod()
        {
            bool expression = false;
            bool expression2 = false;
            bool expression3 = false;

            if (expression && expression2)
            {
            }

            if (expression && expression2 && expression3)
            {
            }

            if (expression || expression2)
            {
            }

            bool f = false;

            bool f1 = false;
            bool f2 = false;

            f = f1 == f2;
            f = f1 != f2;

            int i1 = 0;
            int i2 = 0;

            int ii = i1;

            int i3 = i1 + i2;
            int i4 = i1 * i2;

            f = i1 > i2;
            f = i1 >= i2;
            f = i1 < i2;
            f = i1 <= i2;
        }
    }
}
