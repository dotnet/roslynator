// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class NegateBinaryExpressionRefactoring
    {
        public void SomeMethod()
        {
            bool a = false;
            bool b = false;
            bool c = false;
            bool d = false;

            bool x = false;
            bool y = false;
            bool z = false;

            if (x && y && z)
            {
            }

            if (x & y & z)
            {
            }

            if (x || y && z)
            {
            }

            if (x && y || z)
            {
            }

            if (a && b || c && d)
            {
            }

            if (a || b && c && d)
            {
            }
        }
    }
}
