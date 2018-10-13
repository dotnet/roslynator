// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text;
using DotMarkdown;
using Microsoft.CodeAnalysis;

namespace Roslynator.Documentation.Markdown
{
    public class MarkdownDocumentationWriter : DocumentationWriter
    {
        private readonly MarkdownWriter _writer;

        public MarkdownDocumentationWriter(
            DocumentationModel documentationModel,
            DocumentationUrlProvider urlProvider,
            DocumentationOptions options = null,
            DocumentationResources resources = null) : base(documentationModel, urlProvider, options, resources)
        {
            _writer = MarkdownWriter.Create(new StringBuilder());
        }

        public override void WriteStartDocument()
        {
        }

        public override void WriteEndDocument()
        {
        }

        public override void WriteStartBold() => _writer.WriteStartBold();

        public override void WriteEndBold() => _writer.WriteEndBold();

        public override void WriteStartItalic() => _writer.WriteStartItalic();

        public override void WriteEndItalic() => _writer.WriteEndItalic();

        public override void WriteStartStrikethrough() => _writer.WriteStartStrikethrough();

        public override void WriteEndStrikethrough() => _writer.WriteEndStrikethrough();

        public override void WriteInlineCode(string text) => _writer.WriteInlineCode(text);

        public override void WriteStartHeading(int level) => _writer.WriteStartHeading(level);

        public override void WriteEndHeading() => _writer.WriteEndHeading();

        public override void WriteStartBulletList()
        {
        }

        public override void WriteEndBulletList()
        {
        }

        public override void WriteStartBulletItem() => _writer.WriteStartBulletItem();

        public override void WriteEndBulletItem() => _writer.WriteEndBulletItem();

        public override void WriteStartOrderedList()
        {
        }

        public override void WriteEndOrderedList()
        {
        }

        public override void WriteStartOrderedItem(int number) => _writer.WriteStartOrderedItem(number);

        public override void WriteEndOrderedItem() => _writer.WriteEndOrderedItem();

        public override void WriteImage(string text, string url, string title = null) => _writer.WriteImage(text, url, title);

        public override void WriteStartLink() => _writer.WriteStartLink();

        public override void WriteEndLink(string url, string title = null) => _writer.WriteEndLink(url, title);

        public override void WriteLink(string text, string url, string title = null) => _writer.WriteLink(text, url, title);

        public override void WriteCodeBlock(string text, string language = null)
        {
            _writer.WriteFencedCodeBlock(text, GetLanguageIdentifier());

            string GetLanguageIdentifier()
            {
                switch (language)
                {
                    case LanguageNames.CSharp:
                        return LanguageIdentifiers.CSharp;
                    case LanguageNames.VisualBasic:
                        return LanguageIdentifiers.VisualBasic;
                    case LanguageNames.FSharp:
                        return LanguageIdentifiers.FSharp;
                }

                return null;
            }
        }

        public override void WriteStartBlockQuote() => _writer.WriteStartBlockQuote();

        public override void WriteEndBlockQuote() => _writer.WriteEndBlockQuote();

        public override void WriteHorizontalRule() => _writer.WriteHorizontalRule();

        public override void WriteStartTable(int columnCount) => _writer.WriteStartTable(columnCount);

        public override void WriteEndTable() => _writer.WriteEndTable();

        public override void WriteStartTableRow() => _writer.WriteStartTableRow();

        public override void WriteEndTableRow() => _writer.WriteEndTableRow();

        public override void WriteStartTableCell() => _writer.WriteStartTableCell();

        public override void WriteEndTableCell() => _writer.WriteEndTableCell();

        public override void WriteTableCell(string text) => _writer.WriteTableCell(text);

        public override void WriteTableHeaderSeparator() => _writer.WriteTableHeaderSeparator();

        public override void WriteCharEntity(char value) => _writer.WriteCharEntity(value);

        public override void WriteEntityRef(string name) => _writer.WriteEntityRef(name);

        public override void WriteComment(string text) => _writer.WriteComment(text);

        public override void Flush() => _writer.Flush();

        public override void WriteString(string text) => _writer.WriteString(text);

        public override void WriteRaw(string data) => _writer.WriteRaw(data);

        public override void WriteLine() => _writer.WriteLine();

        public override void WriteLineBreak()
        {
            WriteRaw(@"\");
            WriteLine();
        }

        public override void WriteLinkDestination(string name)
        {
            WriteRaw($"<a name=\"{name}\"></a>");
        }

        public override string ToString()
        {
            return _writer.ToString();
        }

        public override void Close()
        {
            if (_writer.WriteState != WriteState.Closed)
                _writer.Close();
        }
    }
}
