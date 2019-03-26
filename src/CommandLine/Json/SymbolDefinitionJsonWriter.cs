// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using Roslynator.FindSymbols;
using static Roslynator.Documentation.SymbolDefinitionWriterHelpers;

namespace Roslynator.Documentation.Json
{
    internal class SymbolDefinitionJsonWriter : SymbolDefinitionWriter
    {
        private JsonWriter _writer;
        private StringBuilder _attributeStringBuilder;

        private SymbolDefinitionWriter _definitionWriter;

        public SymbolDefinitionJsonWriter(
            JsonWriter writer,
            SymbolFilterOptions filter = null,
            DefinitionListFormat format = null,
            SymbolDocumentationProvider documentationProvider = null) : base(filter, format, documentationProvider)
        {
            _writer = writer;
        }

        public override bool SupportsMultilineDefinitions => false;

        public override bool SupportsDocumentationComments => false;

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
            WriteStartObject();
        }

        public override void WriteEndDocument()
        {
            WriteEndObject();
        }

        public override void WriteStartAssemblies()
        {
            WriteStartArray("assemblies");
        }

        public override void WriteEndAssemblies()
        {
            WriteEndArray();
        }

        public override void WriteStartAssembly(IAssemblySymbol assemblySymbol)
        {
            if (Format.Includes(SymbolDefinitionPartFilter.AssemblyAttributes)
                && HasAttributes(assemblySymbol, Filter))
            {
                WriteStartObject();
            }
        }

        public override void WriteAssemblyDefinition(IAssemblySymbol assemblySymbol)
        {
            if (Format.Includes(SymbolDefinitionPartFilter.AssemblyAttributes)
                && HasAttributes(assemblySymbol, Filter))
            {
                WriteProperty("assembly", assemblySymbol.Identity.ToString());
                WriteAttributes(assemblySymbol);
            }
            else
            {
                WriteValue(assemblySymbol.Identity.ToString());
            }
        }

        public override void WriteEndAssembly(IAssemblySymbol assemblySymbol)
        {
            if (Format.Includes(SymbolDefinitionPartFilter.AssemblyAttributes)
                && HasAttributes(assemblySymbol, Filter))
            {
                WriteEndObject();
            }
        }

        public override void WriteAssemblySeparator()
        {
        }

        public override void WriteStartNamespaces()
        {
            WriteStartArray("namespaces");
        }

        public override void WriteEndNamespaces()
        {
            WriteEndArray();
        }

        public override void WriteStartNamespace(INamespaceSymbol namespaceSymbol)
        {
            WriteStartObject();
        }

        public override void WriteNamespaceDefinition(INamespaceSymbol namespaceSymbol, SymbolDisplayFormat format = null)
        {
            WritePropertyName("namespace");

            if (!namespaceSymbol.IsGlobalNamespace)
                WriteDefinition(namespaceSymbol, format);
        }

        public override void WriteEndNamespace(INamespaceSymbol namespaceSymbol)
        {
            WriteEndObject();
        }

        public override void WriteNamespaceSeparator()
        {
        }

        public override void WriteStartTypes()
        {
            WriteStartArray("types");
        }

        public override void WriteEndTypes()
        {
            WriteEndArray();
        }

        public override void WriteStartType(INamedTypeSymbol typeSymbol)
        {
            if (HasContent(typeSymbol))
                WriteStartObject();
        }

        public override void WriteTypeDefinition(INamedTypeSymbol typeSymbol, SymbolDisplayFormat format = null)
        {
            if (HasContent(typeSymbol))
            {
                if (typeSymbol != null)
                {
                    WritePropertyName("type");
                    WriteDefinition(typeSymbol, format);

                    if (Format.Includes(SymbolDefinitionPartFilter.Attributes))
                    {
                        WriteAttributes(typeSymbol);
                        WriteParameters(GetParameters(typeSymbol));
                    }
                }
                else
                {
                    WriteProperty("type", "");
                }
            }
            else if (typeSymbol != null)
            {
                WriteDefinition(typeSymbol, format);
            }
            else
            {
                WriteValue("");
            }
        }

        public override void WriteEndType(INamedTypeSymbol typeSymbol)
        {
            if (HasContent(typeSymbol))
                WriteEndObject();
        }

        public override void WriteTypeSeparator()
        {
        }

        public override void WriteStartMembers()
        {
            WriteStartArray("members");
        }

        public override void WriteEndMembers()
        {
            WriteEndArray();
        }

        public override void WriteStartMember(ISymbol symbol)
        {
            if (Format.Includes(SymbolDefinitionPartFilter.Attributes)
                && HasAttributes(symbol, Filter))
            {
                WriteStartObject();
            }
        }

        public override void WriteMemberDefinition(ISymbol symbol, SymbolDisplayFormat format = null)
        {
            if (Format.Includes(SymbolDefinitionPartFilter.Attributes)
                && HasAttributes(symbol, Filter))
            {
                WritePropertyName("member");
                WriteDefinition(symbol, format);
                WriteAttributes(symbol);
                WriteParameters(GetParameters(symbol));

                (IMethodSymbol accessor1, IMethodSymbol accessor2) = GetAccessors(symbol);
                WriteAccessors(accessor1, accessor2);
            }
            else
            {
                WriteDefinition(symbol, format);
            }
        }

        public override void WriteEndMember(ISymbol symbol)
        {
            if (Format.Includes(SymbolDefinitionPartFilter.Attributes)
                && HasAttributes(symbol, Filter))
            {
                WriteEndObject();
            }
        }

        public override void WriteMemberSeparator()
        {
        }

        public override void WriteStartEnumMembers()
        {
            WriteStartArray("members");
        }

        public override void WriteEndEnumMembers()
        {
            WriteEndArray();
        }

        public override void WriteStartEnumMember(ISymbol symbol)
        {
            WriteStartMember(symbol);
        }

        public override void WriteEnumMemberDefinition(ISymbol symbol, SymbolDisplayFormat format = null)
        {
            if (Format.Includes(SymbolDefinitionPartFilter.Attributes)
                && HasAttributes(symbol, Filter))
            {
                WritePropertyName("member");
                WriteDefinition(symbol, format);
                WriteAttributes(symbol);
            }
            else
            {
                WriteDefinition(symbol, format);
            }
        }

        public override void WriteEndEnumMember(ISymbol symbol)
        {
            WriteEndMember(symbol);
        }

        public override void WriteEnumMemberSeparator()
        {
        }

        public override void WriteStartAttributes(ISymbol symbol)
        {
            WriteStartArray("attributes");
        }

        public override void WriteEndAttributes(ISymbol symbol)
        {
            WriteEndArray();
        }

        public override void WriteStartAttribute(AttributeData attribute, ISymbol symbol)
        {
        }

        public override void WriteAttribute(AttributeData attribute)
        {
            if (_definitionWriter == null)
            {
                _attributeStringBuilder = new StringBuilder();
                var stringWriter = new StringWriter(_attributeStringBuilder);
                _definitionWriter = new SymbolDefinitionTextWriter(stringWriter, Filter, Format, DocumentationProvider);
            }

            _definitionWriter.WriteAttribute(attribute);

            WriteValue(_attributeStringBuilder.ToString());

            _attributeStringBuilder.Clear();
        }

        public override void WriteEndAttribute(AttributeData attribute, ISymbol symbol)
        {
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
                        WriteStartArray("parameters");
                        isOpen = true;
                    }

                    WriteStartObject();
                    WriteProperty("parameter", parameter.Name);
                    WriteAttributes(parameter);
                    WriteEndObject();
                }
            }

            if (isOpen)
                WriteEndArray();
        }

        private void WriteAccessors(IMethodSymbol accessor1, IMethodSymbol accessor2)
        {
            bool isOpen = false;

            if (ShouldWriteAccessor(accessor1))
            {
                if (!isOpen)
                {
                    WriteStartArray("accessors");
                    isOpen = true;
                }

                WriteAccessor(accessor1);
            }

            if (ShouldWriteAccessor(accessor2))
            {
                if (!isOpen)
                {
                    WriteStartArray("accessors");
                    isOpen = true;
                }

                WriteAccessor(accessor2);
            }

            if (isOpen)
                WriteEndArray();

            bool ShouldWriteAccessor(IMethodSymbol accessor)
            {
                return accessor?.GetAttributes().Any(f => Filter.IsMatch(accessor, f)) == true;
            }

            void WriteAccessor(IMethodSymbol accessor)
            {
                WriteStartObject();
                WriteProperty("accessor", GetAccessorName(accessor));
                WriteAttributes(accessor);
                WriteEndObject();
            }
        }

        internal override void WriteTypeHierarchyItem(TypeHierarchyItem item, CancellationToken cancellationToken = default(CancellationToken))
        {
            INamedTypeSymbol typeSymbol = item.Symbol;

            if (HasContent(item))
            {
                WriteStartObject();

                WritePropertyName("type");
                WriteDefinition(typeSymbol, DefinitionFormat);

                if (Format.Includes(SymbolDefinitionPartFilter.Attributes))
                {
                    WriteAttributes(typeSymbol);
                    WriteParameters(GetParameters(typeSymbol));
                }

                if (!item.IsExternal)
                    WriteMembers(typeSymbol);

                if (item.HasChildren)
                {
                    WriteStartHierarchyTypes();

                    foreach (TypeHierarchyItem child in item.Children())
                    {
                        WriteTypeSeparator();
                        WriteTypeHierarchyItem(child, cancellationToken);
                    }

                    WriteEndHierarchyTypes();
                }

                WriteEndObject();
            }
            else if (typeSymbol != null)
            {
                WriteDefinition(typeSymbol, DefinitionFormat);
            }
            else
            {
                WriteValue("");
            }
        }

        private bool HasContent(TypeHierarchyItem item)
        {
            return item.HasChildren
                || HasContent(item.Symbol);
        }

        private bool HasContent(INamedTypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                return true;

            if (Format.Includes(SymbolDefinitionPartFilter.Attributes)
                && HasAttributes(typeSymbol, Filter))
            {
                return true;
            }

            switch (typeSymbol.TypeKind)
            {
                case TypeKind.Class:
                case TypeKind.Interface:
                case TypeKind.Struct:
                    {
                        if (Filter.Includes(SymbolGroupFilter.Member)
                            && typeSymbol.GetMembers().Any(f => !f.IsKind(SymbolKind.NamedType) && Filter.IsMatch(f)))
                        {
                            return true;
                        }

                        if (Layout != SymbolDefinitionListLayout.TypeHierarchy
                            && (Filter.Includes(SymbolGroupFilter.Type))
                            && typeSymbol.GetTypeMembers().Any(f => Filter.IsMatch(f)))
                        {
                            return true;
                        }

                        break;
                    }
                case TypeKind.Enum:
                    {
                        if (Filter.Includes(SymbolGroupFilter.EnumField)
                            && typeSymbol.GetMembers().Any(m => m.Kind == SymbolKind.Field && Filter.IsMatch((IFieldSymbol)m)))
                        {
                            return true;
                        }

                        break;
                    }
            }

            return false;
        }

        protected override void WriteStartHierarchyTypes()
        {
            WriteStartTypes();
        }

        protected override void WriteEndHierarchyTypes()
        {
            WriteEndTypes();
        }

        public override void WriteDefinition(ISymbol symbol, ImmutableArray<SymbolDisplayPart> parts)
        {
            if (_definitionWriter == null)
            {
                _attributeStringBuilder = new StringBuilder();
                var stringWriter = new StringWriter(_attributeStringBuilder);
                _definitionWriter = new SymbolDefinitionTextWriter(stringWriter, Filter, Format, DocumentationProvider);
            }

            _definitionWriter.WriteDefinition(symbol, parts);

            WriteValue(_attributeStringBuilder.ToString());

            _attributeStringBuilder.Clear();
        }

        public void WriteValue(string value)
        {
            Debug.Assert(value?.Contains("\n") != true, @"\n");
            Debug.Assert(value?.Contains("\r") != true, @"\r");

            _writer.WriteValue(value);
        }

        public override void Write(string value)
        {
            Debug.Assert(value?.Contains("\n") != true, @"\n");
            Debug.Assert(value?.Contains("\r") != true, @"\r");

            _writer.WriteRawValue(value);
        }

        public override void WriteLine()
        {
            throw new InvalidOperationException();
        }

        public override void WriteLine(string value)
        {
            throw new InvalidOperationException();
        }

        private void WriteStartObject()
        {
            _writer.WriteStartObject();
            IncreaseDepth();
        }

        private void WriteEndObject()
        {
            _writer.WriteEndObject();
            DecreaseDepth();
        }

        private void WriteStartArray()
        {
            _writer.WriteStartArray();
            IncreaseDepth();
        }

        private void WriteStartArray(string name)
        {
            WritePropertyName(name);
            WriteStartArray();
        }

        private void WriteEndArray()
        {
            _writer.WriteEndArray();
            DecreaseDepth();
        }

        private void WritePropertyName(string name)
        {
            _writer.WritePropertyName(name);
        }

        private void WriteProperty(string name, string value)
        {
            WritePropertyName(name);
            WriteValue(value);
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
                        ((IDisposable)_writer).Dispose();
                        _definitionWriter.Dispose();
                    }
                    finally
                    {
                        _writer = null;
                        _definitionWriter = null;
                    }
                }
            }
        }
    }
}
