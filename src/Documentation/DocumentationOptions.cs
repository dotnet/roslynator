// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator.Documentation;

public class DocumentationOptions
{
    private int _maxDerivedTypes = DefaultValues.MaxDerivedTypes;
    private CommonDocumentationParts _ignoredCommonParts = CommonDocumentationParts.None;

    public List<MetadataName> IgnoredNames { get; } = new();

    public string RootFileHeading { get; set; }

    public string PreferredCultureName { get; set; }

    public string RootDirectoryUrl { get; set; }

    public int MaxDerivedTypes
    {
        get { return _maxDerivedTypes; }
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), value, "Maximum number of derived items must be greater than or equal to 0.");

            _maxDerivedTypes = value;
        }
    }

    public bool IncludeSystemNamespace { get; set; } = DefaultValues.IncludeSystemNamespace;

    public bool PlaceSystemNamespaceFirst { get; set; } = DefaultValues.PlaceSystemNamespaceFirst;

    public bool WrapDeclarationBaseTypes { get; set; } = DefaultValues.WrapDeclarationBaseTypes;

    public bool WrapDeclarationConstraints { get; set; } = DefaultValues.WrapDeclarationConstraints;

    public bool MarkObsolete { get; set; } = DefaultValues.MarkObsolete;

    public bool IncludeMemberInheritedFrom { get; set; } = DefaultValues.IncludeMemberInheritedFrom;

    public bool IncludeMemberOverrides { get; set; } = DefaultValues.IncludeMemberOverrides;

    public bool IncludeMemberImplements { get; set; } = DefaultValues.IncludeMemberImplements;

    public bool IncludeMemberConstantValue { get; set; } = DefaultValues.IncludeMemberConstantValue;

    public bool IncludeInheritedInterfaceMembers { get; set; } = DefaultValues.IncludeInheritedInterfaceMembers;

    public bool IncludeAllDerivedTypes { get; set; } = DefaultValues.IncludeAllDerivedTypes;

    public bool IncludeAttributeArguments { get; set; } = DefaultValues.IncludeAttributeArguments;

    public bool IncludeInheritedAttributes { get; set; } = DefaultValues.IncludeInheritedAttributes;

    public bool OmitIEnumerable { get; set; } = DefaultValues.OmitIEnumerable;

    public DocumentationDepth Depth { get; set; } = DefaultValues.Depth;

    public InheritanceStyle InheritanceStyle { get; set; } = DefaultValues.InheritanceStyle;

    public RootDocumentationParts IgnoredRootParts { get; set; } = RootDocumentationParts.None;

    public NamespaceDocumentationParts IgnoredNamespaceParts { get; set; } = NamespaceDocumentationParts.None;

    public TypeDocumentationParts IgnoredTypeParts { get; set; } = TypeDocumentationParts.None;

    public MemberDocumentationParts IgnoredMemberParts { get; set; } = MemberDocumentationParts.None;

    public CommonDocumentationParts IgnoredCommonParts
    {
        get { return _ignoredCommonParts; }
        set
        {
            if ((value & CommonDocumentationParts.Content) != 0)
            {
                IgnoredRootParts |= RootDocumentationParts.Content;
                IgnoredNamespaceParts |= NamespaceDocumentationParts.Content;
                IgnoredTypeParts |= TypeDocumentationParts.Content;
                IgnoredMemberParts |= MemberDocumentationParts.Content;
            }

            _ignoredCommonParts = value;
        }
    }

    public SymbolTitleParts IgnoredTitleParts { get; set; } = SymbolTitleParts.None;

    public IncludeContainingNamespaceFilter IncludeContainingNamespaceFilter { get; set; } = IncludeContainingNamespaceFilter.None;

    public FilesLayout FilesLayout { get; set; } = FilesLayout.Hierarchical;

    public bool ScrollToContent { get; set; } = DefaultValues.ScrollToContent;

    internal FileSystemFilter FileSystemFilter { get; set; }

    internal bool IncludeContainingNamespace(IncludeContainingNamespaceFilter filter)
    {
        return (IncludeContainingNamespaceFilter & filter) == filter;
    }

    internal bool ShouldBeIgnored(INamedTypeSymbol typeSymbol)
    {
        foreach (MetadataName ignoredName in IgnoredNames)
        {
            if (typeSymbol.HasMetadataName(ignoredName))
                return true;

            if (!ignoredName.ContainingTypes.Any())
            {
                INamespaceSymbol n = typeSymbol.ContainingNamespace;

                while (n is not null)
                {
                    if (n.HasMetadataName(ignoredName))
                        return true;

                    n = n.ContainingNamespace;
                }
            }

            if (ShouldBeIgnoredByLocation(typeSymbol))
                return true;
        }

        return false;
    }

    internal bool ShouldBeIgnoredByLocation(ISymbol symbol)
    {
        return FileSystemFilter?.IsMatch(symbol) == false;
    }

    internal static class DefaultValues
    {
        public const DocumentationDepth Depth = DocumentationDepth.Member;
        public const bool WrapDeclarationBaseTypes = true;
        public const bool WrapDeclarationConstraints = true;
        public const InheritanceStyle InheritanceStyle = Documentation.InheritanceStyle.Horizontal;
        public const bool IncludeAllDerivedTypes = false;
        public const bool IncludeAttributeArguments = true;
        public const bool IncludeInheritedAttributes = true;
        public const bool IncludeInheritedInterfaceMembers = false;
        public const bool IncludeMemberConstantValue = true;
        public const bool IncludeMemberImplements = true;
        public const bool IncludeMemberInheritedFrom = true;
        public const bool IncludeMemberOverrides = true;
        public const bool MarkObsolete = true;
        public const int MaxDerivedTypes = 5;
        public const bool OmitIEnumerable = true;
        public const bool PlaceSystemNamespaceFirst = true;
        public const bool IncludeSystemNamespace = false;
        public const bool ScrollToContent = false;
        public const RootDocumentationParts IgnoredRootParts = RootDocumentationParts.None;
        public const NamespaceDocumentationParts IgnoredNamespaceParts = NamespaceDocumentationParts.None;
        public const TypeDocumentationParts IgnoredTypeParts = TypeDocumentationParts.None;
        public const MemberDocumentationParts IgnoredMemberParts = MemberDocumentationParts.None;
        public const CommonDocumentationParts IgnoredCommonParts = CommonDocumentationParts.None;
        public const SymbolTitleParts IgnoredTitleParts = SymbolTitleParts.None;
        public const IncludeContainingNamespaceFilter IncludeContainingNamespaceFilter = Roslynator.Documentation.IncludeContainingNamespaceFilter.None;
    }
}
