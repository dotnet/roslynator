// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using static Roslynator.Markdown.MarkdownFactory;

namespace Roslynator.Markdown
{
    public class Table : IMarkdown
    {
        internal Table()
        {
        }

        public TableHeaderCollection Headers { get; } = new TableHeaderCollection();

        public TableRowCollection Rows { get; } = new TableRowCollection();

        public MarkdownBuilder AppendTo(MarkdownBuilder mb)
        {
            return mb.AppendTable(Headers, Rows);
        }

        public Table AddHeaders(params TableHeader[] headers)
        {
            foreach (TableHeader header in headers)
                Headers.Add(header);

            return this;
        }

        public Table AddHeader(TableHeader header)
        {
            Headers.Add(header);
            return this;
        }

        public Table AddHeaders(TableHeader header1, TableHeader header2)
        {
            Headers.Add(header1);
            Headers.Add(header2);
            return this;
        }

        public Table AddHeaders(TableHeader header1, TableHeader header2, TableHeader header3)
        {
            Headers.Add(header1);
            Headers.Add(header2);
            Headers.Add(header3);
            return this;
        }

        public Table AddHeaders(TableHeader header1, TableHeader header2, TableHeader header3, TableHeader header4)
        {
            Headers.Add(header1);
            Headers.Add(header2);
            Headers.Add(header3);
            Headers.Add(header4);
            return this;
        }

        public Table AddHeaders(TableHeader header1, TableHeader header2, TableHeader header3, TableHeader header4, TableHeader header5)
        {
            Headers.Add(header1);
            Headers.Add(header2);
            Headers.Add(header3);
            Headers.Add(header4);
            Headers.Add(header5);
            return this;
        }

        public Table AddRowIf(bool condition, params object[] values)
        {
            return (condition) ? AddRow(values) : this;
        }

        public Table AddRow(params object[] values)
        {
            Rows.Add(TableRow(values));
            return this;
        }

        public Table AddRow(object value)
        {
            Rows.Add(TableRow(value));
            return this;
        }

        public Table AddRow(object value1, object value2)
        {
            Rows.Add(TableRow(value1, value2));
            return this;
        }

        public Table AddRow(object value1, object value2, object value3)
        {
            Rows.Add(TableRow(value1, value2, value3));
            return this;
        }

        public Table AddRow(object value1, object value2, object value3, object value4)
        {
            Rows.Add(TableRow(value1, value2, value3, value4));
            return this;
        }

        public Table AddRow(object value1, object value2, object value3, object value4, object value5)
        {
            Rows.Add(TableRow(value1, value2, value3, value4, value5));
            return this;
        }

        public Table AddRows(IEnumerable<TableRow> rows)
        {
            Rows.AddRange(rows);
            return this;
        }
    }
}