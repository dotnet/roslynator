// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings.Tests
{
    internal static class CheckExpressionForNullRefactoring
    {
        public static void Foo()
        {
            string s = GetValueOrDefault();

            s = GetValueOrDefault();

            int i = GetValue();
        }

        private static string GetValueOrDefault()
        {
            return null;
        }

        private static int GetValue()
        {
            return 0;
        }
    }
}
