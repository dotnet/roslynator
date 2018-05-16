// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text.RegularExpressions;
using Pihrtsoft.Text.RegularExpressions.Linq;
using static Pihrtsoft.Text.RegularExpressions.Linq.Patterns;

namespace Roslynator.Diagnostics
{
    public static class LogParser
    {
        private const RegexOptions Options = RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture;

        private static readonly string _totalTimePattern = WhileWhiteSpaceExceptNewLine()
            .Text("Total analyzer execution time: ")
            .NamedGroup("TotalSeconds", ArabicDigits())
            .Comma()
            .NamedGroup("TotalMilliseconds", ArabicDigits())
            .Text(" seconds.")
            .ToString(PatternOptions.Format);

        private static readonly string _analyzerTimePattern = WhileWhiteSpaceExceptNewLine()
            .NamedGroup("ElapsedSign", "<").Maybe()
            .NamedGroup("ElapsedSeconds", ArabicDigits())
            .Comma()
            .NamedGroup("ElapsedMilliseconds", ArabicDigits())
            .WhiteSpaceExceptNewLine().OneMany()
            .NamedGroup("PercentSign", "<").Maybe()
            .NamedGroup("Percent", ArabicDigits())
            .WhiteSpaceExceptNewLine().OneMany()
            .NamedGroup("FullName", NotChar(Chars.WhiteSpace().Comma()).OneMany())
            .NamedGroup("TrailingComma", Comma()).Maybe()
            .ToString(PatternOptions.Format);

        private static readonly Regex _totalTimeRegex = new Regex(_totalTimePattern, Options);

        private static readonly Regex _analyzerTimeRegex = new Regex(_analyzerTimePattern, Options);

        public static IEnumerable<ProjectDiagnosticInfo> Parse(string filePath)
        {
            using (IEnumerator<string> en = File.ReadLines(filePath).GetEnumerator())
            {
                while (en.MoveNext())
                {
                    string line = en.Current;

                    if (en.Current.StartsWith("  Total analyzer execution time:"))
                    {
                        Match match = _totalTimeRegex.Match(en.Current);

                        if (!match.Success)
                            throw new InvalidOperationException();

                        int total = (int.Parse(match.Groups["TotalSeconds"].Value) * 1000) + int.Parse(match.Groups["TotalMilliseconds"].Value);

                        if (!en.MoveNext())
                            throw new InvalidOperationException();

                        if (!en.Current.StartsWith("  NOTE:", StringComparison.Ordinal))
                            throw new InvalidOperationException();

                        if (!en.MoveNext())
                            throw new InvalidOperationException();

                        if (!en.Current.StartsWith("  Time (s)", StringComparison.Ordinal))
                            throw new InvalidOperationException();

                        ImmutableArray<AnalyzerDiagnosticInfo>.Builder builder = ImmutableArray.CreateBuilder<AnalyzerDiagnosticInfo>();

                        while (en.MoveNext()
                            && (match = _analyzerTimeRegex.Match(en.Current)).Success)
                        {
                            if (match.Groups["TrailingComma"].Success)
                                continue;

                            string fullName = match.Groups["FullName"].Value;

                            int elapsed = 0;

                            if (!match.Groups["ElapsedSign"].Success)
                                elapsed = (int.Parse(match.Groups["ElapsedSeconds"].Value) * 1000) + int.Parse(match.Groups["ElapsedMilliseconds"].Value);

                            int percent = 0;

                            if (!match.Groups["PercentSign"].Success)
                                percent = int.Parse(match.Groups["Percent"].Value);

                            builder.Add(new AnalyzerDiagnosticInfo(fullName, elapsed, percent));
                        }

                        yield return new ProjectDiagnosticInfo(total, builder.ToImmutableArray());
                    }
                }
            }
        }
    }
}
