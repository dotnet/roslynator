// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class FormatConditionalExpression
    {
        private class Foo
        {
            public void Bar()
            {
                bool condition = false;

                var s = (condition) ? true : false;
            }

            public void Bar2()
            {
                bool condition = false;

                var s = (condition)
                    ? true
                    : false;
            }

            // n

            public void Bar3()
            {
                bool condition = false;

                var s = (condition)
                    ? true //x
                    : false;
            }
        }
    }
}
