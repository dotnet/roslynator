// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text;

namespace Roslynator.Utilities.Markdown
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendItalic(this StringBuilder sb, string value, char character = '*')
        {
            return sb
                .Append(character)
                .Append(value)
                .Append(character);
        }

        public static StringBuilder AppendBold(this StringBuilder sb, string value, char character = '*')
        {
            return sb
                .Append(character, 2)
                .Append(value)
                .Append(character, 2);
        }

        public static StringBuilder AppendStrikethrough(this StringBuilder sb, string value)
        {
            return sb
                .Append('~', 2)
                .Append(value)
                .Append('~', 2);
        }

        public static StringBuilder AppendTableHeader(this StringBuilder sb, params MarkdownTableHeader[] columns)
        {
            if (columns == null)
                return sb;

            int length = columns.Length;

            if (length == 0)
                return sb;

            for (int i = 0; i < length; i++)
            {
                if (i > 0)
                    sb.Append(" | ");

                sb.Append(columns[i].Name);
            }

            sb.AppendLine();

            for (int i = 0; i < length; i++)
            {
                if (i > 0)
                    sb.Append("|");

                switch (columns[i].Alignment)
                {
                    case Alignment.Left:
                        {
                            if (i > 0)
                                sb.Append(' ');

                            sb.Append("---");

                            if (i < length - 1)
                                sb.Append(' ');

                            break;
                        }
                    case Alignment.Center:
                        {
                            sb.Append(":---:");
                            break;
                        }
                    case Alignment.Right:
                        {
                            if (i > 0)
                                sb.Append(' ');

                            sb.Append("---:");

                            break;
                        }
                }
            }

            return sb.AppendLine();
        }

        public static StringBuilder AppendTableRow(this StringBuilder sb, params object[] values)
        {
            if (values == null)
                return sb;

            int length = values.Length;

            if (length == 0)
                return sb;

            for (int i = 0; i < length; i++)
            {
                if (i > 0)
                    sb.Append("|");

                if (values[i] is IAppendable appendable)
                {
                    appendable.Append(sb);
                }
                else
                {
                    sb.AppendEscape(values[i]?.ToString());
                }
            }

            return sb.AppendLine();
        }

        public static StringBuilder AppendHeader2(this StringBuilder sb, string value = null)
        {
            return AppendHeader(sb, value, 2);
        }

        public static StringBuilder AppendHeader3(this StringBuilder sb, string value = null)
        {
            return AppendHeader(sb, value, 3);
        }

        public static StringBuilder AppendHeader4(this StringBuilder sb, string value = null)
        {
            return AppendHeader(sb, value, 4);
        }

        public static StringBuilder AppendHeader5(this StringBuilder sb, string value = null)
        {
            return AppendHeader(sb, value, 5);
        }

        public static StringBuilder AppendHeader6(this StringBuilder sb, string value = null)
        {
            return AppendHeader(sb, value, 6);
        }

        public static StringBuilder AppendHeader(this StringBuilder sb, string value, int level = 1)
        {
            if (level < 1
                || level > 6)
            {
                throw new ArgumentOutOfRangeException(nameof(level), level, "");
            }

            return sb
                .Append('#', level)
                .Append(" ")
                .AppendLineIf(!string.IsNullOrEmpty(value), value, escape: true);
        }

        public static StringBuilder AppendHeader(this StringBuilder sb, MarkdownHeader header)
        {
            return header.Append(sb);
        }

        public static StringBuilder AppendUnorderedListItem2(this StringBuilder sb, string value = null, string indentation = "\t")
        {
            return AppendUnorderedListItem(sb, value, indentation, 2);
        }

        public static StringBuilder AppendUnorderedListItem3(this StringBuilder sb, string value = null, string indentation = "\t")
        {
            return AppendUnorderedListItem(sb, value, indentation, 3);
        }

        public static StringBuilder AppendUnorderedListItem(this StringBuilder sb, string value = null, string indentation = "\t", int level = 1)
        {
            if (level < 1)
                throw new ArgumentOutOfRangeException(nameof(level), level, "");

            for (int i = 2; i <= level; i++)
                sb.Append(indentation);

            return sb
                .Append('*')
                .Append(" ")
                .AppendLineIf(!string.IsNullOrEmpty(value), value);
        }

        public static StringBuilder AppendLink(this StringBuilder sb, string text, string url)
        {
            return sb
                .Append("[")
                .AppendEscape(text)
                .Append("](")
                .Append(url)
                .Append(")");
        }

        public static StringBuilder AppendImage(this StringBuilder sb, string text, string url)
        {
            return sb
                .Append("![")
                .AppendEscape(text)
                .Append("](")
                .Append(url)
                .Append(")");
        }

        public static StringBuilder AppendHorizonalRule(this StringBuilder sb, char value = '_', int repeatCount = 5)
        {
            return sb
                .Append(value, repeatCount)
                .AppendLine();
        }

        public static StringBuilder AppendInlineCode(this StringBuilder sb, string code)
        {
            return sb
                .Append('`')
                .Append(code)
                .Append('`');
        }

        public static StringBuilder AppendCSharpCodeBlock(this StringBuilder sb, string code)
        {
            return AppendCodeBlock(sb, code, "csharp");
        }

        public static StringBuilder AppendCodeBlock(this StringBuilder sb, string code, string language = null)
        {
            return sb
                .Append('`', 3)
                .Append(language)
                .AppendLine()
                .Append(code)
                .AppendLineIf(!code.EndsWith("\n"))
                .Append('`', 3)
                .AppendLine();
        }

        internal static StringBuilder AppendEscape(this StringBuilder sb, string value)
        {
            return Append(sb, value, escape: true);
        }

        internal static StringBuilder Append(this StringBuilder sb, string value, bool escape)
        {
            if (value == null)
                return sb;

            if (!escape)
                return sb;

            int lastIndex = 0;

            for (int i = 0; i < value.Length; i++)
            {
                if (MarkdownUtility.ShouldBeEscaped(value[i]))
                {
                    sb.Append(value, lastIndex, i - lastIndex);
                    sb.Append('\\');
                    sb.Append(value[i]);

                    lastIndex = i + 1;
                }
            }

            sb.Append(value, lastIndex, value.Length - lastIndex);

            return sb;
        }

        internal static StringBuilder AppendLine(this StringBuilder sb, string value, bool escape)
        {
            return sb
                .Append(value, escape)
                .AppendLine();
        }

        internal static StringBuilder AppendIf(this StringBuilder sb, bool condition, string value, bool escape = false)
        {
            if (condition)
                sb.Append(value, escape);

            return sb;
        }

        internal static StringBuilder AppendLineIf(this StringBuilder sb, bool condition)
        {
            if (condition)
                sb.AppendLine();

            return sb;
        }

        internal static StringBuilder AppendLineIf(this StringBuilder sb, bool condition, string value, bool escape = false)
        {
            if (condition)
                sb.AppendLine(value, escape);

            return sb;
        }
    }
}
