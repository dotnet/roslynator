// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp;
using Roslynator.FindSymbols;

namespace Roslynator.Documentation.Html;

internal class SymbolDefinitionHtmlWriter : SymbolDefinitionWriter
{
    private XmlWriter _writer;
    private bool _pendingIndentation;
    private ImmutableHashSet<IAssemblySymbol> _assemblies = ImmutableHashSet<IAssemblySymbol>.Empty;
    private List<TypeHierarchyItem> _typeHierarchy;

    public SymbolDefinitionHtmlWriter(
        XmlWriter writer,
        SymbolFilterOptions filter = null,
        DefinitionListFormat format = null,
        SymbolDocumentationProvider documentationProvider = null,
        INamedTypeSymbol hierarchyRoot = null,
        SourceReferenceProvider sourceReferenceProvider = null,
        DocumentationDisplayMode documentationDisplayMode = DocumentationDisplayMode.ToolTip) : base(filter, format, documentationProvider, hierarchyRoot)
    {
        _writer = writer;
        SourceReferenceProvider = sourceReferenceProvider;
        DocumentationDisplayMode = documentationDisplayMode;
    }

    public override bool SupportsMultilineDefinitions => true;

    public override bool SupportsDocumentationComments => true;

    public SourceReferenceProvider SourceReferenceProvider { get; }

    public DocumentationDisplayMode DocumentationDisplayMode { get; }

    public override void WriteDocument(IEnumerable<IAssemblySymbol> assemblies, CancellationToken cancellationToken = default)
    {
        try
        {
            _assemblies = assemblies.ToImmutableHashSet();
            base.WriteDocument(assemblies, cancellationToken);
        }
        finally
        {
            _assemblies = ImmutableHashSet<IAssemblySymbol>.Empty;
        }
    }

    public override void WriteStartDocument()
    {
        _writer.WriteRaw(@"<!DOCTYPE html>
");
        WriteStartElement("html");
        WriteStartElement("head");
        WriteStartElement("meta");
        WriteAttributeString("charset", "utf-8");
        WriteEndElement();
#if DEBUG
        _writer.WriteRaw(@"
<style type=""text/css"">
* { font-family: Consolas, Courier; }
.containing-namespace { color: gray; }
.external-type { color: gray; }
.type-hierarchy-link { text-decoration: none; }
</style>
");
#endif
        WriteEndElement();
        WriteStartElement("body");
        WriteStartElement("pre");
    }

    public override void WriteEndDocument()
    {
        WriteEndElement();
        WriteEndElement();
        WriteEndElement();
        _writer.WriteEndDocument();
    }

    public override void WriteStartAssemblies()
    {
    }

    public override void WriteEndAssemblies()
    {
    }

    public override void WriteStartAssembly(IAssemblySymbol assemblySymbol)
    {
    }

    public override void WriteAssemblyDefinition(IAssemblySymbol assemblySymbol)
    {
        Write("assembly ");
        _writer.WriteElementString("b", assemblySymbol.Identity.ToString());
        WriteLine();
        IncreaseDepth();

        if (Format.Includes(SymbolDefinitionPartFilter.AssemblyAttributes))
            WriteAttributes(assemblySymbol);
    }

    public override void WriteEndAssembly(IAssemblySymbol assemblySymbol)
    {
        DecreaseDepth();
    }

    public override void WriteAssemblySeparator()
    {
        if (Format.Includes(SymbolDefinitionPartFilter.AssemblyAttributes))
            WriteLine();
    }

    public override void WriteStartNamespaces()
    {
        WriteLine();
    }

    public override void WriteEndNamespaces()
    {
    }

    public override void WriteStartNamespace(INamespaceSymbol namespaceSymbol)
    {
        if (namespaceSymbol.IsGlobalNamespace)
            return;

        WriteLocalRef(namespaceSymbol);
        WriteStartCodeElement();
    }

    public override void WriteNamespaceDefinition(INamespaceSymbol namespaceSymbol, SymbolDisplayFormat format = null)
    {
        if (namespaceSymbol.IsGlobalNamespace)
            return;

        if (DocumentationDisplayMode == DocumentationDisplayMode.Xml)
            WriteDocumentationComment(namespaceSymbol);

        WriteDefinition(namespaceSymbol, format);
        WriteEndElement();
        WriteLine();
        IncreaseDepth();
    }

    public override void WriteEndNamespace(INamespaceSymbol namespaceSymbol)
    {
        if (namespaceSymbol.IsGlobalNamespace)
            return;

        DecreaseDepth();
    }

    public override void WriteNamespaceSeparator()
    {
        WriteLine();
    }

    public override void WriteStartTypes()
    {
        WriteLine();
    }

    public override void WriteEndTypes()
    {
    }

    public override void WriteStartType(INamedTypeSymbol typeSymbol)
    {
        if (typeSymbol is not null)
        {
            WriteLocalRef(typeSymbol);

            bool writeHierarchyLinks = Layout == SymbolDefinitionListLayout.TypeHierarchy
                && _typeHierarchy?.Count > 1;

            if (writeHierarchyLinks)
            {
                for (int i = 0; i < _typeHierarchy.Count - 1; i++)
                {
                    TypeHierarchyItem item = _typeHierarchy[i];

                    WriteStartElement("a");
                    WriteStartAttribute("href");
                    Write("#");
                    WriteLocalLink(item.Symbol);
                    WriteEndAttribute();
                    WriteStartAttribute("title");

                    if (i == _typeHierarchy.Count - 2)
                    {
                        for (int j = 0; j < _typeHierarchy.Count - 1; j++)
                        {
                            for (int k = 1; k <= j; k++)
                                Write(" ");

                            base.WriteSymbol(_typeHierarchy[j].Symbol, TypeSymbolDisplayFormats.Name_ContainingTypes_TypeParameters);

                            if (j < _typeHierarchy.Count - 2)
                                _writer.WriteWhitespace(_writer.Settings.NewLineChars);
                        }
                    }
                    else
                    {
                        base.WriteSymbol(item.Symbol, TypeSymbolDisplayFormats.Name_ContainingTypes_TypeParameters);
                    }

                    WriteEndAttribute();
                    WriteAttributeString("class", "type-hierarchy-link");
                    _writer.WriteRaw("&middot;");
                    WriteEndElement();
                    Write(" ");
                }
            }

            bool isExternal = Layout == SymbolDefinitionListLayout.TypeHierarchy
                && IsExternal(typeSymbol);

            WriteStartCodeElement(indent: !writeHierarchyLinks, isExternal: isExternal);
        }
    }

    public override void WriteTypeDefinition(INamedTypeSymbol typeSymbol, SymbolDisplayFormat format = null)
    {
        if (typeSymbol is not null)
        {
            if (DocumentationDisplayMode == DocumentationDisplayMode.Xml)
                WriteDocumentationComment(typeSymbol);

            WriteDefinition(typeSymbol, format);
            WriteEndElement();
        }

        WriteLine();
        IncreaseDepth();
    }

    public override void WriteEndType(INamedTypeSymbol typeSymbol)
    {
        DecreaseDepth();
    }

    public override void WriteTypeSeparator()
    {
        WriteLine();
    }

    public override void WriteStartMembers()
    {
        WriteLine();
    }

    public override void WriteEndMembers()
    {
    }

    public override void WriteStartMember(ISymbol symbol)
    {
        WriteStartCodeElement();
    }

    public override void WriteMemberDefinition(ISymbol symbol, SymbolDisplayFormat format = null)
    {
        if (DocumentationDisplayMode == DocumentationDisplayMode.Xml)
            WriteDocumentationComment(symbol);

        WriteDefinition(symbol, format);
        WriteEndElement();
        WriteLine();
        IncreaseDepth();
    }

    public override void WriteEndMember(ISymbol symbol)
    {
        DecreaseDepth();
    }

    public override void WriteMemberSeparator()
    {
        WriteLine();
    }

    public override void WriteStartEnumMembers()
    {
        WriteLine();
    }

    public override void WriteEndEnumMembers()
    {
    }

    public override void WriteStartEnumMember(ISymbol symbol)
    {
        WriteStartCodeElement();
    }

    public override void WriteEnumMemberDefinition(ISymbol symbol, SymbolDisplayFormat format = null)
    {
        if (DocumentationDisplayMode == DocumentationDisplayMode.Xml)
            WriteDocumentationComment(symbol);

        WriteDefinition(symbol, format);

        if (Format.Includes(SymbolDefinitionPartFilter.TrailingComma))
            Write(",");

        WriteEndElement();
        WriteLine();
        IncreaseDepth();
    }

    public override void WriteEndEnumMember(ISymbol symbol)
    {
        DecreaseDepth();
    }

    public override void WriteEnumMemberSeparator()
    {
        if (Format.EmptyLineBetweenMembers)
            WriteLine();
    }

    public override void WriteStartAttributes(ISymbol symbol)
    {
        Write("[");
    }

    public override void WriteEndAttributes(ISymbol symbol)
    {
        Write("]");
        if (symbol.Kind == SymbolKind.Assembly || SupportsMultilineDefinitions)
        {
            WriteLine();
        }
        else
        {
            Write(" ");
        }
    }

    public override void WriteStartAttribute(AttributeData attribute, ISymbol symbol)
    {
    }

    public override void WriteEndAttribute(AttributeData attribute, ISymbol symbol)
    {
    }

    public override void WriteAttributeSeparator(ISymbol symbol)
    {
        if (symbol.Kind == SymbolKind.Assembly
            || (Format.Includes(WrapListOptions.Attributes) && SupportsMultilineDefinitions))
        {
            Write("]");
            WriteLine();
            Write("[");
        }
        else
        {
            Write(", ");
        }
    }

    public override void Write(SymbolDisplayPart part)
    {
        base.Write(part);

        if (part.Kind == SymbolDisplayPartKind.LineBreak)
            _pendingIndentation = true;
    }

    public override void WriteDefinition(ISymbol symbol, SymbolDisplayFormat format)
    {
        if (Format.Includes(SymbolDefinitionPartFilter.Attributes))
            WriteAttributes(symbol);

        base.WriteDefinition(symbol, format);
    }

    protected override void WriteDefinitionName(ISymbol symbol, SymbolDisplayFormat format = null)
    {
        bool prefixWithContainingNamespace = Layout == SymbolDefinitionListLayout.TypeHierarchy
            && symbol.IsKind(SymbolKind.NamedType)
            && !symbol.ContainingNamespace.IsGlobalNamespace;

        if (prefixWithContainingNamespace)
            WriteContainingNamespace(symbol);

        WriteStartElement((IsExternal(symbol)) ? "span" : "b");

        if (DocumentationDisplayMode == DocumentationDisplayMode.ToolTip)
            WriteDocumentationCommentToolTip(symbol);

        string url = SourceReferenceProvider?.GetSourceReferences(symbol).FirstOrDefault().Url
            ?? WellKnownExternalUrlProviders.MicrosoftDocs.CreateUrl(symbol).Url;

        if (url is not null)
        {
            WriteStartElement("a");
            WriteAttributeString("href", url);
        }

        if (prefixWithContainingNamespace)
            format = TypeSymbolDisplayFormats.Name_ContainingTypes_TypeParameters;

        base.WriteDefinitionName(symbol, format);

        if (url is not null)
            WriteEndElement();

        WriteEndElement();
    }

    protected override void WriteSymbol(
        ISymbol symbol,
        SymbolDisplayFormat format = null,
        bool removeAttributeSuffix = false)
    {
        bool shouldWriteContainingNamespace;

        if (symbol.Kind == SymbolKind.Field
            && symbol.ContainingType.TypeKind == TypeKind.Enum)
        {
            base.WriteSymbol(symbol, format);
        }
        else
        {
            shouldWriteContainingNamespace = ShouldWriteContainingNamespace();

            if (IsExternal(symbol))
            {
                string url = WellKnownExternalUrlProviders.MicrosoftDocs.CreateUrl(symbol).Url;

                if (url is not null)
                {
                    if (shouldWriteContainingNamespace)
                        WriteContainingNamespace(symbol);

                    WriteStartElement("a");
                    WriteAttributeString("href", url);
                    WriteName();
                    WriteEndElement();
                }
                else
                {
                    WriteName();
                }
            }
            else
            {
                if (shouldWriteContainingNamespace)
                    WriteContainingNamespace(symbol);

                WriteStartElement("a");
                WriteStartAttribute("href");
                Write("#");
                WriteLocalLink(symbol);
                WriteEndAttribute();
                WriteName();
                WriteEndElement();
            }
        }

        bool ShouldWriteContainingNamespace()
        {
            return symbol.IsKind(SymbolKind.NamedType)
                && !symbol.ContainingNamespace.IsGlobalNamespace
                && Format.Includes(SymbolDefinitionPartFilter.ContainingNamespace)
                && !CSharpFacts.IsPredefinedType(((INamedTypeSymbol)symbol).SpecialType)
                && (format is null || format.TypeQualificationStyle == SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);
        }

        void WriteName()
        {
            if (symbol.IsKind(SymbolKind.Namespace))
            {
                format = TypeSymbolDisplayFormats.Name_ContainingTypes_Namespaces;
            }
            else if (shouldWriteContainingNamespace)
            {
                format = TypeSymbolDisplayFormats.Name_ContainingTypes_SpecialTypes;
            }

            base.WriteSymbol(symbol, format, removeAttributeSuffix: removeAttributeSuffix);
        }
    }

    protected override void WriteParameterName(ISymbol symbol, SymbolDisplayPart part)
    {
        if (DocumentationDisplayMode == DocumentationDisplayMode.ToolTip)
        {
            XElement element = DocumentationProvider?
                .GetXmlDocumentation(symbol)?
                .GetElement(WellKnownXmlTags.Param, "name", part.ToString());

            if (element is not null)
            {
                WriteStartElement("span");
                WriteStartAttribute("title");
                WriteDocumentationCommentToolTip(element);
                WriteEndAttribute();
            }

            base.WriteParameterName(symbol, part);

            if (element is not null)
                WriteEndElement();
        }
        else
        {
            base.WriteParameterName(symbol, part);
        }
    }

    private void WriteLocalRef(ISymbol symbol)
    {
        WriteStartElement("a");
        WriteStartAttribute("id");
        WriteLocalLink(symbol);
        WriteEndAttribute();
        WriteEndElement();
    }

    private void WriteLocalLink(ISymbol symbol)
    {
        WriteString(DocumentationUtility.CreateLocalLink(symbol));
    }

    internal override void WriteTypeHierarchyItem(TypeHierarchyItem item, CancellationToken cancellationToken = default)
    {
        (_typeHierarchy ??= new List<TypeHierarchyItem>()).Add(item);
        base.WriteTypeHierarchyItem(item, cancellationToken);
        _typeHierarchy.RemoveAt(_typeHierarchy.Count - 1);
    }

    private void WriteStartCodeElement(bool indent = true, bool isExternal = false)
    {
        WriteStartElement("code");
        WriteAttributeString("class", (isExternal) ? "csharp external-type" : "csharp");

        if (indent)
            WriteIndentation();
    }

    private void WriteStartElement(string name)
    {
        _writer.WriteStartElement(name);
    }

    private void WriteEndElement()
    {
        _writer.WriteEndElement();
    }

    private void WriteStartAttribute(string name)
    {
        _writer.WriteStartAttribute(name);
    }

    private void WriteEndAttribute()
    {
        _writer.WriteEndAttribute();
    }

    private void WriteAttributeString(string name, string value)
    {
        _writer.WriteAttributeString(name, value);
    }

    public override void Write(string value)
    {
        if (_pendingIndentation)
            WriteIndentation();

        WriteString(value);
    }

    private void WriteString(string text)
    {
        _writer.WriteString(text);
    }

    public override void WriteLine()
    {
        _writer.WriteWhitespace(_writer.Settings.NewLineChars);

        _pendingIndentation = true;
    }

    private void WriteIndentation()
    {
        _pendingIndentation = false;

        for (int i = 0; i < Depth; i++)
        {
            Write(Format.IndentChars);
        }
    }

    public override void WriteDocumentationComment(ISymbol symbol)
    {
        Debug.Assert(DocumentationDisplayMode == DocumentationDisplayMode.Xml, DocumentationDisplayMode.ToString());

        IEnumerable<string> elementsText = DocumentationProvider?.GetXmlDocumentation(symbol)?.GetElementsAsText(skipEmptyElement: true, makeSingleLine: true);

        if (elementsText is null)
            return;

        foreach (string elementText in elementsText)
        {
            var element = XElement.Parse(elementText, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);

            Dictionary<int, List<XElement>> elementsByLine = null;

            foreach (XElement e in element.Descendants())
            {
                switch (XmlTagMapper.GetTagOrDefault(e.Name.LocalName))
                {
                    case XmlTag.See:
                    case XmlTag.ParamRef:
                    case XmlTag.TypeParamRef:
                        {
                            int lineNumber = ((IXmlLineInfo)e).LineNumber;

                            if (elementsByLine is null)
                                elementsByLine = new Dictionary<int, List<XElement>>();

                            if (elementsByLine.TryGetValue(lineNumber, out List<XElement> elements))
                            {
                                elements.Add(e);
                            }
                            else
                            {
                                elementsByLine.Add(lineNumber, new List<XElement>() { e });
                            }

                            break;
                        }
                }
            }

            using (var sr = new StringReader(elementText))
            {
                int lineNumber = 1;

                string line = null;

                while ((line = sr.ReadLine()) is not null)
                {
                    Write("/// ");

                    if (elementsByLine is not null
                        && elementsByLine.TryGetValue(lineNumber, out List<XElement> elements))
                    {
                        int lastPos = 0;

                        foreach (XElement e in elements.OrderBy(e => ((IXmlLineInfo)e).LinePosition))
                        {
                            int linePos = ((IXmlLineInfo)e).LinePosition - 2;

                            switch (XmlTagMapper.GetTagOrDefault(e.Name.LocalName))
                            {
                                case XmlTag.ParamRef:
                                case XmlTag.TypeParamRef:
                                    {
                                        string name = e.Attribute("name")?.Value;

                                        if (name is not null)
                                        {
                                            Write(line.Substring(lastPos, linePos - lastPos));
                                            Write(name);
                                        }

                                        lastPos = linePos + e.ToString().Length;
                                        break;
                                    }
                                case XmlTag.See:
                                    {
                                        string commentId = e.Attribute("cref")?.Value;

                                        if (commentId is not null)
                                        {
                                            Write(line.Substring(lastPos, linePos - lastPos));

                                            ISymbol s = DocumentationProvider.GetFirstSymbolForDeclarationId(commentId)?.OriginalDefinition;

                                            if (s is not null)
                                            {
                                                if (s.Kind == SymbolKind.Field
                                                    && s.ContainingType.TypeKind == TypeKind.Enum)
                                                {
                                                    WriteSymbol(s.ContainingType);
                                                    Write(".");
                                                    Write(s.Name);
                                                }
                                                else
                                                {
                                                    WriteParts(s, s.ToDisplayParts(TypeSymbolDisplayFormats.Name_ContainingTypes_Namespaces_GlobalNamespace_TypeParameters));
                                                    WriteSymbol(s);
                                                }
                                            }
                                            else
                                            {
                                                Debug.Fail(commentId);
                                                Write(TextUtility.RemovePrefixFromDocumentationCommentId(commentId));
                                            }
                                        }
                                        else
                                        {
                                            string langword = e.Attribute("langword")?.Value;

                                            if (langword is not null)
                                            {
                                                Write(line.Substring(lastPos, linePos - lastPos));
                                                Write(langword);
                                            }
                                        }

                                        lastPos = linePos + e.ToString().Length;
                                        break;
                                    }
                            }
                        }

                        WriteLine(line.Substring(lastPos));
                    }
                    else
                    {
                        WriteLine(line);
                    }

                    WriteIndentation();

                    lineNumber++;
                }
            }
        }
    }

    private void WriteDocumentationCommentToolTip(ISymbol symbol)
    {
        Debug.Assert(DocumentationDisplayMode == DocumentationDisplayMode.ToolTip, DocumentationDisplayMode.ToString());

        SymbolXmlDocumentation xmlDocumentation = DocumentationProvider?.GetXmlDocumentation(symbol);

        if (xmlDocumentation is null)
            return;

        XElement summaryElement = xmlDocumentation.GetElement(WellKnownXmlTags.Summary);

        if (summaryElement is not null)
        {
            WriteStartAttribute("title");
            WriteDocumentationCommentToolTip(summaryElement);
        }

        var hasExceptions = false;

        using (IEnumerator<XElement> en = xmlDocumentation.GetElements(WellKnownXmlTags.Exception).GetEnumerator())
        {
            if (en.MoveNext())
            {
                hasExceptions = true;

                if (summaryElement is not null)
                {
                    Write("\n\n");
                }
                else
                {
                    WriteStartAttribute("title");
                }

                Write("Exceptions:");

                do
                {
                    Write("\n  ");

                    string commentId = en.Current.Attribute("cref").Value;

                    if (commentId is not null)
                    {
                        ISymbol exceptionSymbol = DocumentationProvider.GetFirstSymbolForDeclarationId(commentId);

                        if (exceptionSymbol is not null)
                        {
                            Write(exceptionSymbol.ToDisplayParts(Format.GetFormat()));
                        }
                        else
                        {
                            Write(TextUtility.RemovePrefixFromDocumentationCommentId(commentId));
                        }
                    }
                }
                while (en.MoveNext());
            }
        }

        if (summaryElement is not null
            || hasExceptions)
        {
            WriteEndAttribute();
        }
    }

    private void WriteDocumentationCommentToolTip(XElement element)
    {
        using (IEnumerator<XNode> en = element.Nodes().GetEnumerator())
        {
            if (en.MoveNext())
            {
                XNode node;
                var isFirst = true;
                bool isLast;

                do
                {
                    node = en.Current;

                    isLast = !en.MoveNext();

                    if (node is XText t)
                    {
                        string value = t.Value;
                        value = TextUtility.RemoveLeadingTrailingNewLine(value, isFirst, isLast);
                        Write(value);
                    }
                    else if (node is XElement e)
                    {
                        switch (XmlTagMapper.GetTagOrDefault(e.Name.LocalName))
                        {
                            case XmlTag.C:
                                {
                                    string value = e.Value;
                                    value = TextUtility.ToSingleLine(value);
                                    Write(value);
                                    break;
                                }
                            case XmlTag.Para:
                                {
                                    WriteLine();
                                    WriteLine();
                                    WriteDocumentationCommentToolTip(e);
                                    WriteLine();
                                    WriteLine();
                                    break;
                                }
                            case XmlTag.ParamRef:
                                {
                                    string parameterName = e.Attribute("name")?.Value;

                                    if (parameterName is not null)
                                        Write(parameterName);

                                    break;
                                }
                            case XmlTag.See:
                                {
                                    string commentId = e.Attribute("cref")?.Value;

                                    if (commentId is not null)
                                    {
                                        ISymbol symbol = DocumentationProvider.GetFirstSymbolForDeclarationId(commentId);

                                        if (symbol is not null)
                                        {
                                            Write(symbol.ToDisplayParts(Format.GetFormat()));
                                        }
                                        else
                                        {
                                            Write(TextUtility.RemovePrefixFromDocumentationCommentId(commentId));
                                        }
                                    }

                                    break;
                                }
                            case XmlTag.TypeParamRef:
                                {
                                    string typeParameterName = e.Attribute("name")?.Value;

                                    if (typeParameterName is not null)
                                        Write(typeParameterName);

                                    break;
                                }
                        }
                    }
                    else
                    {
                        Debug.Fail(node.NodeType.ToString());
                    }

                    isFirst = false;
                }
                while (!isLast);
            }
        }
    }

    private void WriteContainingNamespace(ISymbol symbol)
    {
        WriteStartElement("span");
        WriteAttributeString("class", "containing-namespace");
        Write(symbol.ContainingNamespace.ToDisplayParts(TypeSymbolDisplayFormats.Name_ContainingTypes_Namespaces));
        Write(".");
        WriteEndElement();
    }

    private bool IsExternal(ISymbol symbol)
    {
        foreach (IAssemblySymbol assembly in _assemblies)
        {
            if (symbol.ContainingAssembly.Identity.Equals(assembly.Identity))
                return false;
        }

        return true;
    }

    public override void Close()
    {
        if (_writer is not null)
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
