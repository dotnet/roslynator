// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Roslynator.CSharp;

namespace Roslynator.Documentation
{
    public abstract class DocumentationWriter : IDisposable
    {
        private bool _disposed;

        protected DocumentationWriter(
            DocumentationModel documentationModel,
            DocumentationUrlProvider urlProvider,
            DocumentationOptions options = null,
            DocumentationResources resources = null)
        {
            DocumentationModel = documentationModel;
            UrlProvider = urlProvider;
            Options = options ?? DocumentationOptions.Default;
            Resources = resources ?? DocumentationResources.Default;
        }

        public DocumentationModel DocumentationModel { get; }

        internal bool CanCreateTypeLocalUrl { get; set; } = true;

        internal bool CanCreateMemberLocalUrl { get; set; } = true;

        internal ISymbol CurrentSymbol { get; set; }

        public DocumentationOptions Options { get; }

        public DocumentationResources Resources { get; }

        public DocumentationUrlProvider UrlProvider { get; }

        private SymbolXmlDocumentation GetXmlDocumentation(ISymbol symbol)
        {
            return DocumentationModel.GetXmlDocumentation(symbol, Options.PreferredCultureName);
        }

        public abstract void WriteStartDocument();

        public abstract void WriteEndDocument();

        public abstract void WriteStartBold();

        public abstract void WriteEndBold();

        public virtual void WriteBold(string text)
        {
            WriteStartBold();
            WriteString(text);
            WriteEndBold();
        }

        public abstract void WriteStartItalic();

        public abstract void WriteEndItalic();

        public virtual void WriteItalic(string text)
        {
            WriteStartItalic();
            WriteString(text);
            WriteEndItalic();
        }

        public abstract void WriteStartStrikethrough();

        public abstract void WriteEndStrikethrough();

        public virtual void WriteStrikethrough(string text)
        {
            WriteStartStrikethrough();
            WriteString(text);
            WriteEndStrikethrough();
        }

        public abstract void WriteInlineCode(string text);

        public abstract void WriteStartHeading(int level);

        public abstract void WriteEndHeading();

        public virtual void WriteHeading1(string text)
        {
            WriteHeading(1, text);
        }

        public virtual void WriteHeading2(string text)
        {
            WriteHeading(2, text);
        }

        public virtual void WriteHeading3(string text)
        {
            WriteHeading(3, text);
        }

        public virtual void WriteHeading4(string text)
        {
            WriteHeading(4, text);
        }

        public virtual void WriteHeading5(string text)
        {
            WriteHeading(5, text);
        }

        public virtual void WriteHeading6(string text)
        {
            WriteHeading(6, text);
        }

        public virtual void WriteHeading(int level, string text)
        {
            WriteStartHeading(level);
            WriteString(text);
            WriteEndHeading();
        }

        public abstract void WriteStartBulletList();

        public abstract void WriteEndBulletList();

        public abstract void WriteStartBulletItem();

        public abstract void WriteEndBulletItem();

        public virtual void WriteBulletItem(string text)
        {
            WriteStartBulletItem();
            WriteString(text);
            WriteEndBulletItem();
        }

        public abstract void WriteStartOrderedList();

        public abstract void WriteEndOrderedList();

        public abstract void WriteStartOrderedItem(int number);

        public abstract void WriteEndOrderedItem();

        public virtual void WriteOrderedItem(int number, string text)
        {
            if (number < 0)
                throw new ArgumentOutOfRangeException(nameof(number), number, "Item number must be greater than or equal to 0.");

            WriteStartOrderedItem(number);
            WriteString(text);
            WriteEndOrderedItem();
        }

        public abstract void WriteImage(string text, string url, string title = null);

        public abstract void WriteStartLink();

        public abstract void WriteEndLink(string url, string title = null);

        public abstract void WriteLink(string text, string url, string title = null);

        public void WriteLinkOrText(string text, string url = null, string title = null)
        {
            if (!string.IsNullOrEmpty(url))
            {
                WriteLink(text, url, title);
            }
            else
            {
                WriteString(text);
            }
        }

        public abstract void WriteCodeBlock(string text, string language = null);

        public abstract void WriteStartBlockQuote();

        public abstract void WriteEndBlockQuote();

        public virtual void WriteBlockQuote(string text)
        {
            WriteStartBlockQuote();
            WriteString(text);
            WriteEndBlockQuote();
        }

        public abstract void WriteHorizontalRule();

        public abstract void WriteStartTable(int columnCount);

        public abstract void WriteEndTable();

        public abstract void WriteStartTableRow();

        public abstract void WriteEndTableRow();

        public abstract void WriteStartTableCell();

        public abstract void WriteEndTableCell();

        public abstract void WriteTableCell(string text);

        public abstract void WriteTableHeaderSeparator();

        public abstract void WriteCharEntity(char value);

        public abstract void WriteEntityRef(string name);

        public abstract void WriteComment(string text);

        public abstract void Flush();

        public abstract void WriteString(string text);

        public abstract void WriteRaw(string data);

        public abstract void WriteLine();

        public abstract void WriteLineBreak();

        public abstract void WriteLinkDestination(string name);

        public virtual void WriteValue(bool value)
        {
            WriteString((value) ? Resources.TrueValue : Resources.FalseValue);
        }

        public virtual void WriteValue(int value)
        {
            WriteString(value.ToString(null, CultureInfo.InvariantCulture));
        }

        public virtual void WriteValue(long value)
        {
            WriteString(value.ToString(null, CultureInfo.InvariantCulture));
        }

        public virtual void WriteValue(float value)
        {
            WriteString(value.ToString(null, CultureInfo.InvariantCulture));
        }

        public virtual void WriteValue(double value)
        {
            WriteString(value.ToString(null, CultureInfo.InvariantCulture));
        }

        public virtual void WriteValue(decimal value)
        {
            WriteString(value.ToString(null, CultureInfo.InvariantCulture));
        }

        internal void WriteSpace()
        {
            WriteString(" ");
        }

        internal void WriteSymbol(ISymbol symbol, SymbolDisplayFormat format = null, SymbolDisplayAdditionalMemberOptions additionalOptions = SymbolDisplayAdditionalMemberOptions.None)
        {
            WriteString(symbol.ToDisplayString(format, additionalOptions));
        }

        internal void WriteTypeSymbol(
            INamedTypeSymbol typeSymbol,
            bool includeContainingNamespace = true,
            bool includeContainingTypes = true)
        {
            if (includeContainingNamespace
                && !typeSymbol.ContainingNamespace.IsGlobalNamespace)
            {
                WriteNamespaceSymbol(typeSymbol.ContainingNamespace);
                WriteString(".");
            }

            SymbolDisplayFormat format = (includeContainingTypes)
                ? SymbolDisplayFormats.TypeNameAndContainingTypesAndTypeParameters
                : SymbolDisplayFormats.TypeNameAndTypeParameters;

            WriteString(typeSymbol.ToDisplayString(format));
        }

        internal void WriteNamespaceSymbol(INamespaceSymbol namespaceSymbol)
        {
            WriteString(namespaceSymbol.ToDisplayString(SymbolDisplayFormats.TypeNameAndContainingTypesAndNamespaces));
        }

        public virtual void WriteContent(IEnumerable<string> names, bool addLinkToRoot = false, bool beginWithSeparator = false)
        {
            IEnumerator<string> en = names.GetEnumerator();

            if (addLinkToRoot)
            {
                if (beginWithSeparator)
                    WriteContentSeparator();

                WriteLink(Resources.HomeTitle, UrlProvider.GetUrlToRoot(UrlProvider.GetFolders(CurrentSymbol).Length, '/', scrollToContent: Options.ScrollToContent));
            }

            if (en.MoveNext())
            {
                if (addLinkToRoot || beginWithSeparator)
                {
                    WriteContentSeparator();
                }

                while (true)
                {
                    WriteLink(en.Current, UrlProvider.GetFragment(en.Current));

                    if (en.MoveNext())
                    {
                        WriteContentSeparator();
                    }
                    else
                    {
                        break;
                    }
                }

                WriteLine();
                WriteLine();
            }
            else if (addLinkToRoot)
            {
                WriteLine();
                WriteLine();
            }
        }

        internal void WriteContentSeparator()
        {
            WriteSpace();
            WriteCharEntity(Resources.InlineSeparatorChar);
            WriteSpace();
        }

        internal void WriteMemberTitle(ISymbol symbol, bool isOverloaded)
        {
            WriteStartHeading(1);

            if (symbol.Kind == SymbolKind.Method
                && ((IMethodSymbol)symbol).MethodKind == MethodKind.Constructor)
            {
                if (isOverloaded)
                {
                    WriteString(symbol.ContainingType.ToDisplayString(SymbolDisplayFormats.TypeNameAndContainingTypesAndTypeParameters));
                    WriteSpace();
                    WriteString(Resources.ConstructorsTitle);
                }
                else
                {
                    WriteString(symbol.ToDisplayString(SymbolDisplayFormats.SimpleDeclaration));
                    WriteSpace();
                    WriteString(Resources.ConstructorTitle);
                }
            }
            else
            {
                SymbolDisplayFormat format = (isOverloaded)
                    ? SymbolDisplayFormats.OverloadedMemberTitle
                    : SymbolDisplayFormats.MemberTitle;

                WriteString(symbol.ToDisplayString(format, SymbolDisplayAdditionalMemberOptions.UseItemPropertyName | SymbolDisplayAdditionalMemberOptions.UseOperatorName));
                WriteSpace();
                WriteString(Resources.GetName(symbol));
            }

            WriteEndHeading();
        }

        public virtual void WriteContainingNamespace(INamespaceSymbol namespaceSymbol, string title)
        {
            WriteBold(title);
            WriteString(Resources.Colon);
            WriteSpace();
            WriteLink(namespaceSymbol, SymbolDisplayFormats.TypeNameAndContainingTypesAndNamespaces);
            WriteLine();
            WriteLine();
        }

        public virtual void WriteContainingType(INamedTypeSymbol typeSymbol, string title)
        {
            WriteBold(title);
            WriteString(Resources.Colon);
            WriteSpace();
            WriteTypeLink(typeSymbol, includeContainingNamespace: Options.IncludeContainingNamespace(OmitContainingNamespaceParts.ContainingType));
            WriteLine();
            WriteLine();
        }

        public virtual void WriteContainingAssembly(IAssemblySymbol assemblySymbol, string title)
        {
            WriteBold(title);
            WriteString(Resources.Colon);
            WriteSpace();
            WriteString(assemblySymbol.Name);
            WriteString(".");
            WriteString(Resources.DllExtension);
            WriteLine();
            WriteLine();
        }

        public virtual void WriteObsoleteMessage(ISymbol symbol)
        {
            WriteBold(Resources.ObsoleteMessage);
            WriteLine();
            WriteLine();

            TypedConstant typedConstant = symbol.GetAttribute(MetadataNames.System_ObsoleteAttribute).ConstructorArguments.FirstOrDefault();

            if (typedConstant.Type?.SpecialType == SpecialType.System_String)
            {
                string message = typedConstant.Value?.ToString();

                if (!string.IsNullOrEmpty(message))
                    WriteString(message);

                WriteLine();
            }

            WriteLine();
        }

        public virtual void WriteSummary(ISymbol symbol, SymbolXmlDocumentation xmlDocumentation, int headingLevelBase = 0)
        {
            WriteSection(
                heading: Resources.SummaryTitle,
                xmlDocumentation: xmlDocumentation,
                elementName: WellKnownXmlTags.Summary,
                headingLevelBase: headingLevelBase);
        }

        //XTODO: WriteDeclaration > WriteDefinition
        public virtual void WriteDeclaration(ISymbol symbol)
        {
            SymbolDisplayAdditionalOptions additionalOptions = SymbolDisplayAdditionalOptions.FormatAttributes
                | SymbolDisplayAdditionalOptions.PreferDefaultLiteral;

            if (Options.IncludeAttributeArguments)
                additionalOptions |= SymbolDisplayAdditionalOptions.IncludeAttributeArguments;

            if (Options.FormatDeclarationBaseList)
                additionalOptions |= SymbolDisplayAdditionalOptions.FormatBaseList;

            if (Options.FormatDeclarationConstraints)
                additionalOptions |= SymbolDisplayAdditionalOptions.FormatConstraints;

            if (Options.OmitIEnumerable)
                additionalOptions |= SymbolDisplayAdditionalOptions.OmitIEnumerable;

            ImmutableArray<SymbolDisplayPart> attributesParts = SymbolDefinitionDisplay.GetAttributesParts(
                symbol,
                SymbolDisplayFormats.FullDeclaration,
                additionalOptions: additionalOptions,
                shouldDisplayAttribute: (s, a) => DocumentationModel.Filter.IsMatch(s, a),
                includeTrailingNewLine: true);

            ImmutableArray<SymbolDisplayPart> parts = SymbolDefinitionDisplay.GetDisplayParts(
                symbol,
                (symbol.GetFirstExplicitInterfaceImplementation() != null)
                    ? SymbolDisplayFormats.ExplicitImplementationFullDeclaration
                    : SymbolDisplayFormats.FullDeclaration,
                typeDeclarationOptions: SymbolDisplayTypeDeclarationOptions.IncludeAccessibility
                    | SymbolDisplayTypeDeclarationOptions.IncludeModifiers
                    | SymbolDisplayTypeDeclarationOptions.BaseList,
                additionalOptions: additionalOptions,
                shouldDisplayAttribute: (s, a) => DocumentationModel.Filter.IsMatch(s, a));

            if (symbol.IsKind(SymbolKind.NamedType))
                RemoveContainingNamespace();

            string text = attributesParts.ToDisplayString() + parts.ToDisplayString();

            WriteCodeBlock(text, LanguageNames.CSharp);

            void RemoveContainingNamespace()
            {
                int i = 0;
                int j = 0;

                while (i < parts.Length)
                {
                    if (parts[i].IsTypeName())
                        break;

                    if (parts[i].Kind == SymbolDisplayPartKind.NamespaceName)
                    {
                        j = i;

                        if (Peek().IsPunctuation("."))
                        {
                            j++;

                            while (Peek().Kind == SymbolDisplayPartKind.NamespaceName
                                && Peek(2).IsPunctuation())
                            {
                                j += 2;
                            }

                            Debug.Assert(Peek().IsTypeName(), Peek().Kind.ToString());

                            if (Peek().IsTypeName())
                            {
                                parts = parts.RemoveRange(i, j - i + 1);
                                return;
                            }
                        }

                        break;
                    }

                    i++;
                }

                SymbolDisplayPart Peek(int offset = 1)
                {
                    if (j < parts.Length - offset)
                    {
                        return parts[j + offset];
                    }

                    return default;
                }
            }
        }

        public virtual void WriteTypeParameters(ImmutableArray<ITypeParameterSymbol> typeParameters)
        {
            ImmutableArray<ITypeParameterSymbol>.Enumerator en = typeParameters.GetEnumerator();

            if (en.MoveNext())
            {
                WriteHeading(3, Resources.TypeParametersTitle);

                while (true)
                {
                    WriteBold(en.Current.Name);

                    XElement element = GetXmlDocumentation(en.Current.ContainingSymbol)?.Element(WellKnownXmlTags.TypeParam, "name", en.Current.Name);

                    if (element?.Nodes().Any() == true)
                    {
                        WriteLine();
                        WriteLine();

                        element.WriteContentTo(this);
                    }

                    if (en.MoveNext())
                    {
                        WriteLine();
                        WriteLine();
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        public virtual void WriteParameters(ImmutableArray<IParameterSymbol> parameters)
        {
            ImmutableArray<IParameterSymbol>.Enumerator en = parameters.GetEnumerator();

            if (en.MoveNext())
            {
                WriteHeading(3, Resources.ParametersTitle);

                while (true)
                {
                    WriteBold(en.Current.Name);

                    XElement element = GetXmlDocumentation(en.Current.ContainingSymbol)?.Element(WellKnownXmlTags.Param, "name", en.Current.Name);

                    if (element?.Nodes().Any() == true)
                    {
                        WriteLine();
                        WriteLine();

                        element.WriteContentTo(this);
                    }

                    if (en.MoveNext())
                    {
                        WriteLine();
                        WriteLine();
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        public virtual void WriteReturnType(ISymbol symbol, SymbolXmlDocumentation xmlDocumentation)
        {
            switch (symbol.Kind)
            {
                case SymbolKind.NamedType:
                    {
                        var namedTypeSymbol = (INamedTypeSymbol)symbol;

                        IMethodSymbol methodSymbol = namedTypeSymbol.DelegateInvokeMethod;

                        if (methodSymbol != null)
                        {
                            ITypeSymbol returnType = methodSymbol.ReturnType;

                            if (returnType.SpecialType == SpecialType.System_Void)
                                return;

                            WriteReturnType(returnType, Resources.ReturnValueTitle);

                            xmlDocumentation?.Element(WellKnownXmlTags.Returns)?.WriteContentTo(this);
                        }

                        break;
                    }
                case SymbolKind.Field:
                    {
                        var fieldSymbol = (IFieldSymbol)symbol;

                        WriteReturnType(fieldSymbol.Type, Resources.FieldValueTitle);
                        break;
                    }
                case SymbolKind.Method:
                    {
                        var methodSymbol = (IMethodSymbol)symbol;

                        switch (methodSymbol.MethodKind)
                        {
                            case MethodKind.UserDefinedOperator:
                            case MethodKind.Conversion:
                                {
                                    WriteReturnType(methodSymbol.ReturnType, Resources.ReturnsTitle);

                                    xmlDocumentation?.Element(WellKnownXmlTags.Returns)?.WriteContentTo(this);
                                    break;
                                }
                            default:
                                {
                                    ITypeSymbol returnType = methodSymbol.ReturnType;

                                    if (returnType.SpecialType == SpecialType.System_Void)
                                        return;

                                    WriteReturnType(returnType, Resources.ReturnsTitle);

                                    xmlDocumentation?.Element(WellKnownXmlTags.Returns)?.WriteContentTo(this);
                                    break;
                                }
                        }

                        break;
                    }
                case SymbolKind.Property:
                    {
                        var propertySymbol = (IPropertySymbol)symbol;

                        WriteReturnType(propertySymbol.Type, Resources.PropertyValueTitle);

                        string elementName = (propertySymbol.IsIndexer) ? WellKnownXmlTags.Returns : WellKnownXmlTags.Value;

                        xmlDocumentation?.Element(elementName)?.WriteContentTo(this);
                        break;
                    }
            }

            void WriteReturnType(ITypeSymbol typeSymbol, string heading)
            {
                WriteHeading(3, heading);
                WriteTypeLink(typeSymbol, includeContainingNamespace: Options.IncludeContainingNamespace(OmitContainingNamespaceParts.ReturnType));
                WriteLine();
                WriteLine();
            }
        }

        public virtual void WriteInheritance(INamedTypeSymbol typeSymbol)
        {
            switch (typeSymbol.TypeKind)
            {
                case TypeKind.Interface:
                    {
                        return;
                    }
                case TypeKind.Class:
                    {
                        if (typeSymbol.IsStatic)
                            return;

                        break;
                    }
            }

            if (typeSymbol.BaseType == null)
                return;

            WriteHeading(3, Resources.InheritanceTitle);

            if (Options.InheritanceStyle == InheritanceStyle.Horizontal)
            {
                using (IEnumerator<ITypeSymbol> en = typeSymbol.BaseTypes().Reverse().GetEnumerator())
                {
                    if (en.MoveNext())
                    {
                        WriteLink(en.Current.OriginalDefinition, SymbolDisplayFormats.TypeNameAndContainingTypesAndTypeParameters);

                        while (en.MoveNext())
                        {
                            WriteSeparator();
                            WriteTypeLink(en.Current.OriginalDefinition, includeContainingNamespace: false);
                        }
                    }
                }

                WriteSeparator();
                WriteTypeSymbol(typeSymbol, includeContainingNamespace: false);
                WriteLine();
            }
            else if (Options.InheritanceStyle == InheritanceStyle.Vertical)
            {
                int depth = 0;

                foreach (INamedTypeSymbol baseType in typeSymbol.BaseTypes().Reverse())
                {
                    WriteIndentation(depth);
                    WriteTypeLink(baseType.OriginalDefinition, includeContainingNamespace: Options.IncludeContainingNamespace(OmitContainingNamespaceParts.BaseType));
                    WriteLineBreak();

                    depth++;
                }

                WriteIndentation(depth);

                WriteTypeSymbol(typeSymbol, includeContainingNamespace: false);
                WriteLine();
            }
            else
            {
                throw new InvalidOperationException();
            }

            void WriteSeparator()
            {
                WriteSpace();
                WriteCharEntity(Resources.InheritanceChar);
                WriteSpace();
            }

            void WriteIndentation(int count)
            {
                for (int i = 0; i < count; i++)
                    WriteEntityRef("emsp");
            }
        }

        public virtual void WriteAttributes(ISymbol symbol, int headingLevelBase = 0)
        {
            ImmutableArray<AttributeInfo> attributes;

            if (symbol is INamedTypeSymbol typeSymbol
                && Options.IncludeInheritedAttributes)
            {
                attributes = typeSymbol.GetAttributesIncludingInherited((s, a) => DocumentationModel.Filter.IsMatch(s, a));
            }
            else
            {
                attributes = symbol
                    .GetAttributes()
                    .Where(f => DocumentationModel.Filter.IsMatch(symbol, f))
                    .Select(f => new AttributeInfo(symbol, f))
                    .ToImmutableArray();
            }

            using (IEnumerator<AttributeInfo> en = attributes
                .Sort(f => f.AttributeClass, systemNamespaceFirst: Options.PlaceSystemNamespaceFirst, includeContainingNamespace: Options.IncludeContainingNamespace(OmitContainingNamespaceParts.Attribute))
                .GetEnumerator())
            {
                if (en.MoveNext())
                {
                    WriteHeading(3 + headingLevelBase, Resources.AttributesTitle);

                    do
                    {
                        WriteStartBulletItem();
                        WriteTypeLink(en.Current.AttributeClass, includeContainingNamespace: Options.IncludeContainingNamespace(OmitContainingNamespaceParts.Attribute));

                        if (symbol != en.Current.Target)
                        {
                            WriteInheritedFrom(en.Current.Target.OriginalDefinition, SymbolDisplayFormats.TypeNameAndContainingTypesAndTypeParameters);
                        }

                        WriteEndBulletItem();
                    }
                    while (en.MoveNext());

                    WriteLine();
                }
            }
        }

        public virtual void WriteDerivedTypes(IEnumerable<INamedTypeSymbol> derivedTypes)
        {
            WriteList(
                derivedTypes,
                heading: Resources.DerivedTitle,
                headingLevel: 3,
                format: SymbolDisplayFormats.TypeNameAndContainingTypesAndTypeParameters,
                maxItems: Options.MaxDerivedTypes,
                allItemsHeading: Resources.DerivedAllTitle,
                allItemsLinkTitle: Resources.SeeAllDerivedTypes,
                includeContainingNamespace: Options.IncludeContainingNamespace(OmitContainingNamespaceParts.DerivedType));
        }

        public virtual void WriteImplementedInterfaces(IEnumerable<INamedTypeSymbol> interfaceTypes)
        {
            WriteList(
                interfaceTypes,
                heading: Resources.ImplementsTitle,
                headingLevel: 3,
                format: SymbolDisplayFormats.TypeNameAndContainingTypesAndTypeParameters,
                addLinkForTypeParameters: true,
                includeContainingNamespace: Options.IncludeContainingNamespace(OmitContainingNamespaceParts.ImplementedInterface));
        }

        public virtual void WriteImplementedInterfaceMembers(IEnumerable<ISymbol> interfaceMembers)
        {
            SymbolDisplayFormat format = SymbolDisplayFormats.TypeNameAndContainingTypesAndTypeParameters;
            bool includeContainingNamespace = Options.IncludeContainingNamespace(OmitContainingNamespaceParts.ImplementedMember);
            const SymbolDisplayAdditionalMemberOptions additionalOptions = SymbolDisplayAdditionalMemberOptions.UseItemPropertyName;

            using (IEnumerator<ISymbol> en = interfaceMembers
                .Sort(format, systemNamespaceFirst: Options.PlaceSystemNamespaceFirst, includeContainingNamespace: includeContainingNamespace, additionalOptions: additionalOptions)
                .GetEnumerator())
            {
                if (en.MoveNext())
                {
                    WriteHeading(3, Resources.ImplementsTitle);

                    WriteStartBulletList();

                    do
                    {
                        WriteStartBulletItem();

                        WriteLink(en.Current, format, additionalOptions, includeContainingNamespace: includeContainingNamespace);
                        WriteEndBulletItem();
                    }
                    while (en.MoveNext());

                    WriteEndBulletList();
                }
            }
        }

        public virtual void WriteExceptions(ISymbol symbol, SymbolXmlDocumentation xmlDocumentation, int headingLevelBase = 0)
        {
            using (IEnumerator<(XElement element, INamedTypeSymbol exceptionSymbol)> en = GetExceptions()
                .Sort(f => f.exceptionSymbol, systemNamespaceFirst: Options.PlaceSystemNamespaceFirst, includeContainingNamespace: Options.IncludeContainingNamespace(OmitContainingNamespaceParts.Exception))
                .GetEnumerator())
            {
                if (en.MoveNext())
                {
                    WriteHeading(3 + headingLevelBase, Resources.ExceptionsTitle);

                    do
                    {
                        XElement element = en.Current.element;
                        INamedTypeSymbol exceptionSymbol = en.Current.exceptionSymbol;

                        WriteTypeLink(exceptionSymbol, includeContainingNamespace: Options.IncludeContainingNamespace(OmitContainingNamespaceParts.Exception));

                        WriteLine();
                        WriteLine();
                        element.WriteContentTo(this);
                        WriteLine();
                        WriteLine();
                    }
                    while (en.MoveNext());
                }
            }

            IEnumerable<(XElement element, INamedTypeSymbol exceptionSymbol)> GetExceptions()
            {
                foreach (XElement element in xmlDocumentation.Elements(WellKnownXmlTags.Exception))
                {
                    string commentId = element.Attribute("cref")?.Value;

                    if (commentId != null
                        && DocumentationModel.GetFirstSymbolForReferenceId(commentId) is INamedTypeSymbol exceptionSymbol)
                    {
                        yield return (element, exceptionSymbol);
                    }
                }
            }
        }

        public virtual void WriteExamples(ISymbol symbol, SymbolXmlDocumentation xmlDocumentation, int headingLevelBase = 0)
        {
            WriteSection(heading: Resources.ExamplesTitle, xmlDocumentation: xmlDocumentation, elementName: WellKnownXmlTags.Example, headingLevelBase: headingLevelBase);
        }

        public virtual void WriteRemarks(ISymbol symbol, SymbolXmlDocumentation xmlDocumentation, int headingLevelBase = 0)
        {
            WriteSection(heading: Resources.RemarksTitle, xmlDocumentation: xmlDocumentation, elementName: WellKnownXmlTags.Remarks, headingLevelBase: headingLevelBase);
        }

        public virtual void WriteEnumFields(IEnumerable<IFieldSymbol> fields, INamedTypeSymbol containingType)
        {
            using (IEnumerator<IFieldSymbol> en = fields.GetEnumerator())
            {
                if (en.MoveNext())
                {
                    bool hasCombinedValue = false;

                    EnumSymbolInfo enumInfo = default;

                    if (containingType.HasAttribute(MetadataNames.System_FlagsAttribute))
                    {
                        enumInfo = EnumSymbolInfo.Create(containingType);

                        foreach (IFieldSymbol field in fields)
                        {
                            if (EnumUtility.GetMinimalConstituentFields(field, enumInfo).Any())
                            {
                                hasCombinedValue = true;
                                break;
                            }
                        }
                    }

                    WriteHeading(2, Resources.FieldsTitle);

                    WriteStartTable((hasCombinedValue) ? 4 : 3);
                    WriteStartTableRow();
                    WriteTableCell(Resources.NameTitle);
                    WriteTableCell(Resources.ValueTitle);

                    if (hasCombinedValue)
                        WriteTableCell(Resources.CombinationOfTitle);

                    WriteTableCell(Resources.SummaryTitle);
                    WriteEndTableRow();
                    WriteTableHeaderSeparator();

                    do
                    {
                        IFieldSymbol fieldSymbol = en.Current;

                        WriteStartTableRow();
                        WriteTableCell(fieldSymbol.ToDisplayString(SymbolDisplayFormats.SimpleDeclaration));
                        WriteTableCell(fieldSymbol.ConstantValue.ToString());

                        if (hasCombinedValue)
                        {
                            WriteStartTableCell();

                            ImmutableArray<EnumFieldSymbolInfo> constituentFields = EnumUtility.GetMinimalConstituentFields(en.Current, enumInfo);

                            if (constituentFields.Any())
                            {
                                WriteString(constituentFields[0].Name);

                                for (int i = 1; i < constituentFields.Length; i++)
                                {
                                    WriteString(" | ");
                                    WriteString(constituentFields[i].Name);
                                }
                            }

                            WriteEndTableCell();
                        }

                        SymbolXmlDocumentation xmlDocumentation = DocumentationModel.GetXmlDocumentation(fieldSymbol, Options.PreferredCultureName);

                        if (xmlDocumentation != null)
                        {
                            WriteStartTableCell();
                            xmlDocumentation?.Element(WellKnownXmlTags.Summary)?.WriteContentTo(this, inlineOnly: true);
                            WriteEndTableCell();
                        }

                        WriteEndTableRow();
                    }
                    while (en.MoveNext());

                    WriteEndTable();
                }
            }
        }

        public virtual void WriteConstructors(IEnumerable<IMethodSymbol> constructors)
        {
            WriteTable(constructors, Resources.ConstructorsTitle, 2, Resources.ConstructorTitle, Resources.SummaryTitle, SymbolDisplayFormats.SimpleDeclaration);
        }

        public virtual void WriteFields(IEnumerable<IFieldSymbol> fields, INamedTypeSymbol containingType)
        {
            if (containingType.TypeKind == TypeKind.Enum)
            {
                WriteEnumFields(fields, containingType);
            }
            else
            {
                WriteTable(fields, Resources.FieldsTitle, 2, Resources.FieldTitle, Resources.SummaryTitle, SymbolDisplayFormats.SimpleDeclaration, containingType: containingType);
            }
        }

        public virtual void WriteIndexers(IEnumerable<IPropertySymbol> indexers, INamedTypeSymbol containingType)
        {
            WriteTable(indexers, Resources.IndexersTitle, 2, Resources.IndexerTitle, Resources.SummaryTitle, SymbolDisplayFormats.SimpleDeclaration, SymbolDisplayAdditionalMemberOptions.UseItemPropertyName, containingType: containingType);
        }

        public virtual void WriteProperties(IEnumerable<IPropertySymbol> properties, INamedTypeSymbol containingType)
        {
            WriteTable(properties, Resources.PropertiesTitle, 2, Resources.PropertyTitle, Resources.SummaryTitle, SymbolDisplayFormats.SimpleDeclaration, SymbolDisplayAdditionalMemberOptions.UseItemPropertyName, containingType: containingType);
        }

        public virtual void WriteMethods(IEnumerable<IMethodSymbol> methods, INamedTypeSymbol containingType)
        {
            WriteTable(methods, Resources.MethodsTitle, 2, Resources.MethodTitle, Resources.SummaryTitle, SymbolDisplayFormats.SimpleDeclaration, containingType: containingType);
        }

        public virtual void WriteOperators(IEnumerable<IMethodSymbol> operators, INamedTypeSymbol containingType)
        {
            WriteTable(operators, Resources.OperatorsTitle, 2, Resources.OperatorTitle, Resources.SummaryTitle, SymbolDisplayFormats.SimpleDeclaration, SymbolDisplayAdditionalMemberOptions.UseOperatorName, containingType: containingType);
        }

        public virtual void WriteEvents(IEnumerable<IEventSymbol> events, INamedTypeSymbol containingType)
        {
            WriteTable(events, Resources.EventsTitle, 2, Resources.EventTitle, Resources.SummaryTitle, SymbolDisplayFormats.SimpleDeclaration, containingType: containingType);
        }

        public virtual void WriteExplicitInterfaceImplementations(IEnumerable<ISymbol> explicitInterfaceImplementations)
        {
            WriteTable(explicitInterfaceImplementations, Resources.ExplicitInterfaceImplementationsTitle, 2, Resources.MemberTitle, Resources.SummaryTitle, SymbolDisplayFormats.SimpleDeclaration, SymbolDisplayAdditionalMemberOptions.UseItemPropertyName, canIndicateInterfaceImplementation: false);
        }

        public virtual void WriteExtensionMethods(IEnumerable<IMethodSymbol> extensionMethods)
        {
            WriteTable(
                extensionMethods,
                Resources.ExtensionMethodsTitle,
                2,
                Resources.MethodTitle,
                Resources.SummaryTitle,
                SymbolDisplayFormats.SimpleDeclaration);
        }

        internal virtual void WriteTypes(
            IEnumerable<INamedTypeSymbol> types,
            TypeKind typeKind,
            INamedTypeSymbol containingType)
        {
            WriteTable(
                types,
                Resources.GetPluralName(typeKind),
                headingLevel: 2,
                Resources.GetName(typeKind),
                Resources.SummaryTitle,
                SymbolDisplayFormats.TypeNameAndTypeParameters,
                containingType: containingType);
        }

        public virtual void WriteSeeAlso(ISymbol symbol, SymbolXmlDocumentation xmlDocumentation, int headingLevelBase = 0)
        {
            using (IEnumerator<ISymbol> en = GetSymbols().GetEnumerator())
            {
                if (en.MoveNext())
                {
                    WriteHeading(2 + headingLevelBase, Resources.SeeAlsoTitle);

                    WriteStartBulletList();

                    do
                    {
                        WriteBulletItemLink(
                            en.Current,
                            SymbolDisplayFormats.TypeNameAndContainingTypesAndTypeParameters,
                            SymbolDisplayAdditionalMemberOptions.UseItemPropertyName | SymbolDisplayAdditionalMemberOptions.UseOperatorName,
                            includeContainingNamespace: Options.IncludeContainingNamespace(OmitContainingNamespaceParts.SeeAlso));
                    }
                    while (en.MoveNext());

                    WriteEndBulletList();
                }
            }

            IEnumerable<ISymbol> GetSymbols()
            {
                foreach (XElement element in xmlDocumentation.Elements(WellKnownXmlTags.SeeAlso))
                {
                    string commentId = element.Attribute("cref")?.Value;

                    if (commentId != null)
                    {
                        ISymbol s = DocumentationModel.GetFirstSymbolForReferenceId(commentId);

                        if (s != null)
                            yield return s;
                    }
                }
            }
        }

        internal void WriteSection(
            string heading,
            SymbolXmlDocumentation xmlDocumentation,
            string elementName,
            int headingLevelBase = 0)
        {
            XElement element = xmlDocumentation.Element(elementName);

            if (element == null)
                return;

            if (heading != null)
            {
                WriteHeading(2 + headingLevelBase, heading);
            }
            else
            {
                WriteLine();
            }

            element.WriteContentTo(this);
        }

        internal void WriteClassHierarchy(
            INamedTypeSymbol baseType,
            IEnumerable<INamedTypeSymbol> types,
            bool includeContainingNamespace = true,
            bool addBaseType = true,
            int maxItems = -1,
            string allItemsHeading = null,
            string allItemsLinkTitle = null,
            int addSeparatorAtIndex = -1)
        {
            if (maxItems == 0)
                return;

            var nodes = new HashSet<INamedTypeSymbol>(types) { baseType };

            foreach (INamedTypeSymbol type in types)
            {
                INamedTypeSymbol t = type.BaseType;

                while (t != null)
                {
                    nodes.Add(t.OriginalDefinition);
                    t = t.BaseType;
                }
            }

            int level = (addBaseType) ? 0 : -1;
            int count = 0;
            bool isMaxReached = false;

            WriteStartBulletList();
            WriteClassHierarchy();
            WriteEndBulletList();

            void WriteClassHierarchy()
            {
                if (level >= 0)
                {
                    WriteStartBulletItem();

                    for (int i = 0; i < level; i++)
                    {
                        if (i > 0)
                        {
                            WriteSpace();
                            WriteString("|");
                            WriteSpace();
                        }

                        WriteEntityRef("emsp");
                    }

                    if (level >= 1)
                        WriteSpace();

                    if (DocumentationModel.IsExternal(baseType))
                    {
                        WriteSymbol(baseType, SymbolDisplayFormats.TypeNameAndContainingTypesAndNamespacesAndTypeParameters);
                    }
                    else
                    {
                        WriteTypeLink(baseType);
                    }

                    WriteObsolete(baseType, before: false);

                    WriteEndBulletItem();

                    count++;

                    if (addSeparatorAtIndex == count)
                    {
                        WriteStartBulletItem();
                        WriteEntityRef("mdash");
                        WriteEntityRef("mdash");
                        WriteEntityRef("mdash");
                        WriteEntityRef("mdash");
                        WriteEntityRef("mdash");
                        WriteEndBulletItem();
                    }
                }

                nodes.Remove(baseType);

                level++;

                using (List<INamedTypeSymbol>.Enumerator en = nodes
                    .Where(f => f.BaseType?.OriginalDefinition == baseType.OriginalDefinition || f.Interfaces.Any(i => i.OriginalDefinition == baseType.OriginalDefinition))
                    .Sort(systemNamespaceFirst: Options.PlaceSystemNamespaceFirst, includeContainingNamespace: includeContainingNamespace)
                    .ToList()
                    .GetEnumerator())
                {
                    if (en.MoveNext())
                    {
                        if (!isMaxReached
                            && count == maxItems)
                        {
                            WriteEllipsis();
                            isMaxReached = true;
                            return;
                        }

                        do
                        {
                            baseType = en.Current;

                            WriteClassHierarchy();

                            if (isMaxReached)
                                return;

                            if (count == maxItems)
                            {
                                if (en.MoveNext())
                                {
                                    WriteEllipsis();
                                    isMaxReached = true;
                                    return;
                                }

                                break;
                            }
                        }
                        while (en.MoveNext());
                    }
                }

                level--;
            }

            void WriteEllipsis()
            {
                if (!string.IsNullOrEmpty(allItemsHeading))
                {
                    WriteStartBulletItem();
                    WriteLink(Resources.Ellipsis, UrlProvider.GetFragment(Resources.DerivedAllTitle), title: allItemsLinkTitle);
                    WriteEndBulletItem();
                }
                else
                {
                    WriteBulletItem(Resources.Ellipsis);
                }
            }
        }

        internal void WriteTable(
            IEnumerable<ISymbol> symbols,
            string heading,
            int headingLevel,
            string header1,
            string header2,
            SymbolDisplayFormat format,
            SymbolDisplayAdditionalMemberOptions additionalOptions = SymbolDisplayAdditionalMemberOptions.None,
            bool addLink = true,
            bool canIndicateInterfaceImplementation = true,
            INamedTypeSymbol containingType = null)
        {
            using (IEnumerator<ISymbol> en = symbols
                .OrderBy(f => f.ToDisplayString(format, additionalOptions))
                .GetEnumerator())
            {
                if (en.MoveNext())
                {
                    if (heading != null)
                        WriteHeading(headingLevel, heading);

                    WriteStartTable(2);
                    WriteStartTableRow();
                    WriteTableCell(header1);
                    WriteTableCell(header2);
                    WriteEndTableRow();
                    WriteTableHeaderSeparator();

                    do
                    {
                        ISymbol symbol = en.Current;

                        Debug.Assert(!symbol.IsKind(SymbolKind.Parameter, SymbolKind.TypeParameter), symbol.Kind.ToString());

                        WriteStartTableRow();
                        WriteStartTableCell();

                        if (symbol.IsKind(SymbolKind.Parameter, SymbolKind.TypeParameter))
                        {
                            WriteString(symbol.Name);
                        }
                        else if (addLink)
                        {
                            WriteLink(symbol, format, additionalOptions);
                        }
                        else
                        {
                            WriteString(symbol.ToDisplayString(format, additionalOptions));
                        }

                        WriteEndTableCell();
                        WriteStartTableCell();

                        WriteObsolete(symbol);

                        bool isInherited = containingType != null
                            && symbol.ContainingType != containingType;

                        if (symbol.Kind == SymbolKind.Parameter)
                        {
                            GetXmlDocumentation(symbol.ContainingSymbol)?.Element(WellKnownXmlTags.Param, "name", symbol.Name)?.WriteContentTo(this);
                        }
                        else if (symbol.Kind == SymbolKind.TypeParameter)
                        {
                            GetXmlDocumentation(symbol.ContainingSymbol)?.Element(WellKnownXmlTags.TypeParam, "name", symbol.Name)?.WriteContentTo(this);
                        }
                        else
                        {
                            ISymbol symbol2 = (isInherited) ? symbol.OriginalDefinition : symbol;

                            GetXmlDocumentation(symbol2)?.Element(WellKnownXmlTags.Summary)?.WriteContentTo(this, inlineOnly: true);
                        }

                        if (isInherited)
                        {
                            if (Options.IncludeMemberInheritedFrom)
                                WriteInheritedFrom(symbol.ContainingType.OriginalDefinition, SymbolDisplayFormats.TypeNameAndContainingTypesAndTypeParameters, additionalOptions);
                        }
                        else
                        {
                            if (Options.IncludeMemberOverrides)
                                WriteOverrides(symbol);

                            if (canIndicateInterfaceImplementation
                                && Options.IncludeMemberImplements)
                            {
                                WriteImplements(symbol);
                            }

                            if (Options.IncludeMemberConstantValue
                                && symbol.Kind == SymbolKind.Field)
                            {
                                var fieldSymbol = (IFieldSymbol)symbol;

                                if (fieldSymbol.HasConstantValue)
                                    WriteConstantValue(fieldSymbol);
                            }
                        }

                        WriteEndTableCell();
                        WriteEndTableRow();
                    }
                    while (en.MoveNext());

                    WriteEndTable();
                }
            }

            void WriteOverrides(ISymbol symbol)
            {
                if (symbol.IsOverride)
                {
                    ISymbol overriddenSymbol = symbol.OverriddenSymbol();

                    if (overriddenSymbol != null)
                    {
                        WriteSpace();
                        WriteString(Resources.OpenParenthesis);
                        WriteString(Resources.OverridesTitle);
                        WriteSpace();
                        WriteLink(overriddenSymbol, SymbolDisplayFormats.TypeNameAndTypeParameters, additionalOptions);
                        WriteString(Resources.CloseParenthesis);
                    }
                }
            }

            void WriteImplements(ISymbol symbol)
            {
                using (IEnumerator<ISymbol> en = symbol.FindImplementedInterfaceMembers().GetEnumerator())
                {
                    if (en.MoveNext())
                    {
                        WriteSpace();
                        WriteString(Resources.OpenParenthesis);
                        WriteString(Resources.ImplementsTitle);

                        while (true)
                        {
                            WriteSpace();
                            WriteLink(en.Current, SymbolDisplayFormats.TypeNameAndTypeParameters, additionalOptions);

                            if (en.MoveNext())
                            {
                                WriteString(Resources.Comma);
                            }
                            else
                            {
                                break;
                            }
                        }

                        WriteString(Resources.CloseParenthesis);
                    }
                }
            }

            void WriteConstantValue(IFieldSymbol fieldSymbol)
            {
                WriteSpace();
                WriteString(Resources.OpenParenthesis);
                WriteString(Resources.ValueTitle);
                WriteSpace();
                WriteString(Resources.EqualsSign);
                WriteSpace();

                if (fieldSymbol.Type.TypeKind == TypeKind.Enum)
                {
                    OneOrMany<EnumFieldSymbolInfo>.Enumerator en = EnumUtility.GetConstituentFields(fieldSymbol.ConstantValue, fieldSymbol.ContainingType).GetEnumerator();

                    if (en.MoveNext())
                    {
                        while (true)
                        {
                            WriteSymbol(en.Current.Symbol, SymbolDisplayFormats.TypeName);

                            if (en.MoveNext())
                            {
                                WriteString(" | ");
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        WriteString(fieldSymbol.ConstantValue.ToString());
                    }
                }
                else
                {
                    WriteString(SymbolDisplay.FormatPrimitive(fieldSymbol.ConstantValue, quoteStrings: true, useHexadecimalNumbers: false));
                }

                WriteString(Resources.CloseParenthesis);
            }
        }

        private void WriteInheritedFrom(ISymbol symbol, SymbolDisplayFormat format, SymbolDisplayAdditionalMemberOptions additionalOptions = SymbolDisplayAdditionalMemberOptions.None)
        {
            WriteSpace();
            WriteString(Resources.OpenParenthesis);
            WriteString(Resources.InheritedFrom);
            WriteSpace();
            WriteLink(symbol, format, additionalOptions);
            WriteString(Resources.CloseParenthesis);
        }

        internal void WriteList(
            IEnumerable<ISymbol> symbols,
            string heading,
            int headingLevel,
            SymbolDisplayFormat format,
            SymbolDisplayAdditionalMemberOptions additionalOptions = SymbolDisplayAdditionalMemberOptions.None,
            int maxItems = -1,
            string allItemsHeading = null,
            string allItemsLinkTitle = null,
            bool addLink = true,
            bool addLinkForTypeParameters = false,
            bool includeContainingNamespace = false,
            bool canCreateExternalUrl = true)
        {
            Debug.Assert(!includeContainingNamespace || format.TypeQualificationStyle != SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces, "");

            if (maxItems == 0)
                return;

            using (IEnumerator<ISymbol> en = symbols
                    .Sort(format, systemNamespaceFirst: Options.PlaceSystemNamespaceFirst, includeContainingNamespace: includeContainingNamespace, additionalOptions: additionalOptions)
                    .GetEnumerator())
            {
                if (en.MoveNext())
                {
                    if (heading != null)
                        WriteHeading(headingLevel, heading);

                    WriteStartBulletList();

                    int count = 0;

                    do
                    {
                        if (addLink)
                        {
                            WriteStartBulletItem();

                            WriteLink(en.Current, format, includeContainingNamespace: includeContainingNamespace, addLinkForTypeParameters: addLinkForTypeParameters, canCreateExternalUrl: canCreateExternalUrl);
                            WriteObsolete(en.Current, before: false);
                            WriteEndBulletItem();
                        }
                        else
                        {
                            WriteBulletItem(en.Current.ToDisplayString(format));
                        }

                        count++;

                        if (count == maxItems)
                        {
                            if (en.MoveNext())
                            {
                                if (!string.IsNullOrEmpty(allItemsHeading))
                                {
                                    WriteStartBulletItem();
                                    WriteLink(Resources.Ellipsis, UrlProvider.GetFragment(Resources.DerivedAllTitle), title: allItemsLinkTitle);
                                    WriteEndBulletItem();
                                }
                                else
                                {
                                    WriteBulletItem(Resources.Ellipsis);
                                }
                            }

                            break;
                        }
                    }
                    while (en.MoveNext());

                    WriteEndBulletList();
                }
            }
        }

        internal void WriteHeading(
            int level,
            ISymbol symbol,
            SymbolDisplayFormat format,
            SymbolDisplayAdditionalMemberOptions additionalOptions = SymbolDisplayAdditionalMemberOptions.None,
            bool addLink = true,
            string linkDestination = null)
        {
            if (!string.IsNullOrEmpty(linkDestination))
            {
                WriteLinkDestination(linkDestination);
                WriteLine();
            }

            WriteStartHeading(level);

            if (addLink)
            {
                WriteLink(symbol, format, additionalOptions);
            }
            else
            {
                WriteSymbol(symbol, format, additionalOptions);
            }

            if (symbol.Kind != SymbolKind.Namespace
                || !((INamespaceSymbol)symbol).IsGlobalNamespace)
            {
                WriteSpace();
                WriteString(Resources.GetName(symbol));
            }

            WriteEndHeading();
        }

        internal void WriteBulletItemLink(
            ISymbol symbol,
            SymbolDisplayFormat format,
            SymbolDisplayAdditionalMemberOptions additionalOptions = SymbolDisplayAdditionalMemberOptions.None,
            bool includeContainingNamespace = false,
            bool canCreateExternalUrl = true)
        {
            WriteStartBulletItem();
            WriteLink(symbol, format, additionalOptions: additionalOptions, includeContainingNamespace: includeContainingNamespace, canCreateExternalUrl: canCreateExternalUrl);
            WriteEndBulletItem();
        }

        internal void WriteLink(
            ISymbol symbol,
            SymbolDisplayFormat format,
            SymbolDisplayAdditionalMemberOptions additionalOptions = SymbolDisplayAdditionalMemberOptions.None,
            bool includeContainingNamespace = false,
            bool addLinkForTypeParameters = false,
            bool canCreateExternalUrl = true)
        {
            if (addLinkForTypeParameters
                && symbol is INamedTypeSymbol namedType)
            {
                bool includeContainingTypes = format.TypeQualificationStyle != SymbolDisplayTypeQualificationStyle.NameOnly;

                WriteTypeLink(namedType, includeContainingNamespace: includeContainingNamespace, includeContainingTypes: includeContainingTypes, canCreateExternalUrl: canCreateExternalUrl);
            }
            else
            {
                string url = GetUrl(symbol, canCreateExternalUrl);

                if (!string.IsNullOrEmpty(url))
                {
                    Debug.Assert(symbol.Kind == SymbolKind.Namespace || format.TypeQualificationStyle != SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

                    if (includeContainingNamespace
                        && !symbol.ContainingNamespace.IsGlobalNamespace
                        && format.TypeQualificationStyle != SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces)
                    {
                        WriteNamespaceSymbol(symbol.ContainingNamespace);
                        WriteString(".");
                    }

                    WriteLink(symbol.ToDisplayString(format, additionalOptions), url);
                }
                else
                {
                    WriteString(symbol.ToDisplayString(format, additionalOptions));
                }
            }
        }

        internal void WriteTypeLink(
            ITypeSymbol typeSymbol,
            bool includeContainingNamespace = true,
            bool includeContainingTypes = true,
            bool canCreateExternalUrl = true)
        {
            if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
            {
                WriteTypeLink(namedTypeSymbol, includeContainingNamespace: includeContainingNamespace, includeContainingTypes: includeContainingTypes, canCreateExternalUrl: canCreateExternalUrl);
            }
            else
            {
                string url = GetUrl(typeSymbol, canCreateExternalUrl);

                SymbolDisplayFormat format = (includeContainingTypes)
                    ? SymbolDisplayFormats.TypeNameAndContainingTypesAndTypeParameters
                    : SymbolDisplayFormats.TypeNameAndTypeParameters;

                WriteLinkOrText(typeSymbol.ToDisplayString(format), url);
            }
        }

        private void WriteTypeLink(
            INamedTypeSymbol typeSymbol,
            bool includeContainingNamespace = true,
            bool includeContainingTypes = true,
            bool canCreateExternalUrl = true)
        {
            if (includeContainingNamespace
                && !typeSymbol.ContainingNamespace.IsGlobalNamespace)
            {
                WriteNamespaceSymbol(typeSymbol.ContainingNamespace);
                WriteString(".");
            }

            if (typeSymbol.IsNullableType())
            {
                ITypeSymbol typeArgument = typeSymbol.TypeArguments[0];

                WriteTypeLink(typeArgument, includeContainingNamespace: false, includeContainingTypes: includeContainingTypes, canCreateExternalUrl: canCreateExternalUrl);
                WriteString("?");
            }
            else
            {
                ImmutableArray<ITypeSymbol> typeArguments = typeSymbol.TypeArguments;

                if (typeArguments.Any(f => f.Kind != SymbolKind.TypeParameter))
                {
                    SymbolDisplayFormat format = (includeContainingTypes)
                        ? SymbolDisplayFormats.TypeNameAndContainingTypes
                        : SymbolDisplayFormats.TypeName;

                    string url = GetUrl(typeSymbol, canCreateExternalUrl);

                    WriteLinkOrText(typeSymbol.ToDisplayString(format), url);

                    ImmutableArray<ITypeSymbol>.Enumerator en = typeArguments.GetEnumerator();

                    if (en.MoveNext())
                    {
                        WriteString("<");

                        while (true)
                        {
                            if (en.Current.Kind == SymbolKind.NamedType)
                            {
                                WriteTypeLink((INamedTypeSymbol)en.Current, includeContainingNamespace: false, includeContainingTypes: includeContainingTypes, canCreateExternalUrl: canCreateExternalUrl);
                            }
                            else
                            {
                                Debug.Assert(en.Current.Kind == SymbolKind.TypeParameter, en.Current.Kind.ToString());

                                WriteString(en.Current.Name);
                            }

                            if (en.MoveNext())
                            {
                                WriteString(", ");
                            }
                            else
                            {
                                break;
                            }
                        }

                        WriteString(">");
                    }
                }
                else
                {
                    SymbolDisplayFormat format = (includeContainingTypes)
                        ? SymbolDisplayFormats.TypeNameAndContainingTypesAndTypeParameters
                        : SymbolDisplayFormats.TypeNameAndTypeParameters;

                    string url = GetUrl(typeSymbol, canCreateExternalUrl);

                    WriteLinkOrText(typeSymbol.ToDisplayString(format), url);
                }
            }
        }

        private void WriteObsolete(ISymbol symbol, bool before = true)
        {
            if (Options.MarkObsolete
                && symbol.HasAttribute(MetadataNames.System_ObsoleteAttribute))
            {
                if (!before)
                    WriteSpace();

                WriteString("[");
                WriteString(Resources.DeprecatedTitle);
                WriteString("]");

                if (before)
                    WriteSpace();
            }
        }

        private string GetUrl(
            ISymbol symbol,
            bool canCreateExternalUrl = true)
        {
            ImmutableArray<string> folders = UrlProvider.GetFolders(symbol);

            if (folders.IsDefault)
                return null;

            switch (symbol.Kind)
            {
                case SymbolKind.NamedType:
                    {
                        if (!CanCreateTypeLocalUrl)
                            return null;

                        break;
                    }
                case SymbolKind.Event:
                case SymbolKind.Field:
                case SymbolKind.Method:
                case SymbolKind.Property:
                    {
                        if (!CanCreateMemberLocalUrl)
                            return null;

                        break;
                    }
                case SymbolKind.Parameter:
                case SymbolKind.TypeParameter:
                    {
                        return null;
                    }
            }

            if (DocumentationModel.IsExternal(symbol))
            {
                if (symbol.Kind == SymbolKind.Namespace
                    && ((INamespaceSymbol)symbol).IsGlobalNamespace)
                {
                    return null;
                }

                if (canCreateExternalUrl)
                    return UrlProvider.GetExternalUrl(folders).Url;
            }

            ImmutableArray<string> containingFolders = (CurrentSymbol != null)
                ? UrlProvider.GetFolders(CurrentSymbol)
                : default;

            string fragment = GetFragment();

            if (fragment == null
                && Options.ScrollToContent)
            {
                fragment = "#" + WellKnownNames.TopFragmentName;
            }

            string url = UrlProvider.GetLocalUrl(folders, containingFolders, fragment).Url;

            return Options.RootDirectoryUrl + url;

            string GetFragment()
            {
                if (symbol.Kind == SymbolKind.Method
                    || (symbol.Kind == SymbolKind.Property && ((IPropertySymbol)symbol).IsIndexer))
                {
                    TypeDocumentationModel typeModel = DocumentationModel.GetTypeModel(symbol.ContainingType);

                    IEnumerable<ISymbol> members = GetMembers(typeModel);

                    if (members != null)
                    {
                        using (IEnumerator<ISymbol> en = members.Where(f => f.Name == symbol.Name).GetEnumerator())
                        {
                            if (en.MoveNext()
                                && en.MoveNext())
                            {
                                return "#" + DocumentationUrlProvider.GetFragment(symbol);
                            }
                        }
                    }
                }

                return null;
            }

            IEnumerable<ISymbol> GetMembers(TypeDocumentationModel model)
            {
                switch (symbol.Kind)
                {
                    case SymbolKind.Method:
                        {
                            var methodSymbol = (IMethodSymbol)symbol;

                            switch (methodSymbol.MethodKind)
                            {
                                case MethodKind.Constructor:
                                    {
                                        return model.GetConstructors();
                                    }
                                case MethodKind.Ordinary:
                                    {
                                        return model.GetMethods();
                                    }
                                case MethodKind.Conversion:
                                case MethodKind.UserDefinedOperator:
                                    {
                                        return model.GetOperators();
                                    }
                                case MethodKind.ExplicitInterfaceImplementation:
                                    {
                                        ImmutableArray<IMethodSymbol> explicitInterfaceImplementations = methodSymbol.ExplicitInterfaceImplementations;

                                        if (!explicitInterfaceImplementations.IsDefaultOrEmpty)
                                            return model.GetExplicitInterfaceImplementations();

                                        break;
                                    }
                            }

                            break;
                        }
                    case SymbolKind.Property:
                        {
                            var propertySymbol = (IPropertySymbol)symbol;

                            if (propertySymbol.IsIndexer)
                            {
                                ImmutableArray<IPropertySymbol> explicitInterfaceImplementations = propertySymbol.ExplicitInterfaceImplementations;

                                if (!explicitInterfaceImplementations.IsDefaultOrEmpty)
                                {
                                    return model.GetExplicitInterfaceImplementations();
                                }
                                else
                                {
                                    return model.GetIndexers();
                                }
                            }

                            break;
                        }
                }

                return null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                    Close();

                _disposed = true;
            }
        }

        public virtual void Close()
        {
        }
    }
}
