// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis.Text;
using Roslynator.Text;

namespace Roslynator.Tests
{
    public static class TestSourceText
    {
        internal const string OpenMarker = "[|";
        internal const string CloseMarker = "|]";
        internal const string OpenMarkerAndCloseMarker = OpenMarker + CloseMarker;
        internal static readonly int MarkersLength = OpenMarkerAndCloseMarker.Length;

        public static (string result, TextSpan span) ReplaceSpan(string s, string replacement)
        {
            int index = s.IndexOf(OpenMarkerAndCloseMarker);

            var span = new TextSpan(index, replacement.Length);

            string result = Replace(s, index, replacement);

            return (result, span);
        }

        public static (string result1, string result2, TextSpan span) ReplaceSpan(
            string s,
            string replacement1,
            string replacement2)
        {
            int index = s.IndexOf(OpenMarkerAndCloseMarker);

            var span = new TextSpan(index, replacement1.Length);

            string result1 = Replace(s, index, replacement1);
            string result2 = Replace(s, index, replacement2);

            return (result1, result2, span);
        }

        public static TestSourceTextAnalysis GetSpans(string s, bool reverse = false)
        {
            StringBuilder sb = StringBuilderCache.GetInstance(s.Length - MarkersLength);

            List<LinePositionSpanInfo> spans = null;

            int lastPos = 0;

            int line = 0;
            int column = 0;

            int startLine = -1;
            int startColumn = -1;

            int length = s.Length;

            for (int i = 0; i < length; i++)
            {
                switch (s[i])
                {
                    case '\r':
                        {
                            if (i < length - 1
                                && s[i + 1] == '\n')
                            {
                                i++;
                            }

                            line++;
                            column = 0;
                            continue;
                        }
                    case '\n':
                        {
                            line++;
                            column = 0;
                            continue;
                        }
                    case '[':
                        {
                            if (i < length - 1
                                && i < length - 1
                                && s[i + 1] == '|')
                            {
                                sb.Append(s, lastPos, i - lastPos);

                                startLine = line;
                                startColumn = column;

                                i++;

                                lastPos = i + 1;

                                continue;
                            }

                            break;
                        }
                    case '|':
                        {
                            if (startColumn != -1
                                && i < length - 1
                                && s[i + 1] == ']')
                            {
                                int index = sb.Length;

                                var span = new LinePositionSpanInfo(
                                    new LinePositionInfo(index, startLine, startColumn),
                                    new LinePositionInfo(index + i - lastPos, line, column));

                                (spans ?? (spans = new List<LinePositionSpanInfo>())).Add(span);

                                sb.Append(s, lastPos, i - lastPos);

                                i++;

                                lastPos = i + 1;

                                startLine = -1;
                                startColumn = -1;

                                continue;
                            }

                            break;
                        }
                }

                column++;
            }

            sb.Append(s, lastPos, s.Length - lastPos);

            if (spans != null
                && reverse)
            {
                spans.Reverse();
            }

            return new TestSourceTextAnalysis(
                StringBuilderCache.GetStringAndFree(sb),
                spans?.ToImmutableArray() ?? ImmutableArray<LinePositionSpanInfo>.Empty);
        }

        private static string Replace(string s, int index, string replacement)
        {
            StringBuilder sb = StringBuilderCache.GetInstance(s.Length - MarkersLength + replacement.Length)
                .Append(s, 0, index)
                .Append(replacement)
                .Append(s, index + MarkersLength, s.Length - index - MarkersLength);

            return StringBuilderCache.GetStringAndFree(sb);
        }
    }
}
