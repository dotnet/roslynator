// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using static Roslynator.Markdown.MarkdownFactory;

namespace Roslynator.Markdown
{
    public class MarkdownBuilder
    {
        public MarkdownBuilder(MarkdownSettings settings = null)
            : this(new StringBuilder(), settings)
        {
        }

        public MarkdownBuilder(StringBuilder sb, MarkdownSettings settings = null)
        {
            Settings = settings ?? MarkdownSettings.Default;
            StringBuilder = sb;
        }

        public MarkdownSettings Settings { get; }

        public int IndentLevel { get; private set; }

        public StringBuilder StringBuilder { get; }

        public int Length => StringBuilder.Length;

        private string BoldDelimiter => Settings.BoldDelimiter;

        private string ItalicDelimiter => Settings.ItalicDelimiter;

        private string AlternativeItalicDelimiter => Settings.AlternativeItalicDelimiter;

        private string StrikethroughDelimiter => Settings.StrikethroughDelimiter;

        private string CodeDelimiter => Settings.CodeDelimiter;

        private string TableDelimiter => Settings.TableDelimiter;

        private string ListItemStart => Settings.ListItemStart;

        private string CodeBlockChars => Settings.CodeBlockChars;

        private string IndentChars => Settings.IndentChars;

        private bool AddEmptyLineBeforeHeader => Settings.EmptyLineBeforeHeader;

        private bool AddEmptyLineAfterHeader => Settings.EmptyLineAfterHeader;

        private bool AddEmptyLineBeforeCodeBlock => Settings.EmptyLineBeforeCodeBlock;

        private bool AddEmptyLineAfterCodeBlock => Settings.EmptyLineAfterCodeBlock;

        private bool FormatTableHeader => Settings.FormatTableHeader;

        private bool FormatTableContent => Settings.FormatTableContent;

        private bool UseTableOuterPipe => Settings.UseTableOuterPipe;

        private bool UseTablePadding => Settings.UseTablePadding;

        private bool AllowLinkWithoutUrl => Settings.AllowLinkWithoutUrl;

        public char this[int index]
        {
            get { return StringBuilder[index]; }
        }

        public MarkdownBuilder IncreaseIndent()
        {
            IndentLevel++;
            return this;
        }

        public MarkdownBuilder DecreaseIndent()
        {
            if (IndentLevel == 0)
                throw new InvalidOperationException($"{nameof(IndentLevel)} cannot be less than 0.");

            IndentLevel++;
            return this;
        }

        internal MarkdownBuilder AppendIndentationIf(bool condition)
        {
            if (condition)
                AppendIndentation();

            return this;
        }

        public MarkdownBuilder AppendIndentation()
        {
            for (int i = 0; i < IndentLevel; i++)
                AppendRaw(IndentChars);

            return this;
        }

        public MarkdownBuilder AppendBold(string value)
        {
            AppendDelimiter(BoldDelimiter, value);
            return this;
        }

        public MarkdownBuilder AppendBold(object value)
        {
            AppendDelimiter(BoldDelimiter, value);
            return this;
        }

        public MarkdownBuilder AppendBold(params object[] values)
        {
            AppendDelimiter(BoldDelimiter, values);
            return this;
        }

        public MarkdownBuilder AppendBoldDelimiter()
        {
            AppendRaw(BoldDelimiter);
            return this;
        }

        public MarkdownBuilder AppendItalic(string value)
        {
            AppendDelimiter(ItalicDelimiter, value);
            return this;
        }

        public MarkdownBuilder AppendItalic(object value)
        {
            AppendDelimiter(ItalicDelimiter, value);
            return this;
        }

        public MarkdownBuilder AppendItalic(params object[] values)
        {
            AppendDelimiter(ItalicDelimiter, values);
            return this;
        }

        public MarkdownBuilder AppendItalicDelimiter()
        {
            AppendRaw(ItalicDelimiter);
            return this;
        }

        public MarkdownBuilder AppendStrikethrough(string value)
        {
            AppendDelimiter(StrikethroughDelimiter, value);
            return this;
        }

        public MarkdownBuilder AppendStrikethrough(object value)
        {
            AppendDelimiter(StrikethroughDelimiter, value);
            return this;
        }

        public MarkdownBuilder AppendStrikethrough(params object[] values)
        {
            AppendDelimiter(StrikethroughDelimiter, values);
            return this;
        }

        public MarkdownBuilder AppendStrikethroughDelimiter()
        {
            AppendRaw(StrikethroughDelimiter);
            return this;
        }

        public MarkdownBuilder AppendCode(string value)
        {
            AppendDelimiter(CodeDelimiter, value);
            return this;
        }

        public MarkdownBuilder AppendCode(object value)
        {
            AppendDelimiter(CodeDelimiter, value);
            return this;
        }

        public MarkdownBuilder AppendCode(params object[] values)
        {
            AppendDelimiter(CodeDelimiter, values);
            return this;
        }

        public MarkdownBuilder AppendCodeDelimiter()
        {
            AppendRaw(CodeDelimiter);
            return this;
        }

        public MarkdownBuilder AppendHeader1(object value = null)
        {
            AppendHeader(1, value);
            return this;
        }

        public MarkdownBuilder AppendHeader2(object value = null)
        {
            AppendHeader(2, value);
            return this;
        }

        public MarkdownBuilder AppendHeader3(object value = null)
        {
            AppendHeader(3, value);
            return this;
        }

        public MarkdownBuilder AppendHeader4(object value = null)
        {
            AppendHeader(4, value);
            return this;
        }

        public MarkdownBuilder AppendHeader5(object value = null)
        {
            AppendHeader(5, value);
            return this;
        }

        public MarkdownBuilder AppendHeader6(object value = null)
        {
            AppendHeader(6, value);
            return this;
        }

        public MarkdownBuilder AppendHeader(int level, string value = null)
        {
            AppendLineMarkdown(HeaderStart(level), indent: false, emptyLineBefore: AddEmptyLineBeforeHeader, emptyLineAfter: AddEmptyLineAfterHeader, value: value);
            return this;
        }

        public MarkdownBuilder AppendHeader(int level, object value = null)
        {
            AppendLineMarkdown(HeaderStart(level), indent: false, emptyLineBefore: AddEmptyLineBeforeHeader, emptyLineAfter: AddEmptyLineAfterHeader, value: value);
            return this;
        }

        public MarkdownBuilder AppendHeader(int level, params object[] values)
        {
            AppendLineMarkdown(HeaderStart(level), indent: false, emptyLineBefore: AddEmptyLineBeforeHeader, emptyLineAfter: AddEmptyLineAfterHeader, values: values);
            return this;
        }

        public MarkdownBuilder AppendHeaderStart(int level)
        {
            AppendRaw(HeaderStart(level));
            return this;
        }

        public MarkdownBuilder AppendListItem(string value)
        {
            AppendItem(ListItemStart, value);
            return this;
        }

        public MarkdownBuilder AppendListItem(object value)
        {
            AppendItem(ListItemStart, value);
            return this;
        }

        public MarkdownBuilder AppendListItem(params object[] values)
        {
            AppendItem(ListItemStart, values);
            return this;
        }

        public MarkdownBuilder AppendListItemStart()
        {
            AppendRaw(ListItemStart);
            return this;
        }

        public MarkdownBuilder AppendOrderedListItem(int number, string value)
        {
            AppendItem(OrderedListItemStart(number), value);
            return this;
        }

        public MarkdownBuilder AppendOrderedListItem(int number, object value)
        {
            AppendItem(OrderedListItemStart(number), value);
            return this;
        }

        public MarkdownBuilder AppendOrderedListItem(int number, params object[] values)
        {
            AppendItem(OrderedListItemStart(number), values);
            return this;
        }

        public MarkdownBuilder AppendOrderedListItemStart(int number)
        {
            StringBuilder.Append(number);
            AppendRaw(". ");
            return this;
        }

        public MarkdownBuilder AppendTaskListItem(string value)
        {
            AppendItem(TaskListItemStart(), value);
            return this;
        }

        public MarkdownBuilder AppendTaskListItem(object value)
        {
            AppendItem(TaskListItemStart(), value);
            return this;
        }

        public MarkdownBuilder AppendTaskListItem(params object[] values)
        {
            AppendItem(TaskListItemStart(), values);
            return this;
        }

        public MarkdownBuilder AppendCompletedTaskListItem(string value)
        {
            AppendItem(TaskListItemStart(isCompleted: true), value);
            return this;
        }

        public MarkdownBuilder AppendCompletedTaskListItem(object value)
        {
            AppendItem(TaskListItemStart(isCompleted: true), value);
            return this;
        }

        public MarkdownBuilder AppendCompletedTaskListItem(params object[] values)
        {
            AppendItem(TaskListItemStart(isCompleted: true), values);
            return this;
        }

        public MarkdownBuilder AppendTaskListItemStart(bool isCompleted = false)
        {
            AppendRaw(TaskListItemStart(isCompleted));
            return this;
        }

        public MarkdownBuilder AppendItem(string prefix, string value)
        {
            AppendLineMarkdown(prefix, indent: true, emptyLineBefore: false, emptyLineAfter: false, value: value);
            return this;
        }

        public MarkdownBuilder AppendItem(string prefix, object value)
        {
            AppendLineMarkdown(prefix, indent: true, emptyLineBefore: false, emptyLineAfter: false, value: value);
            return this;
        }

        public MarkdownBuilder AppendItem(string prefix, params object[] values)
        {
            AppendLineMarkdown(prefix, indent: true, emptyLineBefore: false, emptyLineAfter: false, values: values);
            return this;
        }

        public MarkdownBuilder AppendImage(string text, string url)
        {
            AppendRaw("!");
            AppendLinkCore(text, url);
            return this;
        }

        public MarkdownBuilder AppendLink(string text, string url)
        {
            if (url == null)
            {
                if (!AllowLinkWithoutUrl)
                    throw new ArgumentNullException(nameof(url));

                return Append(text);
            }
            else if (url.Length == 0)
            {
                if (!AllowLinkWithoutUrl)
                    throw new ArgumentException("Url cannot be empty.", nameof(url));

                return Append(text);
            }
            else
            {
                return AppendLinkCore(text, url);
            }
        }

        private MarkdownBuilder AppendLinkCore(string text, string url)
        {
            AppendRaw("[");
            Append(text, shouldBeEscaped: f => f == '[' || f == ']');
            AppendRaw("](");
            Append(url, shouldBeEscaped: f => f == '(' || f == ')');
            AppendRaw(")");
            return this;
        }

        public MarkdownBuilder AppendCodeBlock(string code, string language = null)
        {
            AppendEmptyLineIf(AddEmptyLineBeforeCodeBlock);
            AppendIndentation();
            AppendCodeBlockChars();
            AppendLineRaw(language);
            AppendBlock(code, escape: false);
            AppendIndentation();
            AppendCodeBlockChars();
            AppendLine();
            AppendEmptyLineIf(AddEmptyLineAfterCodeBlock);
            return this;
        }

        public MarkdownBuilder AppendQuoteBlock(string value = null)
        {
            AppendBlock(value, prefix: "> ");
            return this;
        }

        private void AppendBlock(string value, string prefix = null, bool shouldEndWithNewLine = true, bool escape = true)
        {
            if (string.IsNullOrEmpty(value))
                return;

            if (IndentLevel == 0
                && prefix == null)
            {
                Append(value, escape);
            }
            else
            {
                int length = value.Length;

                for (int i = 0; i < length; i++)
                {
                    char ch = value[i];

                    if (ch == 10)
                    {
                        AppendRaw(ch);

                        if (i + 1 < length)
                        {
                            AppendIndentation();
                        }

                        AppendRaw(prefix);
                    }
                    else if (ch == 13)
                    {
                        AppendRaw(ch);

                        if (i + 1 < length)
                        {
                            ch = value[i + 1];

                            if (ch == 10)
                            {
                                AppendRaw(ch);
                                i++;
                            }
                        }

                        if (i + 1 < length)
                            AppendIndentation();
                    }
                }
            }

            if (shouldEndWithNewLine)
            {
                char last = value[value.Length - 1];

                if (last != 10
                    && last != 13)
                {
                    AppendLine();
                }
            }
        }

        public MarkdownBuilder AppendCodeBlockChars()
        {
            AppendRaw(CodeBlockChars);
            return this;
        }

        public MarkdownBuilder AppendHorizonalRule()
        {
            AppendLineRaw(Settings.HorizontalRule);
            return this;
        }

        public MarkdownBuilder AppendTable(TableHeader header1, TableHeader header2, IEnumerable<TableRow> rows)
        {
            AppendTable(new TableHeaderCollection() { header1, header2 }, rows);
            return this;
        }

        public MarkdownBuilder AppendTable(TableHeader header1, TableHeader header2, TableHeader header3, IEnumerable<TableRow> rows)
        {
            AppendTable(new TableHeaderCollection() { header1, header2, header3 }, rows);
            return this;
        }

        public MarkdownBuilder AppendTable(TableHeader header1, TableHeader header2, TableHeader header3, TableHeader header4, IEnumerable<TableRow> rows)
        {
            AppendTable(new TableHeaderCollection() { header1, header2, header3, header4 }, rows);
            return this;
        }

        public MarkdownBuilder AppendTable(TableHeader header1, TableHeader header2, TableHeader header3, TableHeader header4, TableHeader header5, IEnumerable<TableRow> rows)
        {
            AppendTable(new TableHeaderCollection() { header1, header2, header3, header4, header5 }, rows);
            return this;
        }

        public MarkdownBuilder AppendTable(TableHeaderCollection headers, IEnumerable<TableRow> rows)
        {
            AppendTable(headers, new TableRowCollection(rows));
            return this;
        }

        public MarkdownBuilder AppendTable(TableHeaderCollection headers, TableRowCollection rows)
        {
            int columnCount = headers.Count;

            if (columnCount == 0)
                return this;

            if (!FormatTableContent)
            {
                AppendTableHeader(headers, columnCount);
                AppendTableRows(rows, columnCount);
            }
            else
            {
                bool formatHeader = FormatTableHeader;

                List<int> widths = CalculateWidths((formatHeader) ? headers : null, rows, columnCount);

                AppendTableHeader(headers, columnCount, (formatHeader) ? widths : null);
                AppendTableRows(rows, columnCount, widths);
            }

            return this;
        }

        private List<int> CalculateWidths(TableHeaderCollection headers, TableRowCollection rows, int columnCount)
        {
            var widths = new List<int>();

            int index = 0;

            var mb = new MarkdownBuilder(Settings);

            if (headers != null)
            {
                foreach (TableHeader header in headers)
                {
                    mb.Append(header.Name);
                    widths.Add(mb.Length - index);
                    index = mb.Length;
                }
            }

            foreach (TableRow row in rows)
            {
                for (int i = 0; i < row.Count; i++)
                {
                    mb.Append(row[i]);
                    widths.Add(mb.Length - index);
                    index = mb.Length;
                }
            }

            int count = widths.Count;

            var maxWidths = new List<int>();

            for (int i = 0; i < columnCount; i++)
            {
                maxWidths.Add(0);

                for (int j = i; j < count; j += columnCount)
                {
                    maxWidths[i] = Math.Max(maxWidths[i], widths[j]);
                }
            }

            return maxWidths;
        }

        public MarkdownBuilder AppendTableHeader(params TableHeader[] headers)
        {
            if (headers == null)
                throw new ArgumentNullException(nameof(headers));

            int length = headers.Length;

            if (length == 0)
                return this;

            AppendTableHeader(headers, length);
            return this;
        }

        public MarkdownBuilder AppendTableHeader(IList<TableHeader> headers)
        {
            if (headers == null)
                throw new ArgumentNullException(nameof(headers));

            int count = headers.Count;

            if (count == 0)
                return this;

            AppendTableHeader(headers, count);
            return this;
        }

        internal void AppendTableHeader(IList<TableHeader> headers, int columnCount, IList<int> widths = null)
        {
            for (int i = 0; i < columnCount; i++)
            {
                AppendTableRowStart(i);

                AppendTablePadding();

                string name = headers[i].Name;

                Append(name);

                if (FormatTableHeader)
                {
                    int width = name.Length;
                    int minimalWidth = width;

                    if (FormatTableHeader)
                        minimalWidth = Math.Max(minimalWidth, 3);

                    AppendPadRight(width, widths?[i], minimalWidth);
                }

                AppendTableCellEnd(i, columnCount);
            }

            AppendLine();

            for (int i = 0; i < columnCount; i++)
            {
                TableHeader header = headers[i];

                AppendTableRowStart(i);

                if (header.Alignment == Alignment.Center)
                {
                    AppendRaw(":");
                }
                else
                {
                    AppendTablePadding();
                }

                AppendRaw("---");

                if (FormatTableHeader)
                    AppendPadRight(3, widths?[i] ?? headers[i].Name.Length, 3, '-');

                if (header.Alignment != Alignment.Left)
                {
                    AppendRaw(":");
                }
                else
                {
                    AppendTablePadding();
                }

                if (i == columnCount - 1)
                    AppendTableOuterPipe();
            }

            AppendLine();
        }

        internal void AppendTableRows(IList<TableRow> rows, int columnCount, List<int> widths = null)
        {
            foreach (TableRow row in rows)
                AppendTableRow(row, columnCount, widths);
        }

        internal void AppendTableRow(TableRow row, int columnCount, IList<int> widths = null)
        {
            for (int i = 0; i < columnCount; i++)
            {
                AppendTableRowStart(i);
                AppendTablePadding();

                object value = row[i];
                int? proposedWidth = widths?[i];

                int length = Length;

                Append(value);

                length = Length - length;

                if (FormatTableContent)
                    AppendPadRight(length, proposedWidth);

                AppendTableCellEnd(i, columnCount);
            }

            AppendLine();
        }

        private void AppendTableRowStart(int index)
        {
            if (index == 0)
            {
                AppendTableOuterPipe();
            }
            else
            {
                AppendTableDelimiter();
            }
        }

        private void AppendTableCellEnd(int index, int length)
        {
            if (index == length - 1)
            {
                if (UseTableOuterPipe)
                {
                    AppendTablePadding();
                    AppendTableDelimiter();
                }
            }
            else
            {
                AppendTablePadding();
            }
        }

        private void AppendTableOuterPipe()
        {
            if (UseTableOuterPipe)
                AppendTableDelimiter();
        }

        private void AppendTablePadding()
        {
            if (UseTablePadding)
                AppendRaw(" ");
        }

        public void AppendTableDelimiter()
        {
            AppendRaw(TableDelimiter);
        }

        private void AppendPadRight(int width, int? proposedWidth, int minimalWidth = 0, char paddingChar = ' ')
        {
            int totalWidth = Math.Max(proposedWidth ?? width, minimalWidth);

            for (int j = width; j < totalWidth; j++)
            {
                AppendRaw(paddingChar);
            }
        }

        public MarkdownBuilder AppendComment(string value)
        {
            AppendRaw("<!-- ");
            AppendRaw(value);
            AppendRaw(" -->");
            return this;
        }

        public void AppendDelimiter(EmphasisOptions options)
        {
            if (options == EmphasisOptions.None)
                return;

            if ((options & EmphasisOptions.Bold) != 0)
            {
                Append(BoldDelimiter);

                if ((options & EmphasisOptions.Italic) != 0)
                    Append(AlternativeItalicDelimiter);
            }
            else if ((options & EmphasisOptions.Italic) != 0)
            {
                Append(ItalicDelimiter);
            }

            if ((options & EmphasisOptions.Strikethrough) != 0)
                Append(StrikethroughDelimiter);

            if ((options & EmphasisOptions.Code) != 0)
                Append(CodeDelimiter);
        }

        private void AppendDelimiter(string delimiter, string value)
        {
            AppendRaw(delimiter);
            Append(value);
            AppendRaw(delimiter);
        }

        private void AppendDelimiter(string delimiter, object value)
        {
            AppendRaw(delimiter);
            Append(value);
            AppendRaw(delimiter);
        }

        private void AppendDelimiter(string delimiter, params object[] values)
        {
            AppendRaw(delimiter);
            AppendRange(values);
            AppendRaw(delimiter);
        }

        private void AppendLineMarkdown(string prefix, bool indent, bool emptyLineBefore, bool emptyLineAfter, string value)
        {
            AppendEmptyLineIf(emptyLineBefore);
            AppendIndentationIf(indent);
            AppendRaw(prefix);
            AppendLineIf(AppendInternal(value), emptyLineAfter);
        }

        private void AppendLineMarkdown(string prefix, bool indent, bool emptyLineBefore, bool emptyLineAfter, object value)
        {
            AppendEmptyLineIf(emptyLineBefore);
            AppendIndentationIf(indent);
            AppendRaw(prefix);
            AppendLineIf(AppendInternal(value), emptyLineAfter);
        }

        private void AppendLineMarkdown(string prefix, bool indent, bool emptyLineBefore, bool emptyLineAfter, params object[] values)
        {
            AppendEmptyLineIf(emptyLineBefore);
            AppendIndentationIf(indent);
            AppendRaw(prefix);
            AppendLineIf(AppendInternal(values), emptyLineAfter);
        }

        internal bool AppendInternal(string value)
        {
            int length = Length;
            Append(value);
            return length != Length;
        }

        internal bool AppendInternal(object value)
        {
            int length = Length;
            Append(value);
            return length != Length;
        }

        internal bool AppendInternal(params object[] values)
        {
            int length = Length;
            AppendRange(values);
            return length != Length;
        }

        public MarkdownBuilder Append<TMarkdown>(TMarkdown markdown) where TMarkdown : IMarkdown
        {
            return markdown.AppendTo(this);
        }

        public MarkdownBuilder Append(char value)
        {
            return Append(value, escape: true);
        }

        public MarkdownBuilder Append(char value, bool escape)
        {
            if (escape
                && Settings.ShouldBeEscaped(value))
            {
                return AppendRaw('\\');
            }

            return AppendRaw(value);
        }

        public MarkdownBuilder Append(string value)
        {
            return Append(value, escape: true);
        }

        public MarkdownBuilder Append(string value, bool escape)
        {
            if (escape)
            {
                return Append(value, Settings.ShouldBeEscaped);
            }
            else
            {
                return AppendRaw(value);
            }
        }

        public MarkdownBuilder Append(string value, EmphasisOptions options)
        {
            return Append(value, options, escape: true);
        }

        public MarkdownBuilder Append(string value, EmphasisOptions options, bool escape)
        {
            AppendDelimiter(options);
            Append(value, escape);
            AppendDelimiter(options);
            return this;
        }

        internal MarkdownBuilder Append(string value, Func<char, bool> shouldBeEscaped)
        {
            MarkdownEscaper.Escape(value, shouldBeEscaped, StringBuilder);

            return this;
        }

        public MarkdownBuilder Append(object value)
        {
            return Append(value, escape: true);
        }

        public MarkdownBuilder Append(object value, bool escape)
        {
            if (value == null)
                return this;

            if (value is IMarkdown markdown)
            {
                return markdown.AppendTo(this);
            }
            else
            {
                return Append(value.ToString(), escape: escape);
            }
        }

        internal MarkdownBuilder AppendRange(params object[] values)
        {
            foreach (object value in values)
                Append(value);

            return this;
        }

        public MarkdownBuilder AppendLine(string value, bool escape = true)
        {
            return Append(value, escape: escape).AppendLine();
        }

        public MarkdownBuilder AppendLineRaw(string value)
        {
            return AppendLine(value, escape: false);
        }

        private void AppendLineIf(bool condition, bool addEmptyLine = false)
        {
            if (!condition)
                return;

            int length = Length;

            if (length == 0)
                return;

            if (this[length - 1] != '\n')
                AppendLine();

            if (addEmptyLine)
                AppendEmptyLine();
        }

        private void AppendEmptyLineIf(bool condition)
        {
            if (condition)
                AppendEmptyLine();
        }

        private void AppendEmptyLine()
        {
            int length = Length;

            if (length == 0)
                return;

            int index = length - 1;

            char ch = this[index];

            if (ch == '\n'
                && --index >= 0)
            {
                ch = this[index];

                if (ch == '\r')
                {
                    if (--index >= 0)
                    {
                        ch = this[index];

                        if (ch == '\n')
                            return;
                    }
                }
                else if (ch == '\n')
                {
                    return;
                }
            }

            AppendLine();
        }

        public MarkdownBuilder AppendRaw(char value)
        {
            StringBuilder.Append(value);
            return this;
        }

        public MarkdownBuilder AppendRaw(string value)
        {
            StringBuilder.Append(value);
            return this;
        }

        public MarkdownBuilder AppendLine()
        {
            StringBuilder.AppendLine();
            return this;
        }

        public override string ToString()
        {
            return StringBuilder.ToString();
        }
    }
}