// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Roslynator.CSharp;
using Roslynator.Text;

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

        private void WriteContainingNamespacePrefix(ISymbol symbol)
        {
            Debug.Assert(!symbol.IsKind(SymbolKind.Namespace), symbol.Kind.ToString());

            INamespaceSymbol namespaceSymbol = symbol.ContainingNamespace;

            if (namespaceSymbol?.IsGlobalNamespace == false)
            {
                if (Options.IncludeSystemNamespace
                    || !namespaceSymbol.IsSystemNamespace())
                {
                    WriteString(namespaceSymbol.ToDisplayString(TypeSymbolDisplayFormats.Name_ContainingTypes_Namespaces));
                    WriteString(".");
                }
            }
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
                    WriteString(symbol.ContainingType.ToDisplayString(TypeSymbolDisplayFormats.Name_ContainingTypes_TypeParameters));
                    WriteSpace();
                    WriteString(Resources.ConstructorsTitle);
                }
                else
                {
                    WriteString(symbol.ToDisplayString(DocumentationDisplayFormats.SimpleDeclaration));
                    WriteSpace();
                    WriteString(Resources.ConstructorTitle);
                }
            }
            else
            {
                SymbolDisplayFormat format = (isOverloaded)
                    ? DocumentationDisplayFormats.OverloadedMemberTitle
                    : DocumentationDisplayFormats.MemberTitle;

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
            WriteLink(namespaceSymbol, TypeSymbolDisplayFormats.Name_ContainingTypes_Namespaces);
            WriteLine();
            WriteLine();
        }

        public virtual void WriteContainingType(INamedTypeSymbol typeSymbol, string title)
        {
            WriteBold(title);
            WriteString(Resources.Colon);
            WriteSpace();
            WriteTypeLink(typeSymbol, includeContainingNamespace: Options.IncludeContainingNamespace(IncludeContainingNamespaceFilter.ContainingType));
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
                heading: null,
                xmlDocumentation: xmlDocumentation,
                elementName: WellKnownXmlTags.Summary,
                headingLevelBase: headingLevelBase);
        }

        public virtual void WriteDefinition(ISymbol symbol)
        {
            var additionalOptions = SymbolDisplayAdditionalOptions.FormatAttributes;

            if (Options.IncludeAttributeArguments)
                additionalOptions |= SymbolDisplayAdditionalOptions.IncludeAttributeArguments;

            if (Options.WrapDeclarationBaseTypes)
                additionalOptions |= SymbolDisplayAdditionalOptions.WrapBaseTypes;

            if (Options.WrapDeclarationConstraints)
                additionalOptions |= SymbolDisplayAdditionalOptions.WrapConstraints;

            if (Options.OmitIEnumerable)
                additionalOptions |= SymbolDisplayAdditionalOptions.OmitIEnumerable;

            ImmutableArray<SymbolDisplayPart> attributesParts = SymbolDefinitionDisplay.GetAttributesParts(
                symbol,
                DocumentationDisplayFormats.FullDeclaration,
                additionalOptions: additionalOptions,
                shouldDisplayAttribute: (s, a) => DocumentationModel.Filter.IsMatch(s, a),
                includeTrailingNewLine: true);

            ImmutableArray<SymbolDisplayPart> definitionParts = SymbolDefinitionDisplay.GetDisplayParts(
                symbol,
                (symbol.GetFirstExplicitInterfaceImplementation() != null)
                    ? DocumentationDisplayFormats.ExplicitImplementationFullDeclaration
                    : DocumentationDisplayFormats.FullDeclaration,
                typeDeclarationOptions: SymbolDisplayTypeDeclarationOptions.IncludeAccessibility
                    | SymbolDisplayTypeDeclarationOptions.IncludeModifiers
                    | SymbolDisplayTypeDeclarationOptions.BaseList,
                additionalOptions: additionalOptions,
                shouldDisplayAttribute: (s, a) => DocumentationModel.Filter.IsMatch(s, a));

            StringBuilder sb = StringBuilderCache.GetInstance(attributesParts.Length + definitionParts.Length);

            AppendParts(attributesParts, removeContainingNamespace: false);
            AppendParts(definitionParts, removeContainingNamespace: symbol.IsKind(SymbolKind.NamedType));

            string text = StringBuilderCache.GetStringAndFree(sb);

            WriteCodeBlock(text, LanguageNames.CSharp);

            void AppendParts(ImmutableArray<SymbolDisplayPart> parts, bool removeContainingNamespace)
            {
                for (int i = 0; i < parts.Length; i++)
                {
                    if (parts[i].IsGlobalNamespace()
                        && Peek(i).IsPunctuation("::"))
                    {
                        i += 2;

                        if (parts[i].Kind == SymbolDisplayPartKind.NamespaceName)
                        {
                            if (!Options.IncludeSystemNamespace
                                && parts[i].Symbol is INamespaceSymbol namespaceSymbol
                                && namespaceSymbol.IsSystemNamespace()
                                && Peek(i).IsPunctuation(".")
                                && Peek(i + 1).Kind != SymbolDisplayPartKind.NamespaceName)
                            {
                                i += 2;
                            }
                            else if (removeContainingNamespace
                                && Peek(i).IsPunctuation("."))
                            {
                                i += 2;

                                while (i + 1 < parts.Length
                                    && parts[i].Kind == SymbolDisplayPartKind.NamespaceName
                                    && parts[i + 1].IsPunctuation("."))
                                {
                                    i += 2;
                                }
                            }

                            removeContainingNamespace = false;
                        }
                    }

                    sb.Append(parts[i].ToString());
                }

                SymbolDisplayPart Peek(int index)
                {
                    if (index + 1 < parts.Length)
                        return parts[index + 1];

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
                    WriteSpace();
                    WriteEntityRef("ensp");
                    WriteSpace();
                    WriteTypeLink(en.Current.Type, includeContainingNamespace: Options.IncludeContainingNamespace(IncludeContainingNamespaceFilter.Parameter));

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
                WriteTypeLink(typeSymbol, includeContainingNamespace: Options.IncludeContainingNamespace(IncludeContainingNamespaceFilter.ReturnType));
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
                        WriteLink(en.Current.OriginalDefinition, TypeSymbolDisplayFormats.Name_ContainingTypes_TypeParameters);

                        while (en.MoveNext())
                        {
                            WriteSeparator();
                            WriteTypeLink(en.Current.OriginalDefinition);
                        }
                    }
                }

                WriteSeparator();
                WriteSymbol(typeSymbol, TypeSymbolDisplayFormats.Name_ContainingTypes_TypeParameters);
                WriteLine();
            }
            else if (Options.InheritanceStyle == InheritanceStyle.Vertical)
            {
                int depth = 0;

                foreach (INamedTypeSymbol baseType in typeSymbol.BaseTypes().Reverse())
                {
                    WriteIndentation(depth);
                    WriteTypeLink(baseType.OriginalDefinition, includeContainingNamespace: Options.IncludeContainingNamespace(IncludeContainingNamespaceFilter.BaseType));
                    WriteLineBreak();

                    depth++;
                }

                WriteIndentation(depth);
                WriteSymbol(typeSymbol, TypeSymbolDisplayFormats.Name_ContainingTypes_TypeParameters);
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
                .OrderBy(f => f.AttributeClass, SymbolComparer.Create(systemNamespaceFirst: Options.PlaceSystemNamespaceFirst, includeNamespaces: Options.IncludeContainingNamespace(IncludeContainingNamespaceFilter.Attribute)))
                .GetEnumerator())
            {
                if (en.MoveNext())
                {
                    WriteHeading(3 + headingLevelBase, Resources.AttributesTitle);

                    do
                    {
                        WriteStartBulletItem();
                        WriteTypeLink(en.Current.AttributeClass, includeContainingNamespace: Options.IncludeContainingNamespace(IncludeContainingNamespaceFilter.Attribute));

                        if (!SymbolEqualityComparer.Default.Equals(symbol, en.Current.Target))
                        {
                            WriteInheritedFrom(en.Current.Target.OriginalDefinition, TypeSymbolDisplayFormats.Name_ContainingTypes_TypeParameters);
                        }

                        WriteEndBulletItem();

                    } while (en.MoveNext());

                    WriteLine();
                }
            }
        }

        public virtual void WriteDerivedTypes(IEnumerable<INamedTypeSymbol> derivedTypes)
        {
            WriteTypeList(
                derivedTypes,
                heading: Resources.DerivedTitle,
                headingLevel: 3,
                maxItems: Options.MaxDerivedTypes,
                allItemsHeading: Resources.DerivedAllTitle,
                allItemsLinkTitle: Resources.SeeAllDerivedTypes,
                includeContainingNamespace: Options.IncludeContainingNamespace(IncludeContainingNamespaceFilter.DerivedType));
        }

        public virtual void WriteImplementedInterfaces(IEnumerable<INamedTypeSymbol> interfaceTypes)
        {
            WriteTypeList(
                interfaceTypes,
                heading: Resources.ImplementsTitle,
                headingLevel: 3,
                includeContainingNamespace: Options.IncludeContainingNamespace(IncludeContainingNamespaceFilter.ImplementedInterface),
                addLinkForTypeParameters: true);
        }

        public virtual void WriteImplementedInterfaceMembers(IEnumerable<ISymbol> interfaceMembers)
        {
            bool includeContainingNamespace = Options.IncludeContainingNamespace(IncludeContainingNamespaceFilter.ImplementedMember);
            const SymbolDisplayAdditionalMemberOptions additionalOptions = SymbolDisplayAdditionalMemberOptions.UseItemPropertyName;

            using (IEnumerator<ISymbol> en = interfaceMembers
                .OrderBy(f => f, SymbolComparer.Create(systemNamespaceFirst: Options.PlaceSystemNamespaceFirst, includeNamespaces: includeContainingNamespace, additionalOptions: additionalOptions))
                .GetEnumerator())
            {
                if (en.MoveNext())
                {
                    WriteHeading(3, Resources.ImplementsTitle);
                    WriteStartBulletList();

                    do
                    {
                        WriteStartBulletItem();
                        WriteLink(en.Current, TypeSymbolDisplayFormats.Name_ContainingTypes_TypeParameters, additionalOptions, includeContainingNamespace: includeContainingNamespace);
                        WriteEndBulletItem();

                    } while (en.MoveNext());

                    WriteEndBulletList();
                }
            }
        }

        public virtual void WriteExceptions(ISymbol symbol, SymbolXmlDocumentation xmlDocumentation, int headingLevelBase = 0)
        {
            using (IEnumerator<(XElement element, INamedTypeSymbol exceptionSymbol)> en = GetExceptions()
                .OrderBy(f => f.exceptionSymbol, SymbolComparer.Create(systemNamespaceFirst: Options.PlaceSystemNamespaceFirst, includeNamespaces: Options.IncludeContainingNamespace(IncludeContainingNamespaceFilter.Exception)))
                .GetEnumerator())
            {
                if (en.MoveNext())
                {
                    WriteHeading(3 + headingLevelBase, Resources.ExceptionsTitle);

                    do
                    {
                        XElement element = en.Current.element;
                        INamedTypeSymbol exceptionSymbol = en.Current.exceptionSymbol;

                        WriteTypeLink(exceptionSymbol, includeContainingNamespace: Options.IncludeContainingNamespace(IncludeContainingNamespaceFilter.Exception));

                        WriteLine();
                        WriteLine();
                        element.WriteContentTo(this);
                        WriteLine();
                        WriteLine();

                    } while (en.MoveNext());
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
                        WriteTableCell(fieldSymbol.ToDisplayString(DocumentationDisplayFormats.SimpleDeclaration));
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

                    } while (en.MoveNext());

                    WriteEndTable();
                }
            }
        }

        public virtual void WriteConstructors(IEnumerable<IMethodSymbol> constructors)
        {
            WriteTable(constructors, Resources.ConstructorsTitle, 2, Resources.ConstructorTitle, Resources.SummaryTitle, DocumentationDisplayFormats.SimpleDeclaration);
        }

        public virtual void WriteFields(IEnumerable<IFieldSymbol> fields, INamedTypeSymbol containingType)
        {
            if (containingType.TypeKind == TypeKind.Enum)
            {
                WriteEnumFields(fields, containingType);
            }
            else
            {
                WriteTable(fields, Resources.FieldsTitle, 2, Resources.FieldTitle, Resources.SummaryTitle, DocumentationDisplayFormats.SimpleDeclaration, containingType: containingType);
            }
        }

        public virtual void WriteIndexers(IEnumerable<IPropertySymbol> indexers, INamedTypeSymbol containingType)
        {
            WriteTable(indexers, Resources.IndexersTitle, 2, Resources.IndexerTitle, Resources.SummaryTitle, DocumentationDisplayFormats.SimpleDeclaration, SymbolDisplayAdditionalMemberOptions.UseItemPropertyName, containingType: containingType);
        }

        public virtual void WriteProperties(IEnumerable<IPropertySymbol> properties, INamedTypeSymbol containingType)
        {
            WriteTable(properties, Resources.PropertiesTitle, 2, Resources.PropertyTitle, Resources.SummaryTitle, DocumentationDisplayFormats.SimpleDeclaration, SymbolDisplayAdditionalMemberOptions.UseItemPropertyName, containingType: containingType);
        }

        public virtual void WriteMethods(IEnumerable<IMethodSymbol> methods, INamedTypeSymbol containingType)
        {
            WriteTable(methods, Resources.MethodsTitle, 2, Resources.MethodTitle, Resources.SummaryTitle, DocumentationDisplayFormats.SimpleDeclaration, containingType: containingType);
        }

        public virtual void WriteOperators(IEnumerable<IMethodSymbol> operators, INamedTypeSymbol containingType)
        {
            WriteTable(operators, Resources.OperatorsTitle, 2, Resources.OperatorTitle, Resources.SummaryTitle, DocumentationDisplayFormats.SimpleDeclaration, SymbolDisplayAdditionalMemberOptions.UseOperatorName, containingType: containingType);
        }

        public virtual void WriteEvents(IEnumerable<IEventSymbol> events, INamedTypeSymbol containingType)
        {
            WriteTable(events, Resources.EventsTitle, 2, Resources.EventTitle, Resources.SummaryTitle, DocumentationDisplayFormats.SimpleDeclaration, containingType: containingType);
        }

        public virtual void WriteExplicitInterfaceImplementations(IEnumerable<ISymbol> explicitInterfaceImplementations)
        {
            WriteTable(explicitInterfaceImplementations, Resources.ExplicitInterfaceImplementationsTitle, 2, Resources.MemberTitle, Resources.SummaryTitle, DocumentationDisplayFormats.SimpleDeclaration, SymbolDisplayAdditionalMemberOptions.UseItemPropertyName, canIncludeInterfaceImplementation: false);
        }

        public virtual void WriteExtensionMethods(IEnumerable<IMethodSymbol> extensionMethods)
        {
            WriteTable(
                extensionMethods,
                Resources.ExtensionMethodsTitle,
                2,
                Resources.MethodTitle,
                Resources.SummaryTitle,
                DocumentationDisplayFormats.SimpleDeclaration);
        }

        internal virtual void WriteNestedTypes(
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
                TypeSymbolDisplayFormats.Name_TypeParameters,
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
                        WriteStartBulletItem();
                        WriteLink(en.Current, TypeSymbolDisplayFormats.Name_ContainingTypes_TypeParameters, additionalOptions: SymbolDisplayAdditionalMemberOptions.UseItemPropertyName | SymbolDisplayAdditionalMemberOptions.UseOperatorName, includeContainingNamespace: Options.IncludeContainingNamespace(IncludeContainingNamespaceFilter.SeeAlso));
                        WriteEndBulletItem();

                    } while (en.MoveNext());

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

        public virtual void WriteAppliesTo(ISymbol symbol, ImmutableArray<SourceReference> sourceReferences, int headingLevelBase = 0)
        {
            Debug.Assert(sourceReferences.Any(), symbol.ToDisplayString());

            IEnumerator<SourceReference> en = sourceReferences
                .OrderByDescending(f => f.Version)
                .GetEnumerator();

            if (en.MoveNext())
            {
                WriteHeading(2 + headingLevelBase, Resources.AppliesToTitle);

                while (true)
                {
                    WriteLinkOrText(en.Current.Version.ToString(), en.Current.Url);

                    if (en.MoveNext())
                    {
                        WriteString(", ");
                    }
                    else
                    {
                        break;
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
                WriteLineBreak();
            }

            element.WriteContentTo(this);
        }

        internal void WriteClassHierarchy(
            INamedTypeSymbol baseType,
            IEnumerable<INamedTypeSymbol> types,
            bool includeContainingNamespace = false,
            int maxItems = -1,
            string allItemsHeading = null,
            string allItemsLinkTitle = null)
        {
            if (maxItems == 0)
                return;

            var nodes = new HashSet<INamedTypeSymbol>(types, MetadataNameEqualityComparer<INamedTypeSymbol>.Instance) { baseType };

            foreach (INamedTypeSymbol type in types)
            {
                INamedTypeSymbol t = type.BaseType;

                while (t != null)
                {
                    nodes.Add(t.OriginalDefinition);
                    t = t.BaseType;
                }
            }

            var baseTypes = new List<INamedTypeSymbol>();
            int count = 0;
            bool isMaxReached = false;

            WriteStartBulletList();
            WriteClassHierarchy(ImmutableHashSet<INamedTypeSymbol>.Empty);
            WriteEndBulletList();

            void WriteClassHierarchy(ImmutableHashSet<INamedTypeSymbol> duplicates)
            {
                nodes.Remove(baseType);

                List<INamedTypeSymbol> derivedTypes = nodes
                    .Where(f => MetadataNameEqualityComparer<INamedTypeSymbol>.Instance.Equals(f.BaseType?.OriginalDefinition, baseType.OriginalDefinition)
                        || f.Interfaces.Any(i => MetadataNameEqualityComparer<INamedTypeSymbol>.Instance.Equals(i.OriginalDefinition, baseType.OriginalDefinition)))
                    .OrderBy(f => f, SymbolComparer.Create(systemNamespaceFirst: Options.PlaceSystemNamespaceFirst, includeNamespaces: includeContainingNamespace))
                    .ToList();

                ImmutableHashSet<INamedTypeSymbol> derivedTypesDuplicates = (includeContainingNamespace)
                    ? ImmutableHashSet<INamedTypeSymbol>.Empty
                    : GetSymbolDisplayDuplicates(derivedTypes);

                WriteStartBulletItem();

                for (int i = 0; i < baseTypes.Count; i++)
                {
                    WriteEntityRef("ensp");
                    WriteSpace();
                    WriteStartLink();
                    WriteEntityRef("bull");
                    WriteEndLink("#" + CreateLocalLink(baseTypes[i]), baseTypes[i].ToDisplayString(TypeSymbolDisplayFormats.Name_ContainingTypes_TypeParameters));
                    WriteSpace();
                }

                WriteEntityRef("ensp");
                WriteSpace();

                bool isExternal = DocumentationModel.IsExternal(baseType);

                if (isExternal)
                    WriteString("(");

                WriteTypeListItem(baseType, duplicates, includeContainingNamespace: includeContainingNamespace);

                if (isExternal)
                    WriteString(")");

                WriteLinkDestination(CreateLocalLink(baseType));

                WriteEndBulletItem();

                count++;

                baseTypes.Add(baseType);

                using (List<INamedTypeSymbol>.Enumerator en = derivedTypes.GetEnumerator())
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

                            WriteClassHierarchy(derivedTypesDuplicates);

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

                        } while (en.MoveNext());
                    }
                }

                baseTypes.RemoveAt(baseTypes.Count - 1);
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

            static string CreateLocalLink(ISymbol symbol)
            {
                return DocumentationUtility.CreateLocalLink(symbol, "class-hierarchy-");
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
            bool canIncludeInterfaceImplementation = true,
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
                            && !SymbolEqualityComparer.Default.Equals(symbol.ContainingType, containingType);

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
                                WriteInheritedFrom(symbol.ContainingType.OriginalDefinition, TypeSymbolDisplayFormats.Name_ContainingTypes_TypeParameters, additionalOptions);
                        }
                        else
                        {
                            if (Options.IncludeMemberOverrides)
                                WriteOverrides(symbol);

                            if (canIncludeInterfaceImplementation
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

                    } while (en.MoveNext());

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
                        WriteLink(overriddenSymbol, TypeSymbolDisplayFormats.Name_TypeParameters, additionalOptions);
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
                            WriteLink(en.Current, TypeSymbolDisplayFormats.Name_TypeParameters, additionalOptions);

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
                            WriteSymbol(en.Current.Symbol, TypeSymbolDisplayFormats.Name);

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

        internal void WriteTypeList(
            IEnumerable<INamedTypeSymbol> symbols,
            string heading,
            int headingLevel,
            int maxItems = -1,
            string allItemsHeading = null,
            string allItemsLinkTitle = null,
            bool includeContainingNamespace = false,
            bool addLinkForTypeParameters = false,
            bool canCreateExternalUrl = true,
            int addSeparatorAtIndex = -1)
        {
            if (maxItems == 0)
                return;

            using (IEnumerator<INamedTypeSymbol> en = symbols
                .OrderBy(f => f, SymbolComparer.Create(systemNamespaceFirst: Options.PlaceSystemNamespaceFirst, includeNamespaces: includeContainingNamespace))
                .GetEnumerator())
            {
                if (en.MoveNext())
                {
                    ImmutableHashSet<INamedTypeSymbol> duplicates = ImmutableHashSet<INamedTypeSymbol>.Empty;

                    if (!includeContainingNamespace)
                        duplicates = GetSymbolDisplayDuplicates(symbols);

                    if (heading != null)
                        WriteHeading(headingLevel, heading);

                    WriteStartBulletList();

                    int count = 0;

                    do
                    {
                        WriteStartBulletItem();
                        WriteTypeListItem(en.Current, duplicates, includeContainingNamespace: includeContainingNamespace, addLinkForTypeParameters: addLinkForTypeParameters, canCreateExternalUrl: canCreateExternalUrl);
                        WriteEndBulletItem();

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

                    } while (en.MoveNext());

                    WriteEndBulletList();
                }
            }
        }

        internal void WriteTypeListGroupedByNamespace(
            IEnumerable<INamedTypeSymbol> symbols,
            int headingLevel = 2,
            bool addLinkForTypeParameters = false,
            bool canCreateExternalUrl = true)
        {
            foreach (IGrouping<INamespaceSymbol, INamedTypeSymbol> typesByNamespace in symbols
                .GroupBy(f => f.ContainingNamespace, MetadataNameEqualityComparer<INamespaceSymbol>.Instance)
                .OrderBy(f => f.Key, (Options.PlaceSystemNamespaceFirst) ? SymbolDefinitionComparer.SystemFirst : SymbolDefinitionComparer.Default)
)
            {
                WriteStartHeading(headingLevel);
                WriteLink(typesByNamespace.Key, TypeSymbolDisplayFormats.Name_ContainingTypes_Namespaces);
                WriteSpace();
                WriteString(Resources.NamespaceTitle);
                WriteEndHeading();

                foreach (IGrouping<TypeKind, INamedTypeSymbol> typesByTypeKind in typesByNamespace
                    .GroupBy(f => f.TypeKind)
                    .OrderBy(f => f.Key, TypeKindComparer.Instance))
                {
                    WriteHeading(headingLevel + 1, Resources.GetPluralName(typesByTypeKind.Key));
                    WriteStartBulletList();

                    foreach (INamedTypeSymbol symbol in typesByTypeKind.OrderBy(f => f, SymbolDefinitionComparer.SystemFirstOmitContainingNamespace))
                    {
                        WriteStartBulletItem();
                        WriteTypeListItem(symbol, ImmutableHashSet<INamedTypeSymbol>.Empty, addLinkForTypeParameters: addLinkForTypeParameters, canCreateExternalUrl: canCreateExternalUrl);
                        WriteEndBulletItem();
                    }

                    WriteEndBulletList();
                }
            }
        }

        private void WriteTypeListItem(
            INamedTypeSymbol symbol,
            ImmutableHashSet<INamedTypeSymbol> duplicates,
            bool includeContainingNamespace = false,
            bool addLinkForTypeParameters = false,
            bool canCreateExternalUrl = true)
        {
            WriteObsolete(symbol);

            bool isExternalSymbolWithoutUrl = DocumentationModel.IsExternal(symbol)
                && !UrlProvider.HasExternalUrl(symbol);

            if (includeContainingNamespace
                || duplicates.Contains(symbol)
                || isExternalSymbolWithoutUrl)
            {
                WriteContainingNamespacePrefix(symbol);
            }

            if (isExternalSymbolWithoutUrl)
            {
                WriteSymbol(symbol, TypeSymbolDisplayFormats.Name_ContainingTypes_TypeParameters);
            }
            else if (addLinkForTypeParameters)
            {
                WriteTypeLink(symbol, canCreateExternalUrl: canCreateExternalUrl);
            }
            else
            {
                WriteLink(symbol, TypeSymbolDisplayFormats.Name_ContainingTypes_TypeParameters, canCreateExternalUrl: canCreateExternalUrl);
            }
        }

        private ImmutableHashSet<INamedTypeSymbol> GetSymbolDisplayDuplicates(IEnumerable<INamedTypeSymbol> typeSymbols)
        {
            return typeSymbols
                .Where(symbol => !IncludesContainingNamespace(symbol))
                .GroupBy(symbol => symbol.ToDisplayString(TypeSymbolDisplayFormats.Name_ContainingTypes_TypeParameters), StringComparer.InvariantCulture)
                .Where(grouping =>
                {
                    using (IEnumerator<INamedTypeSymbol> en = grouping.GetEnumerator())
                    {
                        return en.MoveNext()
                            && en.MoveNext();
                    }
                })
                .SelectMany(grouping => grouping)
                .ToImmutableHashSet();

            bool IncludesContainingNamespace(INamedTypeSymbol symbol)
            {
                return DocumentationModel.IsExternal(symbol)
                    && !UrlProvider.HasExternalUrl(symbol)
                    && (Options.IncludeSystemNamespace || !symbol.ContainingNamespace.IsSystemNamespace());
            }
        }

        internal void WriteNamespaceList(
            IEnumerable<INamespaceSymbol> symbols,
            string heading,
            int headingLevel)
        {
            using (IEnumerator<INamespaceSymbol> en = symbols
                .OrderBy(f => f, (Options.PlaceSystemNamespaceFirst) ? SymbolDefinitionComparer.SystemFirst : SymbolDefinitionComparer.Default)
                .GetEnumerator())
            {
                if (en.MoveNext())
                {
                    if (heading != null)
                        WriteHeading(headingLevel, heading);

                    WriteStartBulletList();

                    do
                    {
                        WriteStartBulletItem();
                        WriteLink(en.Current, TypeSymbolDisplayFormats.Name_ContainingTypes_Namespaces);
                        WriteEndBulletItem();

                    } while (en.MoveNext());

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

        internal void WriteLink(
            ISymbol symbol,
            SymbolDisplayFormat format,
            SymbolDisplayAdditionalMemberOptions additionalOptions = SymbolDisplayAdditionalMemberOptions.None,
            bool includeContainingNamespace = false,
            bool canCreateExternalUrl = true)
        {
            string url = GetUrl(symbol, canCreateExternalUrl);

            if (!string.IsNullOrEmpty(url))
            {
                Debug.Assert(symbol.Kind == SymbolKind.Namespace || format.TypeQualificationStyle != SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

                if (includeContainingNamespace
                    && format.TypeQualificationStyle != SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces)
                {
                    WriteContainingNamespacePrefix(symbol);
                }

                WriteLink(symbol.ToDisplayString(format, additionalOptions), url);
            }
            else
            {
                WriteString(symbol.ToDisplayString(format, additionalOptions));
            }
        }

        internal void WriteTypeLink(
            ITypeSymbol typeSymbol,
            bool includeContainingNamespace = false,
            bool includeContainingTypes = true,
            bool canCreateExternalUrl = true)
        {
            ImmutableArray<SymbolDisplayPart> parts = typeSymbol.ToDisplayParts(TypeSymbolDisplayFormats.Name_ContainingTypes_Namespaces_GlobalNamespace_TypeParameters);

            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i].IsGlobalNamespace()
                    && Peek(i).IsPunctuation("::")
                    && Peek(i + 1).IsNamespaceOrTypeName())
                {
                    i += 2;

                    while (Peek(i).IsPunctuation(".")
                        && Peek(i + 1).IsNamespaceOrTypeName())
                    {
                        i += 2;
                    }

                    ISymbol symbol = parts[i].Symbol;

                    if (includeContainingNamespace
                        && symbol.IsKind(SymbolKind.NamedType))
                    {
                        WriteContainingNamespacePrefix(symbol);
                    }

                    bool includeTypeParameters = false;

                    if (Peek(i).IsPunctuation("<")
                        && symbol.IsDefinition
                        && symbol is INamedTypeSymbol namedTypeSymbol
                        && namedTypeSymbol.Arity > 0)
                    {
                        i += 2;

                        while (!parts[i].IsPunctuation(">"))
                            i++;

                        while (Peek(i).IsPunctuation(".")
                            && Peek(i + 1).IsTypeName())
                        {
                            i += 2;
                            symbol = parts[i].Symbol;
                        }

                        includeTypeParameters = true;
                    }

                    SymbolDisplayFormat format = TypeSymbolDisplayFormats.GetFormat(
                        includeNamespaces: false,
                        includeContainingTypes: includeContainingTypes,
                        includeTypeParameters: includeTypeParameters);

                    string url = GetUrl(symbol, canCreateExternalUrl);

                    WriteLinkOrText(symbol.ToDisplayString(format), url);
                    continue;
                }

                WriteString(parts[i].ToString());
            }

            SymbolDisplayPart Peek(int index)
            {
                if (index + 1 < parts.Length)
                    return parts[index + 1];

                return default;
            }
        }

        private void WriteObsolete(ISymbol symbol)
        {
            if (Options.MarkObsolete
                && symbol.HasAttribute(MetadataNames.System_ObsoleteAttribute))
            {
                WriteString("[");
                WriteString(Resources.DeprecatedTitle);
                WriteString("]");
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
                                            return model.GetExplicitImplementations();

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
                                    return model.GetExplicitImplementations();
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
