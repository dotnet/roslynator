// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Roslynator.Markdown
{
    public class MarkdownSettings
    {
        public MarkdownSettings(
            string boldDelimiter = "**",
            string italicDelimiter = "*",
            string strikethroughDelimiter = "~~",
            string listItemStart = "* ",
            string tableDelimiter = "|",
            string codeDelimiter = "`",
            string codeBlockChars = "```",
            string horizontalRule = "___",
            EmptyLineOptions headerOptions = EmptyLineOptions.EmptyLineBeforeAndAfter,
            EmptyLineOptions codeBlockOptions = EmptyLineOptions.EmptyLineBeforeAndAfter,
            bool useTablePadding = true,
            bool useTableOuterPipe = true,
            TableFormatting tableFormatting = TableFormatting.Header,
            bool allowLinkWithoutUrl = true,
            string indentChars = "  ",
            Func<char, bool> shouldBeEscaped = null)
        {
            BoldDelimiter = boldDelimiter;
            ItalicDelimiter = italicDelimiter;
            StrikethroughDelimiter = strikethroughDelimiter;
            ListItemStart = listItemStart;
            TableDelimiter = tableDelimiter;
            CodeDelimiter = codeDelimiter;
            CodeBlockChars = codeBlockChars;
            HorizontalRule = horizontalRule;
            HeaderOptions = headerOptions;
            CodeBlockOptions = codeBlockOptions;
            TableFormatting = tableFormatting;
            UseTablePadding = useTablePadding;
            UseTableOuterPipe = useTableOuterPipe;
            IndentChars = indentChars;
            AllowLinkWithoutUrl = allowLinkWithoutUrl;
            ShouldBeEscaped = shouldBeEscaped ?? MarkdownEscaper.ShouldBeEscaped;
        }

        public static MarkdownSettings Default { get; } = new MarkdownSettings();

        public string BoldDelimiter { get; }

        public string AlternativeBoldDelimiter
        {
            get { return (BoldDelimiter == "**") ? "__" : "**"; }
        }

        public string ItalicDelimiter { get; }

        public string AlternativeItalicDelimiter
        {
            get { return (ItalicDelimiter == "*") ? "_" : "*"; }
        }

        public string StrikethroughDelimiter { get; }

        public string ListItemStart { get; }

        public string TableDelimiter { get; }

        public string CodeDelimiter { get; }

        public string CodeBlockChars { get; }

        public string HorizontalRule { get; }

        public EmptyLineOptions HeaderOptions { get; }

        internal bool EmptyLineBeforeHeader
        {
            get { return (HeaderOptions & EmptyLineOptions.EmptyLineBefore) != 0; }
        }

        internal bool EmptyLineAfterHeader
        {
            get { return (HeaderOptions & EmptyLineOptions.EmptyLineAfter) != 0; }
        }

        public EmptyLineOptions CodeBlockOptions { get; }

        internal bool EmptyLineBeforeCodeBlock
        {
            get { return (CodeBlockOptions & EmptyLineOptions.EmptyLineBefore) != 0; }
        }

        internal bool EmptyLineAfterCodeBlock
        {
            get { return (CodeBlockOptions & EmptyLineOptions.EmptyLineAfter) != 0; }
        }

        public TableFormatting TableFormatting { get; }

        internal bool FormatTableHeader
        {
            get { return (TableFormatting & TableFormatting.Header) != 0; }
        }

        internal bool FormatTableContent
        {
            get { return (TableFormatting & TableFormatting.Content) != 0; }
        }

        public bool UseTablePadding { get; }

        public bool UseTableOuterPipe { get; }

        public bool AllowLinkWithoutUrl { get; }

        public string IndentChars { get; }

        public Func<char, bool> ShouldBeEscaped { get; }
    }
}
