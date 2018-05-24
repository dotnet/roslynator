// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis.Text;
using Roslynator.Text;

namespace Roslynator.Tests.Text
{
    internal static class TestSourceText
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

            bool startPending = false;
            LinePositionInfo start = default;
            Stack<LinePositionInfo> stack = null;
            List<LinePositionSpanInfo> spans = null;

            int lastPos = 0;

            int line = 0;
            int column = 0;

            int length = s.Length;

            int i = 0;
            while (i < length)
            {
                switch (s[i])
                {
                    case '\r':
                        {
                            if (PeekNextChar() == '\n')
                            {
                                i++;
                            }

                            line++;
                            column = 0;
                            i++;
                            continue;
                        }
                    case '\n':
                        {
                            line++;
                            column = 0;
                            i++;
                            continue;
                        }
                    case '[':
                        {
                            char nextChar = PeekNextChar();
                            if (nextChar == '|')
                            {
                                sb.Append(s, lastPos, i - lastPos);

                                var start2 = new LinePositionInfo(sb.Length, line, column);

                                if (stack != null)
                                {
                                    stack.Push(start2);
                                }
                                else if (!startPending)
                                {
                                    start = start2;
                                    startPending = true;
                                }
                                else
                                {
                                    stack = new Stack<LinePositionInfo>();
                                    stack.Push(start);
                                    stack.Push(start2);
                                    startPending = false;
                                }

                                i += 2;
                                lastPos = i;
                                continue;
                            }
                            else if (nextChar == '['
                                && PeekChar(2) == '|'
                                && PeekChar(3) == ']')
                            {
                                i++;
                                column++;
                                CloseSpan();
                                i += 3;
                                lastPos = i;
                                continue;
                            }

                            break;
                        }
                    case '|':
                        {
                            if (PeekNextChar() == ']')
                            {
                                CloseSpan();
                                i += 2;
                                lastPos = i;
                                continue;
                            }

                            break;
                        }
                }

                column++;
                i++;
            }

            if (startPending
                || stack?.Count > 0)
            {
                throw new InvalidOperationException();
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

            char PeekNextChar()
            {
                return PeekChar(1);
            }

            char PeekChar(int offset)
            {
                return (i + offset >= s.Length) ? '\0' : s[i + offset];
            }

            void CloseSpan()
            {
                if (stack != null)
                {
                    start = stack.Pop();
                }
                else if (startPending)
                {
                    startPending = false;
                }
                else
                {
                    throw new InvalidOperationException();
                }

                var end = new LinePositionInfo(sb.Length + i - lastPos, line, column);

                var span = new LinePositionSpanInfo(start, end);

                (spans ?? (spans = new List<LinePositionSpanInfo>())).Add(span);

                sb.Append(s, lastPos, i - lastPos);
            }
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
