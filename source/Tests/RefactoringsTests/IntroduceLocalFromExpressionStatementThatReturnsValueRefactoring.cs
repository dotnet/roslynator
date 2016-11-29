// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    internal static class IntroduceLocalFromExpressionStatementThatReturnsValueRefactoring
    {
        public static void Foo()
        {
            int i;
            i = 0;
            i++;

            Execute();

            var x = GetValue();

            GetValue();
        }

        public static void Execute()
        {
        }

        public static string GetValue()
        {
            string s = null;

            return s;
        }
    }
}
