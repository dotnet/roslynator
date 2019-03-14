// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator.Documentation
{
    internal class DefinitionListFormat
    {
        public DefinitionListFormat(
            SymbolDefinitionListLayout layout = DefaultValues.Layout,
            SymbolDefinitionPartFilter parts = DefaultValues.Parts,
            SymbolDefinitionFormatOptions formatOptions = DefaultValues.FormatOptions,
            bool groupByAssembly = false,
            bool emptyLineBetweenMembers = DefaultValues.EmptyLineBetweenMembers,
            bool emptyLineBetweenMemberGroups = DefaultValues.EmptyLineBetweenMemberGroups,
            bool omitIEnumerable = DefaultValues.OmitIEnumerable,
            bool preferDefaultLiteral = DefaultValues.PreferDefaultLiteral,
            string indentChars = DefaultValues.IndentChars)
        {
            Layout = layout;
            Parts = parts;
            FormatOptions = formatOptions;
            GroupByAssembly = groupByAssembly;
            EmptyLineBetweenMembers = emptyLineBetweenMembers;
            EmptyLineBetweenMemberGroups = emptyLineBetweenMemberGroups;
            OmitIEnumerable = omitIEnumerable;
            PreferDefaultLiteral = preferDefaultLiteral;
            IndentChars = indentChars;
        }

        public static DefinitionListFormat Default { get; } = new DefinitionListFormat();

        public SymbolDefinitionListLayout Layout { get; }

        public SymbolDefinitionPartFilter Parts { get; }

        public SymbolDefinitionFormatOptions FormatOptions { get; }

        public bool GroupByAssembly { get; }

        public bool EmptyLineBetweenMembers { get; }

        public bool EmptyLineBetweenMemberGroups { get; }

        public bool OmitIEnumerable { get; }

        public bool PreferDefaultLiteral { get; }

        public string IndentChars { get; }

        public bool Includes(SymbolDefinitionPartFilter parts)
        {
            return (Parts & parts) == parts;
        }

        public bool Includes(SymbolDefinitionFormatOptions formatOptions)
        {
            return (FormatOptions & formatOptions) == formatOptions;
        }

        internal SymbolDisplayFormat Update(SymbolDisplayFormat format)
        {
            SymbolDisplayGenericsOptions genericsOptions = format.GenericsOptions;

            if (!Includes(SymbolDefinitionPartFilter.Constraints))
                genericsOptions &= ~SymbolDisplayGenericsOptions.IncludeTypeConstraints;

            SymbolDisplayMemberOptions memberOptions = format.MemberOptions;

            if (!Includes(SymbolDefinitionPartFilter.Modifiers))
                memberOptions &= ~SymbolDisplayMemberOptions.IncludeModifiers;

            if (!Includes(SymbolDefinitionPartFilter.Accessibility))
                memberOptions &= ~SymbolDisplayMemberOptions.IncludeAccessibility;

            SymbolDisplayParameterOptions parameterOptions = format.ParameterOptions;

            if (!Includes(SymbolDefinitionPartFilter.ParameterName))
                parameterOptions &= ~SymbolDisplayParameterOptions.IncludeName;

            if (!Includes(SymbolDefinitionPartFilter.ParameterDefaultValue))
                parameterOptions &= ~SymbolDisplayParameterOptions.IncludeDefaultValue;

            return format.Update(
                genericsOptions: genericsOptions,
                memberOptions: memberOptions,
                parameterOptions: parameterOptions);
        }

        internal SymbolDisplayFormat GetFormat()
        {
            return (Includes(SymbolDefinitionPartFilter.ContainingNamespace))
                ? SymbolDefinitionDisplayFormats.TypeNameAndContainingTypesAndNamespacesAndTypeParameters
                : SymbolDefinitionDisplayFormats.TypeNameAndContainingTypesAndTypeParameters;
        }

        internal static class DefaultValues
        {
            public const SymbolDefinitionListLayout Layout = SymbolDefinitionListLayout.NamespaceList;
            public const SymbolDefinitionPartFilter Parts = SymbolDefinitionPartFilter.All;
            public const SymbolDefinitionFormatOptions FormatOptions = SymbolDefinitionFormatOptions.None;
            public const Visibility Visibility = Roslynator.Visibility.Private;
            public const SymbolGroupFilter SymbolGroupFilter = Roslynator.SymbolGroupFilter.TypeOrMember;
            public const string IndentChars = "  ";
            public const bool EmptyLineBetweenMemberGroups = true;
            public const bool EmptyLineBetweenMembers = false;
            public const bool OmitIEnumerable = true;
            public const bool PreferDefaultLiteral = true;
        }
    }
}
