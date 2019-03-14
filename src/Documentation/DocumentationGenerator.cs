// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Roslynator.CSharp;

namespace Roslynator.Documentation
{
    public abstract class DocumentationGenerator
    {
        private ImmutableArray<RootDocumentationParts> _enabledAndSortedRootParts;
        private ImmutableArray<NamespaceDocumentationParts> _enabledAndSortedNamespaceParts;
        private ImmutableArray<TypeDocumentationParts> _enabledAndSortedTypeParts;
        private ImmutableArray<MemberDocumentationParts> _enabledAndSortedMemberParts;

        protected DocumentationGenerator(
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

        public DocumentationOptions Options { get; }

        public DocumentationResources Resources { get; }

        public DocumentationUrlProvider UrlProvider { get; }

        public virtual IComparer<RootDocumentationParts> RootPartComparer
        {
            get { return RootDocumentationPartComparer.Instance; }
        }

        public virtual IComparer<NamespaceDocumentationParts> NamespacePartComparer
        {
            get { return NamespaceDocumentationPartComparer.Instance; }
        }

        public virtual IComparer<TypeDocumentationParts> TypePartComparer
        {
            get { return TypeDocumentationPartComparer.Instance; }
        }

        public virtual IComparer<MemberDocumentationParts> MemberPartComparer
        {
            get { return MemberDocumentationPartComparer.Instance; }
        }

        internal ImmutableArray<RootDocumentationParts> EnabledAndSortedRootParts
        {
            get
            {
                if (_enabledAndSortedRootParts.IsDefault)
                {
                    _enabledAndSortedRootParts = Enum.GetValues(typeof(RootDocumentationParts))
                        .Cast<RootDocumentationParts>()
                        .Where(f => f != RootDocumentationParts.None
                            && f != RootDocumentationParts.All
                            && f != RootDocumentationParts.Types
                            && (Options.IgnoredRootParts & f) == 0)
                        .OrderBy(f => f, RootPartComparer)
                        .ToImmutableArray();
                }

                return _enabledAndSortedRootParts;
            }
        }

        internal ImmutableArray<NamespaceDocumentationParts> EnabledAndSortedNamespaceParts
        {
            get
            {
                if (_enabledAndSortedNamespaceParts.IsDefault)
                {
                    _enabledAndSortedNamespaceParts = Enum.GetValues(typeof(NamespaceDocumentationParts))
                        .Cast<NamespaceDocumentationParts>()
                        .Where(f => f != NamespaceDocumentationParts.None
                            && f != NamespaceDocumentationParts.All
                            && (Options.IgnoredNamespaceParts & f) == 0)
                        .OrderBy(f => f, NamespacePartComparer)
                        .ToImmutableArray();
                }

                return _enabledAndSortedNamespaceParts;
            }
        }

        internal ImmutableArray<TypeDocumentationParts> EnabledAndSortedTypeParts
        {
            get
            {
                if (_enabledAndSortedTypeParts.IsDefault)
                {
                    _enabledAndSortedTypeParts = Enum.GetValues(typeof(TypeDocumentationParts))
                        .Cast<TypeDocumentationParts>()
                        .Where(f => f != TypeDocumentationParts.None
                            && f != TypeDocumentationParts.All
                            && f != TypeDocumentationParts.NestedTypes
                            && f != TypeDocumentationParts.AllExceptNestedTypes
                            && (Options.IgnoredTypeParts & f) == 0)
                        .OrderBy(f => f, TypePartComparer)
                        .ToImmutableArray();
                }

                return _enabledAndSortedTypeParts;
            }
        }

        internal ImmutableArray<MemberDocumentationParts> EnabledAndSortedMemberParts
        {
            get
            {
                if (_enabledAndSortedMemberParts.IsDefault)
                {
                    _enabledAndSortedMemberParts = Enum.GetValues(typeof(MemberDocumentationParts))
                        .Cast<MemberDocumentationParts>()
                        .Where(f => f != MemberDocumentationParts.None
                            && f != MemberDocumentationParts.All
                            && (Options.IgnoredMemberParts & f) == 0)
                        .OrderBy(f => f, MemberPartComparer)
                        .ToImmutableArray();
                }

                return _enabledAndSortedMemberParts;
            }
        }

        private DocumentationWriter CreateWriter(ISymbol currentSymbol = null)
        {
            DocumentationWriter writer = CreateWriterCore();

            writer.CurrentSymbol = currentSymbol;
            writer.CanCreateMemberLocalUrl = Options.Depth == DocumentationDepth.Member;
            writer.CanCreateTypeLocalUrl = Options.Depth <= DocumentationDepth.Type;

            return writer;
        }

        protected abstract DocumentationWriter CreateWriterCore();

        public IEnumerable<DocumentationGeneratorResult> Generate(string heading = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            DocumentationDepth depth = Options.Depth;

            DocumentationGeneratorResult objectModel = default;
            DocumentationGeneratorResult externalTypesExtensions = GenerateExternalTypesExtensions();

            using (DocumentationWriter writer = CreateWriter())
            {
                yield return GenerateRoot(writer, heading, addExtensionsLink: externalTypesExtensions.HasContent);
            }

            if (depth <= DocumentationDepth.Namespace)
            {
                IEnumerable<INamedTypeSymbol> typeSymbols = DocumentationModel.Types.Where(f => !Options.ShouldBeIgnored(f));

                foreach (INamespaceSymbol namespaceSymbol in typeSymbols
                    .Select(f => f.ContainingNamespace)
                    .Distinct(MetadataNameEqualityComparer<INamespaceSymbol>.Instance))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    yield return GenerateNamespace(namespaceSymbol);
                }

                if (depth <= DocumentationDepth.Type)
                {
                    foreach (INamedTypeSymbol typeSymbol in typeSymbols)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        if (!Options.ShouldBeIgnored(typeSymbol))
                        {
                            TypeDocumentationModel typeModel = DocumentationModel.GetTypeModel(typeSymbol);

                            yield return GenerateType(typeModel);

                            if (depth == DocumentationDepth.Member)
                            {
                                foreach (DocumentationGeneratorResult result in GenerateMembers(typeModel))
                                {
                                    cancellationToken.ThrowIfCancellationRequested();

                                    yield return result;
                                }
                            }
                        }
                    }
                }
            }

            if (objectModel.HasContent)
                yield return objectModel;

            if (externalTypesExtensions.HasContent)
            {
                yield return externalTypesExtensions;

                foreach (INamedTypeSymbol typeSymbol in DocumentationModel.GetExtendedExternalTypes())
                {
                    if (!Options.ShouldBeIgnored(typeSymbol))
                    {
                        yield return GenerateExtendedExternalType(typeSymbol);
                    }
                }
            }
        }

        public DocumentationGeneratorResult GenerateRoot(
            string heading,
            bool addExtensionsLink = false)
        {
            using (DocumentationWriter writer = CreateWriter())
            {
                return GenerateRoot(writer, heading, addExtensionsLink: addExtensionsLink);
            }
        }

        internal DocumentationGeneratorResult GenerateRoot(
            DocumentationWriter writer,
            string heading,
            bool addExtensionsLink = false)
        {
            writer.WriteStartDocument();

            if (Options.ScrollToContent)
                writer.WriteLinkDestination(WellKnownNames.TopFragmentName);

            writer.WriteStartHeading(1);
            writer.WriteString(heading);
            writer.WriteEndHeading();

            GenerateRoot(writer, addExtensionsLink: addExtensionsLink);

            writer.WriteEndDocument();

            return CreateResult(writer, DocumentationFileKind.Root);
        }

        private void GenerateRoot(DocumentationWriter writer, bool addExtensionsLink = false)
        {
            SymbolDisplayFormat format = SymbolDisplayFormats.TypeNameAndContainingTypesAndTypeParameters;

            IEnumerable<INamedTypeSymbol> typeSymbols = DocumentationModel.Types.Where(f => !Options.ShouldBeIgnored(f));

            foreach (RootDocumentationParts part in EnabledAndSortedRootParts)
            {
                switch (part)
                {
                    case RootDocumentationParts.Content:
                        {
                            IEnumerable<string> names = EnabledAndSortedRootParts
                                .Where(HasContent)
                                .OrderBy(f => f, RootPartComparer)
                                .Select(f => Resources.GetHeading(f));

                            writer.WriteContent(names);
                            break;
                        }
                    case RootDocumentationParts.Namespaces:
                        {
                            IEnumerable<INamespaceSymbol> namespaceSymbols = typeSymbols
                                .Select(f => f.ContainingNamespace)
                                .Distinct(MetadataNameEqualityComparer<INamespaceSymbol>.Instance);

                            writer.WriteList(namespaceSymbols, Resources.NamespacesTitle, 2, SymbolDisplayFormats.TypeNameAndContainingTypesAndNamespaces);
                            break;
                        }
                    case RootDocumentationParts.Classes:
                        {
                            if (Options.IncludeClassHierarchy)
                            {
                                if (typeSymbols.Any(f => !f.IsStatic && f.TypeKind == TypeKind.Class))
                                {
                                    INamedTypeSymbol objectType = DocumentationModel.Compilations[0].ObjectType;

                                    IEnumerable<INamedTypeSymbol> instanceClasses = typeSymbols.Where(f => !f.IsStatic && f.TypeKind == TypeKind.Class);

                                    writer.WriteHeading2(Resources.GetPluralName(TypeKind.Class));

                                    writer.WriteClassHierarchy(objectType, instanceClasses, includeContainingNamespace: Options.IncludeContainingNamespace(OmitContainingNamespaceParts.Root));

                                    writer.WriteLine();
                                }
                            }
                            else
                            {
                                writer.WriteList(
                                    typeSymbols.Where(f => f.TypeKind == TypeKind.Class),
                                    Resources.ClassesTitle,
                                    2,
                                    SymbolDisplayFormats.TypeNameAndContainingTypes,
                                    includeContainingNamespace: Options.IncludeContainingNamespace(OmitContainingNamespaceParts.Root));

                                break;
                            }

                            break;
                        }
                    case RootDocumentationParts.StaticClasses:
                        {
                            if (Options.IncludeClassHierarchy)
                                WriteTypesImpl(f => f.IsStatic && f.TypeKind == TypeKind.Class, Resources.StaticClassesTitle);

                            break;
                        }
                    case RootDocumentationParts.Structs:
                        {
                            WriteTypes(TypeKind.Struct);
                            break;
                        }
                    case RootDocumentationParts.Interfaces:
                        {
                            WriteTypes(TypeKind.Interface);
                            break;
                        }
                    case RootDocumentationParts.Enums:
                        {
                            WriteTypes(TypeKind.Enum);
                            break;
                        }
                    case RootDocumentationParts.Delegates:
                        {
                            WriteTypes(TypeKind.Delegate);
                            break;
                        }
                    case RootDocumentationParts.Other:
                        {
                            if (addExtensionsLink)
                            {
                                writer.WriteHeading2(Resources.OtherTitle);
                                writer.WriteStartBulletItem();
                                writer.WriteLink(Resources.ExtensionsOfExternalTypesTitle, WellKnownNames.Extensions + ((Options.ScrollToContent) ? "#" + WellKnownNames.TopFragmentName : null));
                                writer.WriteEndBulletItem();
                            }

                            break;
                        }
                }
            }

            void WriteTypes(TypeKind typeKind)
            {
                WriteTypesImpl(f => f.TypeKind == typeKind, Resources.GetPluralName(typeKind));
            }

            void WriteTypesImpl(Func<INamedTypeSymbol, bool> predicate, string heading)
            {
                using (IEnumerator<INamedTypeSymbol> en = typeSymbols
                    .Where(predicate)
                    .Sort(systemNamespaceFirst: Options.PlaceSystemNamespaceFirst, includeContainingNamespace: Options.IncludeContainingNamespace(OmitContainingNamespaceParts.Root))
                    .GetEnumerator())
                {
                    if (en.MoveNext())
                    {
                        writer.WriteHeading2(heading);

                        do
                        {
                            WriteBulletItemLink(en.Current);
                        }
                        while (en.MoveNext());
                    }
                }
            }

            void WriteBulletItemLink(INamedTypeSymbol typeSymbol)
            {
                writer.WriteStartBulletItem();

                INamespaceSymbol containingNamespace = typeSymbol.ContainingNamespace;

                if (!containingNamespace.IsGlobalNamespace)
                {
                    writer.WriteNamespaceSymbol(containingNamespace);
                    writer.WriteString(".");
                }

                writer.WriteLink(typeSymbol, format);
                writer.WriteEndBulletItem();
            }

            bool HasContent(RootDocumentationParts part)
            {
                switch (part)
                {
                    case RootDocumentationParts.Content:
                        {
                            return false;
                        }
                    case RootDocumentationParts.Namespaces:
                        {
                            return typeSymbols.Any();
                        }
                    case RootDocumentationParts.Classes:
                        {
                            return typeSymbols.Any(f => !f.IsStatic && f.TypeKind == TypeKind.Class);
                        }
                    case RootDocumentationParts.StaticClasses:
                        {
                            return Options.IncludeClassHierarchy
                                && typeSymbols.Any(f => f.IsStatic && f.TypeKind == TypeKind.Class);
                        }
                    case RootDocumentationParts.Structs:
                        {
                            return typeSymbols.Any(f => f.TypeKind == TypeKind.Struct);
                        }
                    case RootDocumentationParts.Interfaces:
                        {
                            return typeSymbols.Any(f => f.TypeKind == TypeKind.Interface);
                        }
                    case RootDocumentationParts.Enums:
                        {
                            return typeSymbols.Any(f => f.TypeKind == TypeKind.Enum);
                        }
                    case RootDocumentationParts.Delegates:
                        {
                            return typeSymbols.Any(f => f.TypeKind == TypeKind.Delegate);
                        }
                    case RootDocumentationParts.Other:
                        {
                            return addExtensionsLink;
                        }
                    default:
                        {
                            throw new InvalidOperationException();
                        }
                }
            }
        }

        private DocumentationGeneratorResult GenerateNamespace(INamespaceSymbol namespaceSymbol)
        {
            IEnumerable<INamedTypeSymbol> typeSymbols = DocumentationModel
                .Types
                .Where(f => MetadataNameEqualityComparer<INamespaceSymbol>.Instance.Equals(f.ContainingNamespace, namespaceSymbol));

            using (DocumentationWriter writer = CreateWriter(namespaceSymbol))
            {
                writer.WriteStartDocument();

                SymbolXmlDocumentation xmlDocumentation = DocumentationModel.GetXmlDocumentation(namespaceSymbol, Options.PreferredCultureName);

                writer.WriteHeading(1, namespaceSymbol, SymbolDisplayFormats.TypeNameAndContainingTypesAndNamespaces, addLink: false, linkDestination: (Options.ScrollToContent) ? WellKnownNames.TopFragmentName : null);

                foreach (NamespaceDocumentationParts part in EnabledAndSortedNamespaceParts)
                {
                    switch (part)
                    {
                        case NamespaceDocumentationParts.Content:
                            {
                                IEnumerable<string> names = EnabledAndSortedNamespaceParts
                                    .Where(HasContent)
                                    .OrderBy(f => f, NamespacePartComparer)
                                    .Select(f => Resources.GetHeading(f));

                                writer.WriteContent(names, addLinkToRoot: true);
                                break;
                            }
                        case NamespaceDocumentationParts.ContainingNamespace:
                            {
                                INamespaceSymbol containingNamespace = namespaceSymbol.ContainingNamespace;

                                if (containingNamespace?.IsGlobalNamespace == false)
                                    writer.WriteContainingNamespace(containingNamespace, Resources.ContainingNamespaceTitle);

                                break;
                            }
                        case NamespaceDocumentationParts.Summary:
                            {
                                xmlDocumentation?.Element(WellKnownXmlTags.Summary)?.WriteContentTo(writer);
                                break;
                            }
                        case NamespaceDocumentationParts.Examples:
                            {
                                if (xmlDocumentation != null)
                                    writer.WriteExamples(namespaceSymbol, xmlDocumentation);

                                break;
                            }
                        case NamespaceDocumentationParts.Remarks:
                            {
                                if (xmlDocumentation != null)
                                    writer.WriteRemarks(namespaceSymbol, xmlDocumentation);

                                break;
                            }
                        case NamespaceDocumentationParts.Classes:
                            {
                                WriteTypes(typeSymbols, TypeKind.Class);
                                break;
                            }
                        case NamespaceDocumentationParts.Structs:
                            {
                                WriteTypes(typeSymbols, TypeKind.Struct);
                                break;
                            }
                        case NamespaceDocumentationParts.Interfaces:
                            {
                                WriteTypes(typeSymbols, TypeKind.Interface);
                                break;
                            }
                        case NamespaceDocumentationParts.Enums:
                            {
                                WriteTypes(typeSymbols, TypeKind.Enum);
                                break;
                            }
                        case NamespaceDocumentationParts.Delegates:
                            {
                                WriteTypes(typeSymbols, TypeKind.Delegate);
                                break;
                            }
                        case NamespaceDocumentationParts.SeeAlso:
                            {
                                if (xmlDocumentation != null)
                                    writer.WriteSeeAlso(namespaceSymbol, xmlDocumentation);

                                break;
                            }
                        default:
                            {
                                throw new InvalidOperationException();
                            }
                    }
                }

                writer.WriteEndDocument();

                return CreateResult(writer, DocumentationFileKind.Namespace, namespaceSymbol);

                void WriteTypes(
                    IEnumerable<INamedTypeSymbol> types,
                    TypeKind typeKind)
                {
                    writer.WriteTable(
                        types.Where(f => f.TypeKind == typeKind),
                        Resources.GetPluralName(typeKind),
                        headingLevel: 2,
                        Resources.GetName(typeKind),
                        Resources.SummaryTitle,
                        SymbolDisplayFormats.TypeNameAndContainingTypesAndTypeParameters,
                        addLink: Options.Depth <= DocumentationDepth.Type);
                }

                bool HasContent(NamespaceDocumentationParts part)
                {
                    switch (part)
                    {
                        case NamespaceDocumentationParts.Content:
                        case NamespaceDocumentationParts.Summary:
                        case NamespaceDocumentationParts.ContainingNamespace:
                            {
                                return false;
                            }
                        case NamespaceDocumentationParts.Examples:
                            {
                                return xmlDocumentation?.HasElement(WellKnownXmlTags.Example) == true;
                            }
                        case NamespaceDocumentationParts.Remarks:
                            {
                                return xmlDocumentation?.HasElement(WellKnownXmlTags.Remarks) == true;
                            }
                        case NamespaceDocumentationParts.Classes:
                            {
                                return typeSymbols.Any(f => f.TypeKind == TypeKind.Class);
                            }
                        case NamespaceDocumentationParts.Structs:
                            {
                                return typeSymbols.Any(f => f.TypeKind == TypeKind.Struct);
                            }
                        case NamespaceDocumentationParts.Interfaces:
                            {
                                return typeSymbols.Any(f => f.TypeKind == TypeKind.Interface);
                            }
                        case NamespaceDocumentationParts.Enums:
                            {
                                return typeSymbols.Any(f => f.TypeKind == TypeKind.Enum);
                            }
                        case NamespaceDocumentationParts.Delegates:
                            {
                                return typeSymbols.Any(f => f.TypeKind == TypeKind.Delegate);
                            }
                        case NamespaceDocumentationParts.SeeAlso:
                            {
                                return xmlDocumentation?.Elements(WellKnownXmlTags.SeeAlso).Any() == true;
                            }
                        default:
                            {
                                throw new InvalidOperationException();
                            }
                    }
                }
            }
        }

        private DocumentationGeneratorResult GenerateExternalTypesExtensions()
        {
            IEnumerable<INamedTypeSymbol> extendedExternalTypes = DocumentationModel.GetExtendedExternalTypes()
                .Where(f => !Options.ShouldBeIgnored(f));

            IEnumerable<INamespaceSymbol> namespaces = extendedExternalTypes
                .Select(f => f.ContainingNamespace)
                .Distinct(MetadataNameEqualityComparer<INamespaceSymbol>.Instance);

            if (!namespaces.Any())
                return CreateResult(null, DocumentationFileKind.Extensions);

            using (DocumentationWriter writer = CreateWriter())
            {
                writer.WriteStartDocument();

                if (Options.ScrollToContent)
                {
                    writer.WriteLinkDestination(WellKnownNames.TopFragmentName);
                    writer.WriteLine();
                }

                writer.WriteStartHeading(1);
                writer.WriteString(Resources.ExtensionsOfExternalTypesTitle);
                writer.WriteEndHeading();

                writer.WriteLink(Resources.HomeTitle, UrlProvider.GetUrlToRoot(0, '/', scrollToContent: Options.ScrollToContent));
                writer.WriteContentSeparator();
                writer.WriteLink(Resources.NamespacesTitle, UrlProvider.GetFragment(Resources.NamespacesTitle));

                writer.WriteContent(extendedExternalTypes
                    .Select(f => f.TypeKind.ToNamespaceDocumentationPart())
                    .Where(f => (Options.IgnoredNamespaceParts & f) == 0)
                    .Distinct()
                    .OrderBy(f => f, NamespacePartComparer)
                    .Select(f => Resources.GetHeading(f)), beginWithSeparator: true);

                writer.WriteList(namespaces, Resources.NamespacesTitle, 2, SymbolDisplayFormats.TypeNameAndContainingTypesAndNamespaces);

                foreach (IGrouping<TypeKind, INamedTypeSymbol> typesByKind in extendedExternalTypes
                    .Where(f => (Options.IgnoredNamespaceParts & f.TypeKind.ToNamespaceDocumentationPart()) == 0)
                    .GroupBy(f => f.TypeKind)
                    .OrderBy(f => f.Key.ToNamespaceDocumentationPart(), NamespacePartComparer))
                {
                    writer.WriteList(
                        typesByKind,
                        Resources.GetPluralName(typesByKind.Key),
                        headingLevel: 2,
                        SymbolDisplayFormats.TypeNameAndContainingTypesAndTypeParameters,
                        includeContainingNamespace: true,
                        canCreateExternalUrl: false);
                }

                writer.WriteEndDocument();

                return CreateResult(writer, DocumentationFileKind.Extensions);
            }
        }

        private DocumentationGeneratorResult GenerateExtendedExternalType(INamedTypeSymbol typeSymbol)
        {
            using (DocumentationWriter writer = CreateWriter(typeSymbol))
            {
                writer.WriteStartDocument();

                if (Options.ScrollToContent)
                {
                    writer.WriteLinkDestination(WellKnownNames.TopFragmentName);
                    writer.WriteLine();
                }

                writer.WriteStartHeading(1);
                writer.WriteLink(typeSymbol, SymbolDisplayFormats.TypeNameAndContainingTypesAndTypeParameters);
                writer.WriteSpace();
                writer.WriteString(Resources.GetName(typeSymbol.TypeKind));
                writer.WriteSpace();
                writer.WriteString(Resources.ExtensionsTitle);
                writer.WriteEndHeading();
                writer.WriteContent(Array.Empty<string>(), addLinkToRoot: true);

                writer.WriteTable(
                    DocumentationModel.GetExtensionMethods(typeSymbol),
                    heading: null,
                    headingLevel: -1,
                    Resources.ExtensionMethodTitle,
                    Resources.SummaryTitle,
                    SymbolDisplayFormats.SimpleDeclaration);

                writer.WriteEndDocument();

                return CreateResult(writer, DocumentationFileKind.Type, typeSymbol);
            }
        }

        private DocumentationGeneratorResult GenerateType(TypeDocumentationModel typeModel)
        {
            INamedTypeSymbol typeSymbol = typeModel.Symbol;

            ImmutableArray<INamedTypeSymbol> derivedTypes = ImmutableArray<INamedTypeSymbol>.Empty;

            if (EnabledAndSortedTypeParts.Contains(TypeDocumentationParts.Derived))
            {
                derivedTypes = (Options.IncludeAllDerivedTypes)
                    ? DocumentationModel.GetAllDerivedTypes(typeSymbol).ToImmutableArray()
                    : DocumentationModel.GetDerivedTypes(typeSymbol).ToImmutableArray();
            }

            bool includeInherited = typeModel.TypeKind != TypeKind.Interface || Options.IncludeInheritedInterfaceMembers;

            SymbolXmlDocumentation xmlDocumentation = DocumentationModel.GetXmlDocumentation(typeModel.Symbol, Options.PreferredCultureName);

            using (DocumentationWriter writer = CreateWriter(typeSymbol))
            {
                writer.WriteStartDocument();

                writer.WriteHeading(
                    1,
                    typeSymbol,
                    SymbolDisplayFormats.TypeNameAndContainingTypesAndTypeParameters,
                    SymbolDisplayAdditionalMemberOptions.UseItemPropertyName | SymbolDisplayAdditionalMemberOptions.UseOperatorName,
                    addLink: false,
                    linkDestination: (Options.ScrollToContent) ? WellKnownNames.TopFragmentName : null);

                foreach (TypeDocumentationParts part in EnabledAndSortedTypeParts)
                {
                    switch (part)
                    {
                        case TypeDocumentationParts.Content:
                            {
                                IEnumerable<string> names = EnabledAndSortedTypeParts
                                    .Where(HasContent)
                                    .OrderBy(f => f, TypePartComparer)
                                    .Select(f => Resources.GetHeading(f));

                                writer.WriteContent(names, addLinkToRoot: true);
                                break;
                            }
                        case TypeDocumentationParts.ContainingNamespace:
                            {
                                INamespaceSymbol containingNamespace = typeModel.ContainingNamespace;

                                if (containingNamespace != null)
                                    writer.WriteContainingNamespace(containingNamespace, Resources.NamespaceTitle);

                                break;
                            }
                        case TypeDocumentationParts.ContainingAssembly:
                            {
                                writer.WriteContainingAssembly(typeModel.ContainingAssembly, Resources.AssemblyTitle);
                                break;
                            }
                        case TypeDocumentationParts.ObsoleteMessage:
                            {
                                if (typeModel.IsObsolete)
                                    writer.WriteObsoleteMessage(typeSymbol);

                                break;
                            }
                        case TypeDocumentationParts.Summary:
                            {
                                if (xmlDocumentation != null)
                                    writer.WriteSummary(typeSymbol, xmlDocumentation);

                                break;
                            }
                        case TypeDocumentationParts.Declaration:
                            {
                                writer.WriteDeclaration(typeSymbol);
                                break;
                            }
                        case TypeDocumentationParts.TypeParameters:
                            {
                                writer.WriteTypeParameters(typeModel.TypeParameters);
                                break;
                            }
                        case TypeDocumentationParts.Parameters:
                            {
                                writer.WriteParameters(typeModel.Parameters);
                                break;
                            }
                        case TypeDocumentationParts.ReturnValue:
                            {
                                if (xmlDocumentation != null)
                                    writer.WriteReturnType(typeSymbol, xmlDocumentation);

                                break;
                            }
                        case TypeDocumentationParts.Inheritance:
                            {
                                writer.WriteInheritance(typeSymbol);
                                break;
                            }
                        case TypeDocumentationParts.Attributes:
                            {
                                writer.WriteAttributes(typeSymbol);
                                break;
                            }
                        case TypeDocumentationParts.Derived:
                            {
                                if (derivedTypes.Any())
                                    writer.WriteDerivedTypes(derivedTypes);

                                break;
                            }
                        case TypeDocumentationParts.Implements:
                            {
                                writer.WriteImplementedInterfaces(typeModel.GetImplementedInterfaces(Options.OmitIEnumerable));
                                break;
                            }
                        case TypeDocumentationParts.Examples:
                            {
                                if (xmlDocumentation != null)
                                    writer.WriteExamples(typeSymbol, xmlDocumentation);

                                break;
                            }
                        case TypeDocumentationParts.Remarks:
                            {
                                if (xmlDocumentation != null)
                                    writer.WriteRemarks(typeSymbol, xmlDocumentation);

                                break;
                            }
                        case TypeDocumentationParts.Constructors:
                            {
                                writer.WriteConstructors(typeModel.GetConstructors());
                                break;
                            }
                        case TypeDocumentationParts.Fields:
                            {
                                if (typeModel.TypeKind == TypeKind.Enum)
                                {
                                    writer.WriteEnumFields(typeModel.GetFields(), typeSymbol);
                                }
                                else
                                {
                                    writer.WriteFields(typeModel.GetFields(includeInherited: includeInherited), containingType: typeSymbol);
                                }

                                break;
                            }
                        case TypeDocumentationParts.Indexers:
                            {
                                writer.WriteIndexers(typeModel.GetIndexers(includeInherited: includeInherited), containingType: typeSymbol);
                                break;
                            }
                        case TypeDocumentationParts.Properties:
                            {
                                writer.WriteProperties(typeModel.GetProperties(includeInherited: includeInherited), containingType: typeSymbol);
                                break;
                            }
                        case TypeDocumentationParts.Methods:
                            {
                                writer.WriteMethods(typeModel.GetMethods(includeInherited: includeInherited), containingType: typeSymbol);
                                break;
                            }
                        case TypeDocumentationParts.Operators:
                            {
                                writer.WriteOperators(typeModel.GetOperators(includeInherited: true), containingType: typeSymbol);
                                break;
                            }
                        case TypeDocumentationParts.Events:
                            {
                                writer.WriteEvents(typeModel.GetEvents(includeInherited: includeInherited), containingType: typeSymbol);
                                break;
                            }
                        case TypeDocumentationParts.ExplicitInterfaceImplementations:
                            {
                                writer.WriteExplicitInterfaceImplementations(typeModel.GetExplicitInterfaceImplementations());
                                break;
                            }
                        case TypeDocumentationParts.ExtensionMethods:
                            {
                                writer.WriteExtensionMethods(DocumentationModel.GetExtensionMethods(typeSymbol));
                                break;
                            }
                        case TypeDocumentationParts.Classes:
                            {
                                writer.WriteTypes(typeModel.GetClasses(includeInherited: includeInherited), TypeKind.Class, typeSymbol);
                                break;
                            }
                        case TypeDocumentationParts.Structs:
                            {
                                writer.WriteTypes(typeModel.GetStructs(includeInherited: includeInherited), TypeKind.Struct, typeSymbol);
                                break;
                            }
                        case TypeDocumentationParts.Interfaces:
                            {
                                writer.WriteTypes(typeModel.GetInterfaces(includeInherited: includeInherited), TypeKind.Interface, typeSymbol);
                                break;
                            }
                        case TypeDocumentationParts.Enums:
                            {
                                writer.WriteTypes(typeModel.GetEnums(includeInherited: includeInherited), TypeKind.Enum, typeSymbol);
                                break;
                            }
                        case TypeDocumentationParts.Delegates:
                            {
                                writer.WriteTypes(typeModel.GetDelegates(includeInherited: includeInherited), TypeKind.Delegate, typeSymbol);
                                break;
                            }
                        case TypeDocumentationParts.SeeAlso:
                            {
                                if (xmlDocumentation != null)
                                    writer.WriteSeeAlso(typeSymbol, xmlDocumentation);

                                break;
                            }
                    }
                }

                if (derivedTypes.Any()
                    && derivedTypes.Length > Options.MaxDerivedTypes)
                {
                    if (Options.IncludeClassHierarchy)
                    {
                        writer.WriteHeading(2, Resources.DerivedAllTitle);

                        writer.WriteClassHierarchy(
                            typeSymbol,
                            derivedTypes,
                            includeContainingNamespace: Options.IncludeContainingNamespace(OmitContainingNamespaceParts.DerivedType),
                            addBaseType: false,
                            addSeparatorAtIndex: (derivedTypes.Length > Options.MaxDerivedTypes) ? Options.MaxDerivedTypes : -1);

                        writer.WriteLine();
                    }
                    else
                    {
                        writer.WriteList(
                            derivedTypes,
                            heading: Resources.DerivedAllTitle,
                            headingLevel: 2,
                            format: SymbolDisplayFormats.TypeNameAndContainingTypesAndTypeParameters,
                            includeContainingNamespace: Options.IncludeContainingNamespace(OmitContainingNamespaceParts.DerivedType));
                    }
                }

                writer.WriteEndDocument();

                return CreateResult(writer, DocumentationFileKind.Type, typeSymbol);
            }

            bool HasContent(TypeDocumentationParts part)
            {
                switch (part)
                {
                    case TypeDocumentationParts.Content:
                    case TypeDocumentationParts.ContainingNamespace:
                    case TypeDocumentationParts.ContainingAssembly:
                    case TypeDocumentationParts.ObsoleteMessage:
                    case TypeDocumentationParts.Summary:
                    case TypeDocumentationParts.Declaration:
                    case TypeDocumentationParts.TypeParameters:
                    case TypeDocumentationParts.Parameters:
                    case TypeDocumentationParts.ReturnValue:
                    case TypeDocumentationParts.Inheritance:
                    case TypeDocumentationParts.Attributes:
                    case TypeDocumentationParts.Derived:
                    case TypeDocumentationParts.Implements:
                        {
                            return false;
                        }
                    case TypeDocumentationParts.Examples:
                        {
                            return xmlDocumentation?.HasElement(WellKnownXmlTags.Example) == true;
                        }
                    case TypeDocumentationParts.Remarks:
                        {
                            return xmlDocumentation?.HasElement(WellKnownXmlTags.Remarks) == true;
                        }
                    case TypeDocumentationParts.Constructors:
                        {
                            return typeModel.GetConstructors().Any();
                        }
                    case TypeDocumentationParts.Fields:
                        {
                            if (typeModel.TypeKind == TypeKind.Enum)
                            {
                                return typeModel.GetFields().Any();
                            }
                            else
                            {
                                return typeModel.GetFields(includeInherited: true).Any();
                            }
                        }
                    case TypeDocumentationParts.Indexers:
                        {
                            return typeModel.GetIndexers(includeInherited: includeInherited).Any();
                        }
                    case TypeDocumentationParts.Properties:
                        {
                            return typeModel.GetProperties(includeInherited: includeInherited).Any();
                        }
                    case TypeDocumentationParts.Methods:
                        {
                            return typeModel.GetMethods(includeInherited: includeInherited).Any();
                        }
                    case TypeDocumentationParts.Operators:
                        {
                            return typeModel.GetOperators(includeInherited: true).Any();
                        }
                    case TypeDocumentationParts.Events:
                        {
                            return typeModel.GetEvents(includeInherited: includeInherited).Any();
                        }
                    case TypeDocumentationParts.ExplicitInterfaceImplementations:
                        {
                            return typeModel.GetExplicitInterfaceImplementations().Any();
                        }
                    case TypeDocumentationParts.ExtensionMethods:
                        {
                            return DocumentationModel.GetExtensionMethods(typeSymbol).Any();
                        }
                    case TypeDocumentationParts.Classes:
                        {
                            return typeModel.GetClasses(includeInherited: includeInherited).Any();
                        }
                    case TypeDocumentationParts.Structs:
                        {
                            return typeModel.GetStructs(includeInherited: includeInherited).Any();
                        }
                    case TypeDocumentationParts.Interfaces:
                        {
                            return typeModel.GetInterfaces(includeInherited: includeInherited).Any();
                        }
                    case TypeDocumentationParts.Enums:
                        {
                            return typeModel.GetEnums(includeInherited: includeInherited).Any();
                        }
                    case TypeDocumentationParts.Delegates:
                        {
                            return typeModel.GetDelegates(includeInherited: includeInherited).Any();
                        }
                    case TypeDocumentationParts.SeeAlso:
                        {
                            return xmlDocumentation?.Elements(WellKnownXmlTags.SeeAlso).Any() == true;
                        }
                    default:
                        {
                            throw new InvalidOperationException();
                        }
                }
            }
        }

        private IEnumerable<DocumentationGeneratorResult> GenerateMembers(TypeDocumentationModel typeModel)
        {
            foreach (IGrouping<string, ISymbol> grouping in typeModel
                .GetMembers(Options.IgnoredTypeParts)
                .GroupBy(f => f.Name))
            {
                using (IEnumerator<ISymbol> en = grouping.GetEnumerator())
                {
                    if (en.MoveNext())
                    {
                        ISymbol symbol = en.Current;

                        using (DocumentationWriter writer = CreateWriter(symbol))
                        {
                            writer.WriteStartDocument();

                            bool isOverloaded = en.MoveNext();

                            if (Options.ScrollToContent)
                            {
                                writer.WriteLinkDestination(WellKnownNames.TopFragmentName);
                                writer.WriteLine();
                            }

                            writer.WriteMemberTitle(symbol, isOverloaded);

                            writer.WriteContent(Array.Empty<string>(), addLinkToRoot: true);

                            if ((Options.IgnoredMemberParts & MemberDocumentationParts.ContainingType) == 0)
                                writer.WriteContainingType(symbol.ContainingType, Resources.ContainingTypeTitle);

                            if ((Options.IgnoredMemberParts & MemberDocumentationParts.ContainingAssembly) == 0)
                                writer.WriteContainingAssembly(symbol.ContainingAssembly, Resources.AssemblyTitle);

                            if (isOverloaded)
                            {
                                SymbolDisplayFormat format = SymbolDisplayFormats.SimpleDeclaration;

                                const SymbolDisplayAdditionalMemberOptions additionalOptions = SymbolDisplayAdditionalMemberOptions.UseItemPropertyName | SymbolDisplayAdditionalMemberOptions.UseOperatorName;

                                writer.WriteTable(
                                    grouping,
                                    heading: Resources.OverloadsTitle,
                                    headingLevel: 2,
                                    header1: Resources.GetName(symbol),
                                    header2: Resources.SummaryTitle,
                                    format: format,
                                    additionalOptions: additionalOptions);

                                foreach (ISymbol overloadSymbol in grouping.OrderBy(f => f.ToDisplayString(format, additionalOptions)))
                                {
                                    string id = DocumentationUrlProvider.GetFragment(overloadSymbol);

                                    writer.WriteStartHeading(2);
                                    writer.WriteString(overloadSymbol.ToDisplayString(format, additionalOptions));
                                    writer.WriteSpace();
                                    writer.WriteLinkDestination(id);
                                    writer.WriteEndHeading();

                                    GenerateMemberContent(writer, overloadSymbol, headingLevelBase: 1);
                                }
                            }
                            else
                            {
                                GenerateMemberContent(writer, symbol);
                            }

                            writer.WriteEndDocument();

                            yield return CreateResult(writer, DocumentationFileKind.Member, symbol);
                        }
                    }
                }
            }

            void GenerateMemberContent(DocumentationWriter writer, ISymbol symbol, int headingLevelBase = 0)
            {
                SymbolXmlDocumentation xmlDocumentation = DocumentationModel.GetXmlDocumentation(symbol, Options.PreferredCultureName);

                foreach (MemberDocumentationParts part in EnabledAndSortedMemberParts)
                {
                    switch (part)
                    {
                        case MemberDocumentationParts.ObsoleteMessage:
                            {
                                if (symbol.HasAttribute(MetadataNames.System_ObsoleteAttribute))
                                    writer.WriteObsoleteMessage(symbol);

                                break;
                            }
                        case MemberDocumentationParts.Summary:
                            {
                                if (xmlDocumentation != null)
                                    writer.WriteSummary(symbol, xmlDocumentation, headingLevelBase: headingLevelBase);

                                break;
                            }
                        case MemberDocumentationParts.Declaration:
                            {
                                writer.WriteDeclaration(symbol);
                                break;
                            }
                        case MemberDocumentationParts.TypeParameters:
                            {
                                writer.WriteTypeParameters(symbol.GetTypeParameters());
                                break;
                            }
                        case MemberDocumentationParts.Parameters:
                            {
                                writer.WriteParameters(symbol.GetParameters());
                                break;
                            }
                        case MemberDocumentationParts.ReturnValue:
                            {
                                writer.WriteReturnType(symbol, xmlDocumentation);
                                break;
                            }
                        case MemberDocumentationParts.Implements:
                            {
                                writer.WriteImplementedInterfaceMembers(symbol.FindImplementedInterfaceMembers());
                                break;
                            }
                        case MemberDocumentationParts.Attributes:
                            {
                                writer.WriteAttributes(symbol);
                                break;
                            }
                        case MemberDocumentationParts.Exceptions:
                            {
                                if (xmlDocumentation != null)
                                    writer.WriteExceptions(symbol, xmlDocumentation);

                                break;
                            }
                        case MemberDocumentationParts.Examples:
                            {
                                if (xmlDocumentation != null)
                                    writer.WriteExamples(symbol, xmlDocumentation, headingLevelBase: headingLevelBase);

                                break;
                            }
                        case MemberDocumentationParts.Remarks:
                            {
                                if (xmlDocumentation != null)
                                    writer.WriteRemarks(symbol, xmlDocumentation, headingLevelBase: headingLevelBase);

                                break;
                            }
                        case MemberDocumentationParts.SeeAlso:
                            {
                                if (xmlDocumentation != null)
                                    writer.WriteSeeAlso(symbol, xmlDocumentation, headingLevelBase: headingLevelBase);

                                break;
                            }
                    }
                }
            }
        }

        private DocumentationGeneratorResult CreateResult(DocumentationWriter writer, DocumentationFileKind kind, ISymbol symbol = null)
        {
            string fileName = UrlProvider.GetFileName(kind);

            return new DocumentationGeneratorResult(writer?.ToString(), GetPath(), kind);

            string GetPath()
            {
                switch (kind)
                {
                    case DocumentationFileKind.Root:
                    case DocumentationFileKind.Extensions:
                        return fileName;
                    case DocumentationFileKind.Namespace:
                    case DocumentationFileKind.Type:
                    case DocumentationFileKind.Member:
                        return DocumentationUrlProvider.GetUrl(fileName, UrlProvider.GetFolders(symbol), Path.DirectorySeparatorChar);
                    default:
                        throw new ArgumentException("", nameof(kind));
                }
            }
        }
    }
}
