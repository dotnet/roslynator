// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator.Documentation
{
    public class DeclarationListOptions
    {
        private readonly ImmutableArray<MetadataName> _ignoredMetadataNames;

        public DeclarationListOptions(
            IEnumerable<string> ignoredNames = null,
            bool indent = DefaultValues.Indent,
            string indentChars = DefaultValues.IndentChars,
            bool nestNamespaces = DefaultValues.NestNamespaces,
            bool newLineBeforeOpenBrace = DefaultValues.NewLineBeforeOpenBrace,
            bool emptyLineBetweenMembers = DefaultValues.EmptyLineBetweenMembers,
            bool formatBaseList = DefaultValues.FormatBaseList,
            bool formatConstraints = DefaultValues.FormatConstraints,
            bool formatParameters = DefaultValues.FormatParameters,
            bool splitAttributes = DefaultValues.SplitAttributes,
            bool includeAttributeArguments = DefaultValues.IncludeAttributeArguments,
            bool omitIEnumerable = DefaultValues.OmitIEnumerable,
            bool useDefaultLiteral = DefaultValues.UseDefaultLiteral,
            bool fullyQualifiedNames = DefaultValues.FullyQualifiedNames,
            DocumentationDepth depth = DefaultValues.Depth,
            DeclarationListParts ignoredParts = DeclarationListParts.None)
        {
            _ignoredMetadataNames = ignoredNames?.Select(name => MetadataName.Parse(name)).ToImmutableArray() ?? default;

            IgnoredNames = ignoredNames?.ToImmutableArray() ?? ImmutableArray<string>.Empty;
            Indent = indent;
            IndentChars = indentChars;
            NestNamespaces = nestNamespaces;
            NewLineBeforeOpenBrace = newLineBeforeOpenBrace;
            EmptyLineBetweenMembers = emptyLineBetweenMembers;
            FormatBaseList = formatBaseList;
            FormatConstraints = formatConstraints;
            FormatParameters = formatParameters;
            SplitAttributes = splitAttributes;
            IncludeAttributeArguments = includeAttributeArguments;
            OmitIEnumerable = omitIEnumerable;
            UseDefaultLiteral = useDefaultLiteral;
            FullyQualifiedNames = fullyQualifiedNames;
            Depth = depth;
            IgnoredParts = ignoredParts;
        }

        public static DeclarationListOptions Default { get; } = new DeclarationListOptions();

        public ImmutableArray<string> IgnoredNames { get; }

        public DeclarationListParts IgnoredParts { get; }

        public DocumentationDepth Depth { get; }

        public bool Indent { get; }

        public string IndentChars { get; }

        public bool NestNamespaces { get; }

        public bool NewLineBeforeOpenBrace { get; }

        public bool EmptyLineBetweenMembers { get; }

        public bool FormatBaseList { get; }

        public bool FormatConstraints { get; }

        public bool FormatParameters { get; }

        public bool SplitAttributes { get; }

        public bool IncludeAttributeArguments { get; }

        public bool OmitIEnumerable { get; }

        public bool UseDefaultLiteral { get; }

        public bool FullyQualifiedNames { get; }

        internal bool ShouldBeIgnored(INamedTypeSymbol typeSymbol)
        {
            foreach (MetadataName metadataName in _ignoredMetadataNames)
            {
                if (typeSymbol.HasMetadataName(metadataName))
                    return true;

                if (!metadataName.ContainingTypes.Any())
                {
                    INamespaceSymbol n = typeSymbol.ContainingNamespace;

                    while (n != null)
                    {
                        if (n.HasMetadataName(metadataName))
                            return true;

                        n = n.ContainingNamespace;
                    }
                }
            }

            return false;
        }

        internal static class DefaultValues
        {
            public const DocumentationDepth Depth = DocumentationOptions.DefaultValues.Depth;
            public const bool EmptyLineBetweenMembers = false;
            public const bool FormatBaseList = false;
            public const bool FormatConstraints = false;
            public const bool FormatParameters = false;
            public const bool IncludeAttributeArguments = true;
            public const bool Indent = true;
            public const string IndentChars = "    ";
            public const bool NestNamespaces = false;
            public const bool NewLineBeforeOpenBrace = true;
            public const bool OmitIEnumerable = true;
            public const bool SplitAttributes = true;
            public const bool UseDefaultLiteral = true;
            public const bool FullyQualifiedNames = false;
        }
    }
}
