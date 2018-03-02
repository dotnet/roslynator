// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

#pragma warning disable RCS1032, RCS1163, RCS1176, RCS1187, RCS1190, RCS1213

namespace Roslynator.CSharp.Analyzers.Tests
{
    public static class UseRegexInstanceInsteadOfStaticMethod
    {
        private class Foo
        {
            protected Regex _regex;
            private static readonly string _input = "";
            private const string _pattern = "";
            private static readonly string _replacement = "";

            protected readonly Match _match = Regex.Match("input", "pattern");

            private class Foo1
            {
                public void Bar1()
                {
                    bool isMatch1 = Regex.IsMatch(_input, ("pattern"));
                }
            }

            private class Foo2
            {
                public void Bar2()
                {
                    bool isMatch2 = Regex.IsMatch(_input, _pattern);
                }
            }

            private class Foo3
            {
                public void Bar3()
                {
                    bool isMatch3 = Regex.IsMatch(_input, "pattern", RegexOptions.Singleline);
                }
            }

            private class Foo4
            {
                public void Bar4()
                {
                    bool isMatch4 = Regex.IsMatch(_input, "pattern", RegexOptions.Singleline | RegexOptions.Multiline);
                }
            }

            private class Foo5
            {
                public void Bar5()
                {
                    Match match1 = Regex.Match(_input, "pattern");
                }
            }

            private class Foo6
            {
                public void Bar6()
                {
                    Match match2 = Regex.Match(_input, "pattern", RegexOptions.None);
                }
            }

            private class Foo7
            {
                public void Bar7()
                {
                    MatchCollection matches1 = Regex.Matches(_input, "pattern");
                }
            }

            private class Foo8
            {
                public void Bar8()
                {
                    MatchCollection matches2 = Regex.Matches(_input, "pattern", RegexOptions.None);
                }
            }

            private class Foo9
            {
                public void Bar9()
                {
                    string[] values1 = Regex.Split(_input, "pattern");
                }
            }

            private class Foo10
            {
                public void Bar10()
                {
                    string[] values2 = Regex.Split(_input, "pattern", RegexOptions.None);
                }
            }

            private class Foo11
            {
                public void Bar11()
                {
                    string value1 = Regex.Replace(_input, "pattern", _replacement);
                }
            }

            private class Foo12
            {
                public void Bar12()
                {
                    string value2 = Regex.Replace(_input, "pattern", _replacement, RegexOptions.None);
                }
            }

            private class Foo13
            {
                public void Bar13()
                {
                    string value3 = Regex.Replace(_input, "pattern", default(MatchEvaluator));
                }
            }

            private class Foo14
            {
                public void Bar14()
                {
                    string value4 = Regex.Replace(_input, "pattern", default(MatchEvaluator), RegexOptions.None);
                }
            }

            private class Foo15
            {
                public void Bar15()
                {
                    Action<object> action = f => { Match match3 = Regex.Match(_input, "pattern"); };
                }
            }

            private class Foo16
            {
                public string Property
                {
                    get
                    {
                        Match match = Regex.Match(_input, _pattern);
                        return null;
                    }
                }
            }
        }

        // n

        private class Foo2
        {
            private readonly string _pattern;

            public void Bar()
            {
                const RegexOptions options = RegexOptions.None;

                const string pattern = "";

                bool isMatch1 = Regex.IsMatch("input", _pattern);
                bool isMatch2 = Regex.IsMatch("input", pattern);
                bool isMatch3 = Regex.IsMatch("input", "pattern", options);
                bool isMatch4 = Regex.IsMatch("input", "pattern", RegexOptions.None, TimeSpan.Zero);

                Match match = Regex.Match("input", "pattern", RegexOptions.None, TimeSpan.Zero);

                MatchCollection matches = Regex.Matches("input", "pattern", RegexOptions.None, TimeSpan.Zero);

                string[] values = Regex.Split("input", "pattern", RegexOptions.None, TimeSpan.Zero);

                string value3 = Regex.Replace("input", "pattern", default(MatchEvaluator), RegexOptions.None, TimeSpan.Zero);
                string value4 = Regex.Replace("input", "pattern", "replacement", RegexOptions.None, TimeSpan.Zero);
            }

            public void BarInstance()
            {
                Regex _regex = null;

                bool isMatch1 = _regex.IsMatch("pattern");

                Match match1 = _regex.Match("pattern");

                MatchCollection matches1 = _regex.Matches("pattern");

                string[] values1 = _regex.Split("pattern");

                string value1 = _regex.Replace("pattern", "replacement");

                Action<object> action = f => { Match match3 = _regex.Match("pattern"); };
            }
        }
    }
}
