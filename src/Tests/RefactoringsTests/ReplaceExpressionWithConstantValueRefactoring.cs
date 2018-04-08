// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text.RegularExpressions;

#pragma warning disable CS0219, IDE0002, RCS1118

namespace Roslynator.CSharp.Refactorings.Tests
{
    public static class ReplaceExpressionWithConstantValueRefactoring
    {
        private class Foo
        {
            private const string StringConstant = "abc";
            private const string StringNullConstant = null;
            private const bool BooleanConstant = true;
            private const char CharConstant = 'a';
            private const int Int32Constant = 1;
            private const int Int64Constant = Int32Constant + 1;

            public static void Bar()
            {
                string s = StringConstant;
                s = Foo.StringConstant;
                s = ReplaceExpressionWithConstantValueRefactoring.Foo.StringConstant;

                string s2 = StringNullConstant;
                bool f = BooleanConstant;
                char ch = CharConstant;
                int i = Int32Constant;
                int l = Int64Constant;

                const string x = "x";
                string x2 = x;

                RegexOptions options1 = RegexOptions.None;
                RegexOptions options2 = System.Text.RegularExpressions.RegexOptions.None;
                RegexOptions options3 = global::System.Text.RegularExpressions.RegexOptions.None;
            }
        }
    }
}
