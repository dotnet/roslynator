// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal class FormatArgumentListRefactoring
    {
        private class FormatAllArgumentsOnSingleLineRefactoring
        {
            public void SomeMethod2()
            {
                string value = null;
                string value2 = null;
                string value3 = null;

                SomeMethod(
                    value,
                    value2,
                    value3);
            }

            public void SomeMethod(string value, string value2, string value3)
            {
            }
        }

        private class FormatEachArgumentOnSeparateLineRefactoring
        {
            public void SomeMethod2()
            {
                string value = null;
                string value2 = null;
                string value3 = null;

                SomeMethod(value, value2, value3);
            }

            public void SomeMethod(string value, string value2, string value3)
            {
            }
        }
    }
}
