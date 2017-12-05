// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Analyzers.Tests
{
    internal static class ParenthesizeConditionInConditionalExpression
    {
        private static void Foo()
        {
            bool condition = false;

            object x = condition ? WhenTrue() : WhenFalse();
        }

        private static object WhenTrue()
        {
            return null;
        }

        private static object WhenFalse()
        {
            return null;
        }
    }
}
