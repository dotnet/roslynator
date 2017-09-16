// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

#pragma warning disable RCS1176

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class UseRegexInstanceInsteadOfStaticMethod
    {
        private class Foo
        {
            private const string _const = "";

            private void Bar()
            {
                Regex _regex = null;

                bool isMatch = Regex.IsMatch("input", "pattern");
                bool isMatch2 = Regex.IsMatch("input", "pattern", RegexOptions.None);
                bool isMatch3 = Regex.IsMatch("input", "pattern", RegexOptions.None, TimeSpan.Zero);
            }
        }

        private static class Foo2
        {
            private const string _const = "";

            private static string Bar
            {
                get
                {
                    Match match = Regex.Match("input", "pattern");
                    Match match2 = Regex.Match("input", "pattern", RegexOptions.None);
                    Match match3 = Regex.Match("input", "pattern", RegexOptions.None, TimeSpan.Zero);

                    return "";
                }
            }
        }

        private class Foo3
        {
            private const string _const = "";

            private readonly MatchCollection _matches = Regex.Matches("input", "pattern");
            private readonly MatchCollection _matches2 = Regex.Matches("input", "pattern", RegexOptions.None);
            private readonly MatchCollection _matches3 = Regex.Matches("input", "pattern", RegexOptions.None, TimeSpan.Zero);
        }

        private class Foo4
        {
            private const string _const = "";

            private void Bar()
            {
                string[] values = Regex.Split("input", "pattern");
                string[] values2 = Regex.Split("input", "pattern", RegexOptions.None);
                string[] values3 = Regex.Split("input", "pattern", RegexOptions.None, TimeSpan.Zero);
            }
        }

        private class Foo5
        {
            private void Bar()
            {
                Action<object> action = f =>
                {
                    string value = Regex.Replace("input", "pattern", default(MatchEvaluator));
                    string value2 = Regex.Replace("input", "pattern", "replacement");
                    string value3 = Regex.Replace("input", "pattern", default(MatchEvaluator), RegexOptions.None);
                    string value4 = Regex.Replace("input", "pattern", "replacement", RegexOptions.None);
                    string value5 = Regex.Replace("input", "pattern", default(MatchEvaluator), RegexOptions.None, TimeSpan.Zero);
                    string value6 = Regex.Replace("input", "pattern", "replacement", RegexOptions.None, TimeSpan.Zero);
                };
            }
        }
    }
}
