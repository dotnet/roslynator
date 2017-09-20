// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class UsePostfixUnaryOperatorInsteadOfAssignment
    {
        private static int _field;

        public static int Property { get; set; }

        public static void Method(int i, int i2)
        {
            i = i + 1;
            i = i - 1;
            i += 1;
            i -= 1;

            _field = _field + 1;
            _field = _field - 1;
            _field += 1;
            _field -= 1;

            Property = Property + 1;
            Property = Property - 1;
            Property += 1;
            Property -= 1;

            i /*a*/ = /*b*/ i /*c*/ + /*d*/ 1 /*e*/ ; /*f*/

            //n

            i = i2 + 1;
            i = i2 - 1;

            i =
            #region
                i + 1;
            #endregion
        }

        private class Foo
        {
            public int Property { get; set; }

            private void Bar()
            {
                var foo = new Foo() { Property = Property + 1 };

                var x = new { Property = Property + 1 };
            }
        }
    }
}
