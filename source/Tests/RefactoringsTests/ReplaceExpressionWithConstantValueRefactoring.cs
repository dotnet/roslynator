// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Roslynator.CSharp.Refactorings.Tests
{
    public static class ReplaceExpressionWithConstantValueRefactoring
    {
        private const string StringConstant = "abc";
        private const string StringNullConstant = null;
        private const bool BooleanConstant = true;
        private const char CharConstant = 'a';
        private const int Int32Constant = 1;
        private const int Int64Constant = Int32Constant + 1;

        public static void Foo()
        {
            string s = StringConstant;
            string s2 = StringNullConstant;
            bool f = BooleanConstant;
            char ch = CharConstant;
            int i = Int32Constant;
            int l = Int64Constant;

            const string x = "x";
            string x2 = x;
        }
    }
}
