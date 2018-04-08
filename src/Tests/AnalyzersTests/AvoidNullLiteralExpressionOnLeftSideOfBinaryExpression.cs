// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class AvoidNullLiteralExpressionOnLeftSideOfBinaryExpression
    {
        public static void Foo()
        {
            string s = GetValue();

            if (null == s)
            {
            }

            if (null
#if DEBUG
                == s)
            {
            }
#endif
        }

        private static string GetValue()
        {
            return null;
        }
    }
}
