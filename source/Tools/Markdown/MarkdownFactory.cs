// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Roslynator.Markdown
{
    public static class MarkdownFactory
    {
        public static MarkdownText Text(string value, EmphasisOptions options = EmphasisOptions.None)
        {
            return new MarkdownText(value, options, escape: true);
        }

        public static MarkdownText RawText(string value, EmphasisOptions options = EmphasisOptions.None)
        {
            return new MarkdownText(value, options, escape: false);
        }

        public static MarkdownText Bold(string value)
        {
            return Text(value, EmphasisOptions.Bold);
        }

        public static MarkdownText Italic(string value)
        {
            return Text(value, EmphasisOptions.Italic);
        }

        public static MarkdownText Strikethrough(string value)
        {
            return Text(value, EmphasisOptions.Strikethrough);
        }

        public static MarkdownText Code(string value)
        {
            return Text(value, EmphasisOptions.Code);
        }

        public static MarkdownJoin Join(string separator, IEnumerable<object> values, bool escape = true)
        {
            return new MarkdownJoin(separator, values, escape);
        }

        public static Header Header(string value, int level)
        {
            return new Header(value, level);
        }

        public static Header Header1(string value = null)
        {
            return new Header(value, 1);
        }

        public static Header Header2(string value = null)
        {
            return new Header(value, 2);
        }

        public static Header Header3(string value = null)
        {
            return new Header(value, 3);
        }

        public static Header Header4(string value = null)
        {
            return new Header(value, 4);
        }

        public static Header Header5(string value = null)
        {
            return new Header(value, 5);
        }

        public static Header Header6(string value = null)
        {
            return new Header(value, 6);
        }

        public static string HeaderStart(int level)
        {
            switch (level)
            {
                case 1:
                    return "# ";
                case 2:
                    return "## ";
                case 3:
                    return "### ";
                case 4:
                    return "#### ";
                case 5:
                    return "##### ";
                case 6:
                    return "###### ";
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, "Header level cannot be less than 1 or greater than 6");
            }
        }

        public static TableHeader TableHeader(string name, Alignment alignment = Alignment.Left)
        {
            return new TableHeader(name, alignment);
        }

        public static ListItem ListItem(string value = null)
        {
            return new ListItem(value);
        }

        public static OrderedListItem OrderedListItem(int number, string value = null)
        {
            return new OrderedListItem(number, value);
        }

        public static string OrderedListItemStart(int number)
        {
            switch (number)
            {
                case 1:
                    return "1. ";
                case 2:
                    return "2. ";
                case 3:
                    return "3. ";
                case 4:
                    return "4. ";
                case 5:
                    return "5. ";
                case 6:
                    return "6. ";
                case 7:
                    return "7. ";
                case 8:
                    return "8. ";
                case 9:
                    return "9. ";
                default:
                    return number.ToString() + ". ";
            }
        }

        public static TaskListItem TaskListItem(string value = null)
        {
            return new TaskListItem(value);
        }

        public static TaskListItem CompletedTaskListItem(string value = null)
        {
            return new TaskListItem(value, isCompleted: true);
        }

        public static string TaskListItemStart(bool isCompleted = false)
        {
            if (isCompleted)
            {
                return "- [x] ";
            }
            else
            {
                return "- [ ] ";
            }
        }

        public static Link Link(string text, string url)
        {
            return new Link(text, url);
        }

        public static Image Image(string text, string url)
        {
            return new Image(text, url);
        }

        public static CodeBlock CodeBlock(string value, string language = null)
        {
            return new CodeBlock(value, language);
        }

        public static QuoteBlock QuoteBlock(string value)
        {
            return new QuoteBlock(value);
        }

        public static MarkdownText HorizontalRule()
        {
            return RawText(MarkdownSettings.Default.HorizontalRule);
        }

        public static Table Table()
        {
            return new Table();
        }

        public static Table Table(params TableHeader[] headers)
        {
            Table table = Table();

            table.AddHeaders(headers);

            return table;
        }

        public static Table Table(TableHeader header1, TableHeader header2)
        {
            Table table = Table();

            table.AddHeaders(header1, header2);

            return table;
        }

        public static Table Table(TableHeader header1, TableHeader header2, TableHeader header3)
        {
            Table table = Table();

            table.AddHeaders(header1, header2, header3);

            return table;
        }

        public static Table Table(TableHeader header1, TableHeader header2, TableHeader header3, TableHeader header4)
        {
            Table table = Table();

            table.AddHeaders(header1, header2, header3, header4);

            return table;
        }

        public static Table Table(TableHeader header1, TableHeader header2, TableHeader header3, TableHeader header4, TableHeader header5)
        {
            var table = new Table();

            table.AddHeaders(header1, header2, header3, header4, header5);

            return table;
        }

        public static TableRow TableRow()
        {
            return new TableRow();
        }

        public static TableRow TableRow(params object[] values)
        {
            TableRow row = TableRow();

            foreach (object value in values)
                row.Add(value);

            return row;
        }

        public static TableRow TableRow(object value)
        {
            return new TableRow() { value };
        }

        public static TableRow TableRow(object value1, object value2)
        {
            return new TableRow() { value1, value2 };
        }

        public static TableRow TableRow(object value1, object value2, object value3)
        {
            return new TableRow() { value1, value2, value3 };
        }

        public static TableRow TableRow(object value1, object value2, object value3, object value4)
        {
            return new TableRow() { value1, value2, value3, value4 };
        }

        public static TableRow TableRow(object value1, object value2, object value3, object value4, object value5)
        {
            return new TableRow() { value1, value2, value3, value4, value5 };
        }
    }
}
