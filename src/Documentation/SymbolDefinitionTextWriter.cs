// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;
using Roslynator.FindSymbols;

namespace Roslynator.Documentation
{
    internal class SymbolDefinitionTextWriter : AbstractSymbolDefinitionTextWriter
    {
        private TextWriter _writer;
        private bool _pendingIndentation;

        public SymbolDefinitionTextWriter(
            TextWriter writer,
            SymbolFilterOptions filter = null,
            DefinitionListFormat format = null,
            SymbolDocumentationProvider documentationProvider = null) : base(filter, format, documentationProvider)
        {
            _writer = writer;
        }

        public override bool SupportsDocumentationComments => true;

        public override void WriteStartNamespaces()
        {
            WriteLine();
        }

        public override void WriteNamespaceSeparator()
        {
            WriteLine();
        }

        public override void WriteStartTypes()
        {
            WriteLine();
        }

        public override void WriteTypeSeparator()
        {
            WriteLine();
        }

        public override void WriteStartMembers()
        {
            WriteLine();
        }

        public override void WriteMemberSeparator()
        {
            WriteLine();
        }

        public override void WriteStartEnumMembers()
        {
            WriteLine();
        }

        public override void WriteEnumMemberSeparator()
        {
            if (Format.EmptyLineBetweenMembers)
                WriteLine();
        }

        public override void Write(SymbolDisplayPart part)
        {
            base.Write(part);

            if (part.Kind == SymbolDisplayPartKind.LineBreak)
                _pendingIndentation = true;
        }

        public override void Write(string value)
        {
            if (_pendingIndentation)
                WriteIndentation();

            _writer.Write(value);
        }

        public override void WriteLine()
        {
            _writer.WriteLine();

            _pendingIndentation = true;
        }

        protected override void WriteIndentation()
        {
            _pendingIndentation = false;

            for (int i = 0; i < Depth; i++)
            {
                Write(Format.IndentChars);
            }
        }

        public override void WriteDocumentationComment(ISymbol symbol)
        {
            IEnumerable<string> elements = DocumentationProvider?.GetXmlDocumentation(symbol)?.GetElementsAsText(skipEmptyElement: true, makeSingleLine: true);

            if (elements == null)
                return;

            foreach (string element in elements)
                WriteDocumentation(element);

            void WriteDocumentation(string element)
            {
                using (var sr = new StringReader(element))
                {
                    string line = null;

                    while ((line = sr.ReadLine()) != null)
                    {
                        WriteLine(line);
                        WriteIndentation();
                    }
                }
            }
        }

        public override void Close()
        {
            if (_writer != null)
            {
                try
                {
                    _writer.Flush();
                }
                finally
                {
                    try
                    {
                        _writer.Dispose();
                    }
                    finally
                    {
                        _writer = null;
                    }
                }
            }
        }
    }
}
