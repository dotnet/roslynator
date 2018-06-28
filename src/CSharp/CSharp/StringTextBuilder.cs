// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp
{
    internal class StringTextBuilder
    {
        public StringTextBuilder(StringBuilder stringBuilder = null, bool isVerbatim = false, bool isInterpolated = false)
        {
            StringBuilder = stringBuilder ?? new StringBuilder();
            IsVerbatim = isVerbatim;
            IsInterpolated = isInterpolated;
        }

        public StringBuilder StringBuilder { get; }

        public bool IsVerbatim { get; }

        public bool IsInterpolated { get; }

        public int Length
        {
            get { return StringBuilder.Length; }
        }

        public void Append(InterpolatedStringExpressionSyntax interpolatedString)
        {
            if (interpolatedString == null)
                return;

            if (!IsInterpolated)
                throw new ArgumentException("", nameof(interpolatedString));

            bool isVerbatim = interpolatedString.IsVerbatim();

            foreach (InterpolatedStringContentSyntax content in interpolatedString.Contents)
            {
                Debug.Assert(content.IsKind(SyntaxKind.Interpolation, SyntaxKind.InterpolatedStringText), content.Kind().ToString());

                switch (content.Kind())
                {
                    case SyntaxKind.Interpolation:
                        {
                            StringBuilder.Append(content.ToFullString());
                            break;
                        }
                    case SyntaxKind.InterpolatedStringText:
                        {
                            var interpolatedText = (InterpolatedStringTextSyntax)content;

                            if (IsVerbatim == isVerbatim)
                            {
                                StringBuilder.Append(interpolatedText.TextToken.Text);
                            }
                            else
                            {
                                Append(interpolatedText.TextToken.ValueText, isVerbatim);
                            }

                            break;
                        }
                }
            }
        }

        public void Append(LiteralExpressionSyntax stringLiteral)
        {
            if (stringLiteral == null)
                return;

            if (!stringLiteral.IsKind(SyntaxKind.StringLiteralExpression))
                throw new ArgumentException("", nameof(stringLiteral));

            StringLiteralExpressionInfo literalInfo = SyntaxInfo.StringLiteralExpressionInfo(stringLiteral);
            bool isVerbatim = literalInfo.IsVerbatim;

            if (IsVerbatim == isVerbatim)
            {
                string text = literalInfo.Text;

                int length = text.Length;

                if (length == 0)
                    return;

                if (isVerbatim)
                {
                    StringBuilder.Append(text, 2, length - 3);
                }
                else
                {
                    StringBuilder.Append(text, 1, length - 2);
                }
            }
            else
            {
                Append(literalInfo.ValueText, isVerbatim);
            }
        }

        private void Append(string value, bool isVerbatim)
        {
            int length = value.Length;

            if (length == 0)
                return;

            for (int i = 0; i < length; i++)
            {
                if (IsSpecialChar(value[i]))
                {
                    StringBuilder.Append(value, 0, i);
                    AppendSpecialChar(value[i]);

                    i++;
                    int lastIndex = i;

                    while (i < length)
                    {
                        if (IsSpecialChar(value[i]))
                        {
                            StringBuilder.Append(value, lastIndex, i - lastIndex);
                            AppendSpecialChar(value[i]);

                            i++;
                            lastIndex = i;
                        }
                        else
                        {
                            i++;
                        }
                    }

                    StringBuilder.Append(value, lastIndex, length - lastIndex);
                    return;
                }
            }

            StringBuilder.Append(value);

            bool IsSpecialChar(char ch)
            {
                switch (ch)
                {
                    case '\"':
                        {
                            return true;
                        }
                    case '\\':
                        {
                            return !IsVerbatim;
                        }
                    case '{':
                    case '}':
                        {
                            return IsInterpolated;
                        }
                    case '\r':
                    case '\n':
                        {
                            return IsVerbatim && !isVerbatim;
                        }
                    default:
                        {
                            return false;
                        }
                }
            }

            void AppendSpecialChar(char ch)
            {
                switch (ch)
                {
                    case '\"':
                        {
                            if (IsVerbatim)
                            {
                                StringBuilder.Append("\"\"");
                            }
                            else
                            {
                                StringBuilder.Append("\\\"");
                            }

                            break;
                        }
                    case '\\':
                        {
                            Debug.Assert(!IsVerbatim);

                            StringBuilder.Append(@"\\");
                            break;
                        }
                    case '{':
                        {
                            StringBuilder.Append("{{");
                            break;
                        }
                    case '}':
                        {
                            StringBuilder.Append("}}");
                            break;
                        }
                    case '\r':
                        {
                            StringBuilder.Append(@"\r");
                            break;
                        }
                    case '\n':
                        {
                            StringBuilder.Append(@"\n");
                            break;
                        }
                    default:
                        {
                            Debug.Fail(ch.ToString());
                            break;
                        }
                }
            }
        }

        public void AppendStart()
        {
            if (IsInterpolated)
                StringBuilder.Append('$');

            if (IsVerbatim)
                StringBuilder.Append('@');

            StringBuilder.Append("\"");
        }

        public void AppendEnd()
        {
            StringBuilder.Append("\"");
        }

        public override string ToString()
        {
            return StringBuilder.ToString();
        }
    }
}
