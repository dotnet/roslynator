// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.CodeAnalysis;
using Roslynator.FindSymbols;
using static Roslynator.Documentation.SymbolDefinitionWriterHelpers;

namespace Roslynator.Documentation.Xml
{
    internal class SymbolDefinitionXmlWriter : SymbolDefinitionWriter
    {
        private XmlWriter _writer;

        public SymbolDefinitionXmlWriter(
            XmlWriter writer,
            SymbolFilterOptions filter = null,
            DefinitionListFormat format = null,
            SymbolDocumentationProvider documentationProvider = null) : base(filter, format, documentationProvider)
        {
            _writer = writer;
        }

        public override bool SupportsMultilineDefinitions => false;

        public override bool SupportsDocumentationComments => true;

        protected override SymbolDisplayFormat CreateDefinitionFormat(SymbolDisplayFormat format)
        {
            return format.Update(kindOptions: format.KindOptions & ~SymbolDisplayKindOptions.IncludeNamespaceKeyword);
        }

        protected override SymbolDisplayAdditionalOptions GetAdditionalOptions()
        {
            return base.GetAdditionalOptions() & ~(SymbolDisplayAdditionalOptions.IncludeAccessorAttributes
                | SymbolDisplayAdditionalOptions.IncludeParameterAttributes
                | SymbolDisplayAdditionalOptions.IncludeTrailingSemicolon);
        }

        public override void WriteStartDocument()
        {
            _writer.WriteStartDocument();
            WriteStartElement("root");
        }

        public override void WriteEndDocument()
        {
            WriteEndElement();
            _writer.WriteEndDocument();
        }

        public override void WriteStartAssemblies()
        {
            WriteStartElement("assemblies");
        }

        public override void WriteEndAssemblies()
        {
            WriteEndElement();
        }

        public override void WriteStartAssembly(IAssemblySymbol assemblySymbol)
        {
            WriteStartElement("assembly");
        }

        public override void WriteAssemblyDefinition(IAssemblySymbol assemblySymbol)
        {
            WriteAttributeString("name", assemblySymbol.Identity.ToString());

            if (Format.Includes(SymbolDefinitionPartFilter.AssemblyAttributes))
                WriteAttributes(assemblySymbol);
        }

        public override void WriteEndAssembly(IAssemblySymbol assemblySymbol)
        {
            WriteEndElement();
        }

        public override void WriteAssemblySeparator()
        {
        }

        public override void WriteStartNamespaces()
        {
            WriteStartElement("namespaces");
        }

        public override void WriteEndNamespaces()
        {
            WriteEndElement();
        }

        public override void WriteStartNamespace(INamespaceSymbol namespaceSymbol)
        {
            WriteStartElement("namespace");
        }

        public override void WriteNamespaceDefinition(INamespaceSymbol namespaceSymbol, SymbolDisplayFormat format = null)
        {
            WriteStartAttribute("name");

            if (!namespaceSymbol.IsGlobalNamespace)
                WriteDefinition(namespaceSymbol, format);

            WriteEndAttribute();
            WriteDocumentationComment(namespaceSymbol);
        }

        public override void WriteEndNamespace(INamespaceSymbol namespaceSymbol)
        {
            WriteEndElement();
        }

        public override void WriteNamespaceSeparator()
        {
        }

        public override void WriteStartTypes()
        {
            WriteStartElement("types");
        }

        public override void WriteEndTypes()
        {
            WriteEndElement();
        }

        public override void WriteStartType(INamedTypeSymbol typeSymbol)
        {
            WriteStartElement("type");
        }

        public override void WriteTypeDefinition(INamedTypeSymbol typeSymbol, SymbolDisplayFormat format = null)
        {
            if (typeSymbol != null)
            {
                WriteStartAttribute("def");
                WriteDefinition(typeSymbol, format);
                WriteEndAttribute();
                WriteDocumentationComment(typeSymbol);

                if (Format.Includes(SymbolDefinitionPartFilter.Attributes))
                {
                    WriteAttributes(typeSymbol);
                    WriteParameters(GetParameters(typeSymbol));
                }
            }
            else
            {
                WriteAttributeString("def", "");
            }
        }

        public override void WriteEndType(INamedTypeSymbol typeSymbol)
        {
            WriteEndElement();
        }

        public override void WriteTypeSeparator()
        {
        }

        public override void WriteStartMembers()
        {
            WriteStartElement("members");
        }

        public override void WriteEndMembers()
        {
            WriteEndElement();
        }

        public override void WriteStartMember(ISymbol symbol)
        {
            WriteStartElement("member");
        }

        public override void WriteMemberDefinition(ISymbol symbol, SymbolDisplayFormat format = null)
        {
            WriteStartAttribute("def");
            WriteDefinition(symbol, format);
            WriteEndAttribute();
            WriteDocumentationComment(symbol);

            if (Format.Includes(SymbolDefinitionPartFilter.Attributes))
            {
                WriteAttributes(symbol);
                WriteParameters(GetParameters(symbol));

                (IMethodSymbol accessor1, IMethodSymbol accessor2) = GetAccessors(symbol);
                WriteAccessors(accessor1, accessor2);
            }
        }

        public override void WriteEndMember(ISymbol symbol)
        {
            WriteEndElement();
        }

        public override void WriteMemberSeparator()
        {
        }

        public override void WriteStartEnumMembers()
        {
            WriteStartElement("members");
        }

        public override void WriteEndEnumMembers()
        {
            WriteEndElement();
        }

        public override void WriteStartEnumMember(ISymbol symbol)
        {
            WriteStartElement("member");
        }

        public override void WriteEnumMemberDefinition(ISymbol symbol, SymbolDisplayFormat format = null)
        {
            WriteStartAttribute("def");
            WriteDefinition(symbol, format);
            WriteEndAttribute();
            WriteDocumentationComment(symbol);

            if (Format.Includes(SymbolDefinitionPartFilter.Attributes))
                WriteAttributes(symbol);
        }

        public override void WriteEndEnumMember(ISymbol symbol)
        {
            WriteEndElement();
        }

        public override void WriteEnumMemberSeparator()
        {
        }

        public override void WriteStartAttributes(ISymbol symbol)
        {
            WriteStartElement("attributes");
        }

        public override void WriteEndAttributes(ISymbol symbol)
        {
            WriteEndElement();
        }

        public override void WriteStartAttribute(AttributeData attribute, ISymbol symbol)
        {
            WriteStartElement("attribute");
        }

        public override void WriteEndAttribute(AttributeData attribute, ISymbol symbol)
        {
            WriteEndElement();
        }

        public override void WriteAttributeSeparator(ISymbol symbol)
        {
        }

        private void WriteParameters(ImmutableArray<IParameterSymbol> parameters)
        {
            bool isOpen = false;

            foreach (IParameterSymbol parameter in parameters)
            {
                if (parameter.GetAttributes().Any(f => Filter.IsMatch(parameter, f)))
                {
                    if (!isOpen)
                    {
                        WriteStartElement("parameters");
                        isOpen = true;
                    }

                    WriteStartElement("parameter");
                    WriteAttributeString("name", parameter.Name);
                    WriteAttributes(parameter);
                    WriteEndElement();
                }
            }

            if (isOpen)
                WriteEndElement();
        }

        private void WriteAccessors(IMethodSymbol accessor1, IMethodSymbol accessor2)
        {
            bool isOpen = false;

            if (ShouldWriteAccessor(accessor1))
            {
                if (!isOpen)
                {
                    WriteStartElement("accessors");
                    isOpen = true;
                }

                WriteAccessor(accessor1);
            }

            if (ShouldWriteAccessor(accessor2))
            {
                if (!isOpen)
                {
                    WriteStartElement("accessors");
                    isOpen = true;
                }

                WriteAccessor(accessor2);
            }

            if (isOpen)
                WriteEndElement();

            bool ShouldWriteAccessor(IMethodSymbol accessor)
            {
                return accessor?.GetAttributes().Any(f => Filter.IsMatch(accessor, f)) == true;
            }

            void WriteAccessor(IMethodSymbol accessor)
            {
                WriteStartElement("accessor");
                WriteAttributeString("name", GetAccessorName(accessor));
                WriteAttributes(accessor);
                WriteEndElement();
            }
        }

        public override void Write(string value)
        {
            Debug.Assert(value?.Contains("\n") != true, @"\n");
            Debug.Assert(value?.Contains("\r") != true, @"\r");

            _writer.WriteString(value);
        }

        public override void WriteLine()
        {
            throw new InvalidOperationException();
        }

        public override void WriteLine(string value)
        {
            throw new InvalidOperationException();
        }

        private void WriteStartElement(string localName)
        {
            _writer.WriteStartElement(localName);
            IncreaseDepth();
        }

        private void WriteStartAttribute(string localName)
        {
            _writer.WriteStartAttribute(localName);
        }

        private void WriteEndElement()
        {
            _writer.WriteEndElement();
            DecreaseDepth();
        }

        private void WriteEndAttribute()
        {
            _writer.WriteEndAttribute();
        }

        private void WriteAttributeString(string name, string value)
        {
            _writer.WriteAttributeString(name, value);
        }

        public override void WriteDocumentationComment(ISymbol symbol)
        {
            IEnumerable<string> elements = DocumentationProvider?.GetXmlDocumentation(symbol)?.GetElementsAsText(skipEmptyElement: true, makeSingleLine: true);

            if (elements == null)
                return;

            using (IEnumerator<string> en = elements.GetEnumerator())
            {
                if (en.MoveNext())
                {
                    WriteStartElement("doc");
                    do
                    {
                        WriteDocumentation(en.Current);
                    }
                    while (en.MoveNext());

                    _writer.WriteWhitespace(_writer.Settings.NewLineChars);

                    for (int i = 1; i < Depth; i++)
                        _writer.WriteWhitespace(_writer.Settings.IndentChars);

                    WriteEndElement();
                }
            }

            void WriteDocumentation(string element)
            {
                using (var sr = new StringReader(element))
                {
                    string line = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        _writer.WriteWhitespace(_writer.Settings.NewLineChars);

                        for (int i = 0; i < Depth; i++)
                            _writer.WriteWhitespace(_writer.Settings.IndentChars);

                        _writer.WriteRaw(line);
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
