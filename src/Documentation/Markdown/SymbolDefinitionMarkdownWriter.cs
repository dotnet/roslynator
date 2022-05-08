// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using DotMarkdown;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp;
using Roslynator.FindSymbols;

namespace Roslynator.Documentation.Markdown
{
    internal class SymbolDefinitionMarkdownWriter : AbstractSymbolDefinitionTextWriter
    {
        private const string LocalLinkPrefix = "__roslynator-";

        private MarkdownWriter _writer;
        private readonly DocumentationUrlProvider _urlProvider;

        public SymbolDefinitionMarkdownWriter(
            MarkdownWriter writer,
            SymbolFilterOptions filter = null,
            DefinitionListFormat format = null,
            SymbolDocumentationProvider documentationProvider = null,
            INamedTypeSymbol hierarchyRoot = null,
            DocumentationUrlProvider urlProvider = null) : base(filter, format, documentationProvider, hierarchyRoot)
        {
            _writer = writer;
            _urlProvider = urlProvider;
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

        protected override void WriteAttributeName(ISymbol symbol)
        {
            WriteName(symbol, symbol.ToDisplayString(TypeSymbolDisplayFormats.Name));
        }

        public override void WriteTypeDefinition(INamedTypeSymbol typeSymbol, SymbolDisplayFormat format = null)
        {
            if (typeSymbol != null)
            {
                WriteDocumentationComment(typeSymbol);
                WriteDefinition(typeSymbol, format);
            }

            _writer.WriteRaw($"<a id=\"{DocumentationUtility.CreateLocalLink(typeSymbol, LocalLinkPrefix)}\"></a>");

            WriteLine();
            IncreaseDepth();

            WriteEndBulletItem();
        }

        protected override void WriteDefinitionName(ISymbol symbol, SymbolDisplayFormat format = null)
        {
            DocumentationUrlInfo urlInfo = _urlProvider.GetExternalUrl(symbol);

            if (urlInfo.Url != null)
            {
                WriteContainingNamespaceInTypeHierarchy(symbol);

                _writer.WriteStartBold();
                _writer.WriteLink(symbol.ToDisplayString(format ?? GetDefinitionNameFormat(symbol)), urlInfo.Url);
                _writer.WriteEndBold();
            }
            else
            {
                _writer.WriteStartBold();
                base.WriteDefinitionName(symbol, format);
                _writer.WriteEndBold();
            }
        }

        protected override void WriteSymbol(ISymbol symbol, SymbolDisplayFormat format = null, bool removeAttributeSuffix = false)
        {
            format ??= GetSymbolFormat(symbol);

            ImmutableArray<SymbolDisplayPart> parts = symbol.ToDisplayParts(format);

            int index = -1;

            for (int i = parts.Length - 1; i >= 0; i--)
            {
                if ((parts[i].IsName()
                    || (parts[i].Kind == SymbolDisplayPartKind.Keyword
                        && parts[i].Symbol is ITypeSymbol typeSymbol
                        && CSharpFacts.IsPredefinedType(typeSymbol.SpecialType)))
                    && SymbolEqualityComparer.Default.Equals(symbol, parts[i].Symbol))
                {
                    index = i;
                    break;
                }
            }

            Debug.Assert(index >= 0, $"{index} {parts.Length} {parts.ToDisplayString()}");

            if (index >= 0)
            {
                if (removeAttributeSuffix)
                    parts = SymbolDefinitionWriterHelpers.RemoveAttributeSuffix(symbol, parts);

                Write(parts, 0, index);

                WriteName(symbol, parts[index].ToString());

                Write(parts, index + 1, parts.Length - index - 1);
            }
            else
            {
                base.WriteSymbol(symbol, format, removeAttributeSuffix);
            }
        }

        private void WriteName(ISymbol symbol, string text)
        {
            if (TypeSymbols != null
                && symbol is INamedTypeSymbol typeSymbol
                && TypeSymbols.Contains(typeSymbol))
            {
                _writer.WriteLink(
                    text,
                    "#" + DocumentationUtility.CreateLocalLink(symbol, LocalLinkPrefix),
                    symbol.ToDisplayString(TypeSymbolDisplayFormats.Name_ContainingTypes_TypeParameters));
            }
            else
            {
                string url = _urlProvider.GetExternalUrl(symbol).Url;

                if (url != null)
                {
                    _writer.WriteLink(
                        text,
                        url,
                        symbol.ToDisplayString(TypeSymbolDisplayFormats.Name_ContainingTypes_TypeParameters));
                }
            }
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

                if (SymbolHierarchy.Count > 0)
                {
                    for (int i = 1; i < Depth; i++)
                    {
                        Write(" ");
                        _writer.WriteStartLink();
                        _writer.WriteEntityRef("bull");
                        _writer.WriteEndLink("#" + DocumentationUtility.CreateLocalLink(SymbolHierarchy[i], LocalLinkPrefix), SymbolHierarchy[i].ToDisplayString(TypeSymbolDisplayFormats.Name_ContainingTypes_TypeParameters));
                        Write(" ");
                        _writer.WriteEntityRef("emsp");
                    }
                }
                else
                {
                    for (int i = 1; i < Depth; i++)
                    {
                        Write(" | ");
                        _writer.WriteEntityRef("emsp");
                    }
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
