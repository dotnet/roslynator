// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator.Documentation
{
    public sealed class SymbolDocumentationProvider
    {
        private ImmutableDictionary<string, CultureInfo> _cultures = ImmutableDictionary<string, CultureInfo>.Empty;

        private ImmutableDictionary<IAssemblySymbol, MetadataReference> _assemblyToReferenceMap;

        private readonly Dictionary<ISymbol, SymbolDocumentationData> _symbolData;

        private readonly Dictionary<IAssemblySymbol, XmlDocumentation> _xmlDocumentations;

        private ImmutableArray<string> _additionalXmlDocumentationPaths;

        private ImmutableArray<XmlDocumentation> _additionalXmlDocumentations;

        internal SymbolDocumentationProvider(
            IEnumerable<Compilation> compilations,
            IEnumerable<string> additionalXmlDocumentationPaths = null)
        {
            Compilations = compilations.ToImmutableArray();
            Assemblies = compilations.Select(f => f.Assembly).ToImmutableArray();

            _symbolData = new Dictionary<ISymbol, SymbolDocumentationData>();
            _xmlDocumentations = new Dictionary<IAssemblySymbol, XmlDocumentation>();

            if (additionalXmlDocumentationPaths != null)
                _additionalXmlDocumentationPaths = additionalXmlDocumentationPaths.ToImmutableArray();
        }

        public ImmutableArray<Compilation> Compilations { get; }

        public ImmutableArray<IAssemblySymbol> Assemblies { get; }

        internal ISymbol GetFirstSymbolForDeclarationId(string id)
        {
            if (Compilations.Length == 1)
                return DocumentationCommentId.GetFirstSymbolForDeclarationId(id, Compilations[0]);

            foreach (Compilation compilation in Compilations)
            {
                ISymbol symbol = DocumentationCommentId.GetFirstSymbolForDeclarationId(id, compilation);

                if (symbol != null)
                    return symbol;
            }

            return null;
        }

        internal ISymbol GetFirstSymbolForReferenceId(string id)
        {
            if (Compilations.Length == 1)
                return DocumentationCommentId.GetFirstSymbolForReferenceId(id, Compilations[0]);

            foreach (Compilation compilation in Compilations)
            {
                ISymbol symbol = DocumentationCommentId.GetFirstSymbolForReferenceId(id, compilation);

                if (symbol != null)
                    return symbol;
            }

            return null;
        }

        public SymbolXmlDocumentation GetXmlDocumentation(ISymbol symbol, string preferredCultureName = null)
        {
            if (_symbolData.TryGetValue(symbol, out SymbolDocumentationData data)
                && data.XmlDocumentation != null)
            {
                if (object.ReferenceEquals(data.XmlDocumentation, SymbolXmlDocumentation.Default))
                    return null;

                return data.XmlDocumentation;
            }

            IAssemblySymbol assembly = symbol.ContainingAssembly;

            if (assembly != null)
            {
                SymbolXmlDocumentation xmlDocumentation = GetXmlDocumentation(assembly, preferredCultureName)?.GetXmlDocumentation(symbol);

                if (xmlDocumentation != null)
                {
                    _symbolData[symbol] = data.WithXmlDocumentation(xmlDocumentation);
                    return xmlDocumentation;
                }

                CultureInfo preferredCulture = null;

                if (preferredCultureName != null
                    && !_cultures.TryGetValue(preferredCultureName, out preferredCulture))
                {
                    preferredCulture = ImmutableInterlocked.GetOrAdd(ref _cultures, preferredCultureName, f => new CultureInfo(f));
                }

                string xml = symbol.GetDocumentationCommentXml(preferredCulture: preferredCulture, expandIncludes: true);

                if (!string.IsNullOrEmpty(xml))
                {
                    xml = XmlDocumentation.Unindent(xml);

                    XElement element = XElement.Parse(xml, LoadOptions.PreserveWhitespace);

                    xmlDocumentation = new SymbolXmlDocumentation(symbol, element);

                    _symbolData[symbol] = data.WithXmlDocumentation(xmlDocumentation);
                    return xmlDocumentation;
                }
            }

            if (!_additionalXmlDocumentationPaths.IsDefault)
            {
                if (_additionalXmlDocumentations.IsDefault)
                {
                    _additionalXmlDocumentations = _additionalXmlDocumentationPaths
                        .Select(f => XmlDocumentation.Load(f))
                        .ToImmutableArray();
                }

                string commentId = symbol.GetDocumentationCommentId();

                foreach (XmlDocumentation xmlDocumentation in _additionalXmlDocumentations)
                {
                    SymbolXmlDocumentation documentation = xmlDocumentation.GetXmlDocumentation(symbol, commentId);

                    if (documentation != null)
                    {
                        _symbolData[symbol] = data.WithXmlDocumentation(documentation);
                        return documentation;
                    }
                }
            }

            _symbolData[symbol] = data.WithXmlDocumentation(SymbolXmlDocumentation.Default);
            return null;
        }

        private XmlDocumentation GetXmlDocumentation(IAssemblySymbol assembly, string preferredCultureName = null)
        {
            if (!_xmlDocumentations.TryGetValue(assembly, out XmlDocumentation xmlDocumentation))
            {
                MetadataReference metadataReference = FindMetadataReference(assembly);

                if (metadataReference is PortableExecutableReference portableExecutableReference)
                {
                    string path = portableExecutableReference.FilePath;

                    if (preferredCultureName != null)
                    {
                        path = Path.GetDirectoryName(path);

                        path = Path.Combine(path, preferredCultureName);

                        if (Directory.Exists(path))
                        {
                            string fileName = Path.ChangeExtension(Path.GetFileNameWithoutExtension(path), "xml");

                            path = Path.Combine(path, fileName);

                            if (File.Exists(path))
                                xmlDocumentation = XmlDocumentation.Load(path);
                        }
                    }

                    if (xmlDocumentation == null)
                    {
                        path = Path.ChangeExtension(path, "xml");

                        if (File.Exists(path))
                            xmlDocumentation = XmlDocumentation.Load(path);
                    }
                }

                _xmlDocumentations[assembly] = xmlDocumentation;
            }

            return xmlDocumentation;
        }

        private MetadataReference FindMetadataReference(IAssemblySymbol assembly)
        {
            if (_assemblyToReferenceMap == null)
                Interlocked.CompareExchange(ref _assemblyToReferenceMap, Compilations.ToImmutableDictionary(f => f.Assembly, f => f.GetMetadataReference(f.Assembly)), null);

            if (_assemblyToReferenceMap.TryGetValue(assembly, out MetadataReference metadataReference))
                return metadataReference;

            foreach (Compilation compilation in Compilations)
            {
                foreach (MetadataReference externalReference in compilation.ExternalReferences)
                {
                    ISymbol assemblyOrModule = compilation.GetAssemblyOrModuleSymbol(externalReference);

                    if (SymbolEqualityComparer.Default.Equals(assembly, assemblyOrModule))
                        return ImmutableInterlocked.GetOrAdd(ref _assemblyToReferenceMap, assembly, externalReference);
                }
            }

            return null;
        }

        private readonly struct SymbolDocumentationData
        {
            public SymbolDocumentationData(
                object model,
                SymbolXmlDocumentation xmlDocumentation)
            {
                Model = model;
                XmlDocumentation = xmlDocumentation;
            }

            public object Model { get; }

            public SymbolXmlDocumentation XmlDocumentation { get; }

            public SymbolDocumentationData WithModel(object model)
            {
                return new SymbolDocumentationData(model, XmlDocumentation);
            }

            public SymbolDocumentationData WithXmlDocumentation(SymbolXmlDocumentation xmlDocumentation)
            {
                return new SymbolDocumentationData(Model, xmlDocumentation);
            }
        }
    }
}
