// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal class UsePostfixUnaryOperatorInsteadOfAssignment
    {
        private static int _field;

        public static int Property { get; set; }

        public static void Method()
        {
            int i = 0;
            int i2 = 0;

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

            i = i2 + 1;
            i = i2 - 1;

            i =
            #region
                i + 1;
            #endregion

            i /*a*/ = /*b*/ i /*c*/ + /*d*/ 1 /*e*/ ; /*f*/
        }
    }
}
