// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Pihrtsoft.CodeAnalysis.CSharp.Analyzers.Tests
{
    internal class UsePostfixUnaryInsteadOfAssignment
    {
        private static int _f;

        public static void MethodName()
        {
            int i = 0;
            int i2 = 0;

            i = i + 1;
            i = i - 1;
            i += 1;
            i -= 1;

            _f = _f + 1;
            _f = _f - 1;
            _f += 1;
            _f -= 1;

            P = P + 1;
            P = P - 1;
            P += 1;
            P -= 1;

            i = i2 + 1;
            i = i2 - 1;
        }

        public static int P { get; set; }
    }
}
