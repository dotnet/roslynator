// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using DotMarkdown;
using Microsoft.CodeAnalysis;
using Roslynator.FindSymbols;

namespace Roslynator.Documentation.Markdown
{
    internal class SymbolDefinitionMarkdownWriter : AbstractSymbolDefinitionTextWriter
    {
        private MarkdownWriter _writer;

        public SymbolDefinitionMarkdownWriter(
            MarkdownWriter writer,
            SymbolFilterOptions filter = null,
            DefinitionListFormat format = null,
            SymbolDocumentationProvider documentationProvider = null) : base(filter, format, documentationProvider)
        {
            _writer = writer;
        }

        public override bool SupportsMultilineDefinitions => false;

        public override bool SupportsDocumentationComments => false;

        public override void WriteStartAssembly(IAssemblySymbol assemblySymbol)
        {
            WriteStartBulletItem();
            base.WriteStartAssembly(assemblySymbol);
        }

        public override void WriteAssemblyDefinition(IAssemblySymbol assemblySymbol)
        {
            Write("assembly ");
            WriteLine(assemblySymbol.Identity.ToString());
            WriteEndBulletItem();

            IncreaseDepth();

            if (Format.Includes(SymbolDefinitionPartFilter.AssemblyAttributes))
                WriteAttributes(assemblySymbol);
        }

        public override void WriteAssemblySeparator()
        {
        }

        public override void WriteStartNamespaces()
        {
        }

        public override void WriteStartNamespace(INamespaceSymbol namespaceSymbol)
        {
            WriteStartBulletItem();
            base.WriteStartNamespace(namespaceSymbol);
        }

        public override void WriteNamespaceDefinition(INamespaceSymbol namespaceSymbol, SymbolDisplayFormat format = null)
        {
            base.WriteNamespaceDefinition(namespaceSymbol, format);
            WriteEndBulletItem();
        }

        public override void WriteNamespaceSeparator()
        {
        }

        public override void WriteStartTypes()
        {
        }

        public override void WriteStartType(INamedTypeSymbol typeSymbol)
        {
            WriteStartBulletItem();
            base.WriteStartType(typeSymbol);
        }

        public override void WriteTypeDefinition(INamedTypeSymbol typeSymbol, SymbolDisplayFormat format = null)
        {
            base.WriteTypeDefinition(typeSymbol, format);
            WriteEndBulletItem();
        }

        public override void WriteTypeSeparator()
        {
        }

        public override void WriteStartMembers()
        {
        }

        public override void WriteStartMember(ISymbol symbol)
        {
            WriteStartBulletItem();
            base.WriteStartMember(symbol);
        }

        public override void WriteMemberDefinition(ISymbol symbol, SymbolDisplayFormat format = null)
        {
            base.WriteMemberDefinition(symbol, format);
            WriteEndBulletItem();
        }

        public override void WriteMemberSeparator()
        {
        }

        public override void WriteStartEnumMembers()
        {
        }

        public override void WriteStartEnumMember(ISymbol symbol)
        {
            WriteStartBulletItem();
            base.WriteStartEnumMember(symbol);
        }

        public override void WriteEnumMemberDefinition(ISymbol symbol, SymbolDisplayFormat format = null)
        {
            base.WriteEnumMemberDefinition(symbol, format);
            WriteEndBulletItem();
        }

        public override void WriteEnumMemberSeparator()
        {
        }

        public override void WriteStartAttributes(ISymbol symbol)
        {
            if (symbol.Kind == SymbolKind.Assembly)
            {
                WriteStartBulletItem();
                WriteIndentation();
                Write("[");
            }
            else
            {
                base.WriteStartAttributes(symbol);
            }
        }

        public override void WriteEndAttributes(ISymbol symbol)
        {
            if (symbol.Kind == SymbolKind.Assembly)
            {
                Write("]");
                WriteEndBulletItem();
            }
            else
            {
                base.WriteEndAttributes(symbol);
            }
        }

        public override void WriteAttributeSeparator(ISymbol symbol)
        {
            if (symbol.Kind == SymbolKind.Assembly)
            {
                Write("]");
                WriteEndBulletItem();
                WriteStartBulletItem();
                WriteIndentation();
                Write("[");
            }
            else
            {
                base.WriteAttributeSeparator(symbol);
            }
        }

        private void WriteStartBulletItem()
        {
            _writer.WriteStartBulletItem();
        }

        private void WriteEndBulletItem()
        {
            _writer.WriteEndBulletItem();
        }

        public override void Write(SymbolDisplayPart part)
        {
            base.Write(part);

            Debug.Assert(part.Kind != SymbolDisplayPartKind.LineBreak, "");
        }

        public override void Write(string value)
        {
            Debug.Assert(value?.Contains("\n") != true, @"\n");
            Debug.Assert(value?.Contains("\r") != true, @"\r");

            _writer.WriteString(value);
        }

        public override void WriteLine()
        {
            _writer.WriteLine();
        }

        protected override void WriteIndentation()
        {
            if (Depth > 0)
            {
                _writer.WriteEntityRef("emsp");

                for (int i = 1; i < Depth; i++)
                {
                    Write(" | ");

                    _writer.WriteEntityRef("emsp");
                }

                Write(" ");
            }
        }

        public override void WriteDocumentationComment(ISymbol symbol)
        {
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
