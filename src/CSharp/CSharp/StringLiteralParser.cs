// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Microsoft.CodeAnalysis.Text;
using Roslynator.Text;

namespace Roslynator.CSharp
{
    internal static class StringLiteralParser
    {
        private const string MissingEscapeSequenceMessage = "Missing escape sequence.";
        private const string UnrecognizedEscapeSequenceMessage = "Unrecognized escape sequence.";

        public static bool TryParse(string text, bool isVerbatim, bool isInterpolatedText, out string result)
        {
            return TryParse(text, 0, text.Length, isVerbatim, isInterpolatedText, out result);
        }

        public static bool TryParse(string text, int start, int length, bool isVerbatim, bool isInterpolatedText, out string result)
        {
            StringLiteralParserResult parseResult = (isVerbatim)
                ? ParseVerbatim(text, start, length, isInterpolatedText)
                : ParseRegular(text, start, length, isInterpolatedText);

            if (parseResult.Success)
            {
                result = parseResult.Text;
                return true;
            }

            result = null;
            return false;
        }

        public static string Parse(string text, bool isVerbatim, bool isInterpolatedText)
        {
            return Parse(text, 0, text.Length, isVerbatim, isInterpolatedText);
        }

        public static string Parse(string text, int start, int length, bool isVerbatim, bool isInterpolatedText)
        {
            return (isVerbatim)
                ? ParseVerbatim(text, start, length, throwOnError: true, isInterpolatedText: isInterpolatedText).Text
                : ParseRegular(text, start, length, throwOnError: true, isInterpolatedText: isInterpolatedText).Text;
        }

        private static StringLiteralParserResult ParseRegular(
            string text,
            int start,
            int length,
            bool throwOnError = false,
            bool isInterpolatedText = false)
        {
            StringBuilder sb = null;
            for (int pos = start; pos < start + length; pos++)
            {
                char ch = text[pos];
                if (ch == '\\')
                {
                    pos++;

                    if (pos >= text.Length)
                        return Fail(throwOnError, MissingEscapeSequenceMessage);

                    switch (text[pos])
                    {
                        case '\'':
                            {
                                ch = '\'';
                                break;
                            }
                        case '\"':
                            {
                                ch = '\"';
                                break;
                            }
                        case '\\':
                            {
                                ch = '\\';
                                break;
                            }
                        case '0':
                            {
                                ch = '\0';
                                break;
                            }
                        case 'a':
                            {
                                ch = '\a';
                                break;
                            }
                        case 'b':
                            {
                                ch = '\b';
                                break;
                            }
                        case 'f':
                            {
                                ch = '\f';
                                break;
                            }
                        case 'n':
                            {
                                ch = '\n';
                                break;
                            }
                        case 'r':
                            {
                                ch = '\r';
                                break;
                            }
                        case 't':
                            {
                                ch = '\t';
                                break;
                            }
                        case 'u':
                            {
                                pos++;

                                if (pos + 3 >= text.Length)
                                    return Fail(throwOnError, UnrecognizedEscapeSequenceMessage);

                                if (uint.TryParse(text.Substring(pos, 4), NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo, out uint result))
                                {
                                    ch = (char)result;
                                    pos += 3;
                                }
                                else
                                {
                                    return Fail(throwOnError, UnrecognizedEscapeSequenceMessage);
                                }

                                break;
                            }
                        case 'U':
                            {
                                pos++;

                                if (pos + 7 >= text.Length)
                                    return Fail(throwOnError, UnrecognizedEscapeSequenceMessage);

                                if (uint.TryParse(text.Substring(pos, 8), NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo, out uint result))
                                {
                                    if (result > 0xffff)
                                        return Fail(throwOnError, UnrecognizedEscapeSequenceMessage);

                                    ch = (char)result;
                                    pos += 7;
                                }
                                else
                                {
                                    return Fail(throwOnError, UnrecognizedEscapeSequenceMessage);
                                }

                                break;
                            }
                        case 'v':
                            {
                                ch = '\v';
                                break;
                            }
                        case 'x':
                            {
                                var sb2 = new StringBuilder(10);
                                pos++;

                                if (pos >= text.Length)
                                    return Fail(throwOnError, MissingEscapeSequenceMessage);

                                ch = text[pos];
                                if (IsHexadecimalDigit(ch))
                                {
                                    sb2.Append(ch);
                                    pos++;
                                    if (pos < text.Length)
                                    {
                                        ch = text[pos];
                                        if (IsHexadecimalDigit(ch))
                                        {
                                            sb2.Append(ch);
                                            pos++;
                                            if (pos < text.Length)
                                            {
                                                ch = text[pos];
                                                if (IsHexadecimalDigit(ch))
                                                {
                                                    sb2.Append(ch);
                                                    pos++;
                                                    if (pos < text.Length)
                                                    {
                                                        ch = text[pos];
                                                        if (IsHexadecimalDigit(ch))
                                                        {
                                                            sb2.Append(ch);
                                                            pos++;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    return Fail(throwOnError, UnrecognizedEscapeSequenceMessage);
                                }

                                ch = (char)int.Parse(sb2.ToString(), NumberStyles.HexNumber);
                                pos--;
                                break;
                            }
                        default:
                            {
                                return Fail(throwOnError, UnrecognizedEscapeSequenceMessage);
                            }
                    }
                }
                else if (isInterpolatedText)
                {
                    if (ch == '{' || ch == '}')
                    {
                        pos++;

                        if (pos >= text.Length)
                            return Fail(throwOnError, MissingEscapeSequenceMessage);

                        if (ch != text[pos])
                            return Fail(throwOnError, UnrecognizedEscapeSequenceMessage);
                    }
                }

                (sb ?? (sb = StringBuilderCache.GetInstance(text.Length))).Append(ch);
            }

            return new StringLiteralParserResult((sb != null)
                ? StringBuilderCache.GetStringAndFree(sb)
                : text.Substring(start, length));
        }

        private static StringLiteralParserResult ParseVerbatim(
            string text,
            int start,
            int length,
            bool throwOnError = false,
            bool isInterpolatedText = false)
        {
            StringBuilder sb = null;

            for (int pos = start; pos < start + length; pos++)
            {
                char ch = text[pos];
                if (ch == '\"')
                {
                    pos++;

                    if (pos >= text.Length)
                    {
                        return Fail(throwOnError, MissingEscapeSequenceMessage);
                    }

                    if (text[pos] == '\"')
                    {
                        ch = '\"';
                    }
                    else
                    {
                        return Fail(throwOnError, UnrecognizedEscapeSequenceMessage);
                    }
                }
                else if (isInterpolatedText)
                {
                    if (ch == '{' || ch == '}')
                    {
                        pos++;

                        if (pos >= text.Length)
                            return Fail(throwOnError, MissingEscapeSequenceMessage);

                        if (ch != text[pos])
                            return Fail(throwOnError, UnrecognizedEscapeSequenceMessage);
                    }
                }

                (sb ?? (sb = StringBuilderCache.GetInstance(text.Length))).Append(ch);
            }

            return new StringLiteralParserResult((sb != null)
                ? StringBuilderCache.GetStringAndFree(sb)
                : text.Substring(start, length));
        }

        internal static bool CanExtractSpan(string text, TextSpan span, bool isVerbatim, bool isInterpolatedText)
        {
            return CanExtractSpan(text, 0, text.Length, span, isVerbatim, isInterpolatedText);
        }

        internal static bool CanExtractSpan(
            string text,
            int start,
            int length,
            TextSpan span,
            bool isVerbatim,
            bool isInterpolatedText)
        {
            return (isVerbatim)
                ? CanExtractSpanFromVerbatim(text, start, length, span, isInterpolatedText)
                : CanExtractSpanFromRegular(text, start, length, span, isInterpolatedText);
        }

        private static bool CanExtractSpanFromRegular(
            string text,
            int start,
            int length,
            TextSpan span,
            bool isInterpolatedText)
        {
            for (int pos = start; pos < start + length; pos++)
            {
                char ch = text[pos];

                Debug.WriteLine(ch);

                if (ch == '\\')
                {
                    int startPos = pos;

                    pos++;

                    if (pos >= text.Length)
                        return false;

                    switch (text[pos])
                    {
                        case '\'':
                        case '\"':
                        case '\\':
                        case '0':
                        case 'a':
                        case 'b':
                        case 'f':
                        case 'n':
                        case 'r':
                        case 't':
                        case 'v':
                            {
                                if (IsOverlap(span, pos))
                                    return false;

                                break;
                            }
                        case 'u':
                            {
                                if (pos + 4 >= text.Length)
                                    return false;

                                if (!uint.TryParse(text.Substring(pos + 1, 4), NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo, out uint result))
                                    return false;

                                if (IsOverlap(span, startPos, 6))
                                    return false;

                                pos += 4;

                                break;
                            }
                        case 'U':
                            {
                                if (pos + 8 >= text.Length)
                                    return false;

                                if (!uint.TryParse(text.Substring(pos + 1, 8), NumberStyles.HexNumber, NumberFormatInfo.CurrentInfo, out uint result))
                                    return false;

                                if (result > 0xffff)
                                    return false;

                                if (IsOverlap(span, startPos, 10))
                                    return false;

                                pos += 8;

                                break;
                            }
                        case 'x':
                            {
                                pos++;

                                if (pos >= text.Length)
                                    return false;

                                if (!IsHexadecimalDigit(text[pos]))
                                    return false;

                                pos++;
                                if (pos < text.Length
                                    && IsHexadecimalDigit(text[pos]))
                                {
                                    pos++;
                                    if (pos < text.Length
                                        && IsHexadecimalDigit(text[pos]))
                                    {
                                        pos++;
                                        if (pos < text.Length
                                            && IsHexadecimalDigit(text[pos]))
                                        {
                                            pos++;
                                        }
                                    }
                                }

                                if (IsOverlap(span, startPos, pos - startPos))
                                    return false;

                                pos--;
                                break;
                            }
                        default:
                            {
                                return false;
                            }
                    }
                }
                else if (isInterpolatedText)
                {
                    if (ch == '{' || ch == '}')
                    {
                        pos++;

                        if (pos >= text.Length
                            || ch != text[pos]
                            || IsOverlap(span, pos))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private static bool CanExtractSpanFromVerbatim(
            string text,
            int start,
            int length,
            TextSpan span,
            bool isInterpolatedText)
        {
            for (int pos = start; pos < start + length; pos++)
            {
                char ch = text[pos];

                Debug.WriteLine(ch);

                if (ch == '\"')
                {
                    pos++;

                    if (pos >= text.Length
                        || text[pos] != '\"'
                        || IsOverlap(span, pos))
                    {
                        return false;
                    }
                }
                else if (isInterpolatedText)
                {
                    if (ch == '{' || ch == '}')
                    {
                        pos++;

                        if (pos >= text.Length
                            || ch != text[pos]
                            || IsOverlap(span, pos))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private static bool IsOverlap(TextSpan span, int index)
        {
            return IsOverlap(span, index - 1, 2);
        }

        private static bool IsOverlap(TextSpan span, int start, int length)
        {
            if (span.Start == start && span.Length == length)
            {
                return false;
            }

            return (span.Start > start && span.Start < start + length)
                || (span.End > start && span.End < start + length);
        }

        private static bool IsHexadecimalDigit(char ch)
        {
            return char.IsDigit(ch)
                || (ch >= 'a' && ch <= 'f')
                || (ch >= 'A' && ch <= 'F');
        }

        private static StringLiteralParserResult Fail(bool throwOnError, string message)
        {
            return (throwOnError)
                ? throw new ArgumentException(message)
                : default(StringLiteralParserResult);
        }

        private readonly struct StringLiteralParserResult
        {
            private StringLiteralParserResult(string text, bool success)
            {
                Text = text;
                Success = success;
            }

            public StringLiteralParserResult(string text)
            {
                Text = text;
                Success = true;
            }

            public string Text { get; }

            public bool Success { get; }
        }
    }
}
