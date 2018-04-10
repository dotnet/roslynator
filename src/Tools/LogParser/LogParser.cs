// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Text.RegularExpressions;
using static Pihrtsoft.Text.RegularExpressions.Linq.Patterns;
using Pihrtsoft.Text.RegularExpressions.Linq;

namespace Roslynator.Diagnostics
{
    public static class LogParser
    {
        private static readonly string _pattern = BeginLine()
            .WhileWhiteSpaceExceptNewLine()
            .NamedGroup("ElapsedSign", "<").Maybe()
            .NamedGroup("ElapsedSeconds", ArabicDigits())
            .Comma()
            .NamedGroup("ElapsedMilliSeconds", ArabicDigits())
            .WhiteSpaceExceptNewLine().OneMany()
            .NamedGroup("PercentSign", "<").Maybe()
            .NamedGroup("Percent", ArabicDigits())
            .WhiteSpaceExceptNewLine().OneMany()
            .NamedGroup("FullName", NotChar(Chars.WhiteSpace().Comma()).OneMany())
            .WhiteSpace()
            .ToString(PatternOptions.Format);

        private static readonly Regex _regex = new Regex(_pattern, RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

        public static IEnumerable<AnalyzerLogInfo> Parse(string content)
        {
            Match match = _regex.Match(content);

            while (match.Success)
            {
                string fullName = match.Groups["FullName"].Value;

                int elapsed = 0;

                if (!match.Groups["ElapsedSign"].Success)
                {
                    elapsed = (int.Parse(match.Groups["ElapsedSeconds"].Value) * 1000) + int.Parse(match.Groups["ElapsedMilliSeconds"].Value);
                }

                int percent = 0;

                if (!match.Groups["PercentSign"].Success)
                {
                    percent = int.Parse(match.Groups["Percent"].Value);
                }

                yield return new AnalyzerLogInfo(
                    fullName,
                    elapsed,
                    percent);

                match = match.NextMatch();
            }
        }
    }
}
