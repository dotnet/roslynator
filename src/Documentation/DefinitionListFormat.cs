// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator.Documentation
{
    internal class DefinitionListFormat
    {
        public DefinitionListFormat(
            SymbolDefinitionListLayout layout = DefaultValues.Layout,
            SymbolDefinitionPartFilter parts = DefaultValues.Parts,
            WrapListOptions wrapListOptions = DefaultValues.WrapListOptions,
            bool groupByAssembly = false,
            bool emptyLineBetweenMembers = DefaultValues.EmptyLineBetweenMembers,
            bool emptyLineBetweenMemberGroups = DefaultValues.EmptyLineBetweenMemberGroups,
            bool omitIEnumerable = DefaultValues.OmitIEnumerable,
            bool allowDefaultLiteral = DefaultValues.AllowDefaultLiteral,
            string indentChars = DefaultValues.IndentChars)
        {
            Layout = layout;
            Parts = parts;
            WrapListOptions = wrapListOptions;
            GroupByAssembly = groupByAssembly;
            EmptyLineBetweenMembers = emptyLineBetweenMembers;
            EmptyLineBetweenMemberGroups = emptyLineBetweenMemberGroups;
            OmitIEnumerable = omitIEnumerable;
            AllowDefaultLiteral = allowDefaultLiteral;
            IndentChars = indentChars;
        }

        public static DefinitionListFormat Default { get; } = new();

        public SymbolDefinitionListLayout Layout { get; }

        public SymbolDefinitionPartFilter Parts { get; }

        public WrapListOptions WrapListOptions { get; }

        public bool GroupByAssembly { get; }

        public bool EmptyLineBetweenMembers { get; }

        public bool EmptyLineBetweenMemberGroups { get; }

        public bool OmitIEnumerable { get; }

        public bool AllowDefaultLiteral { get; }

        public string IndentChars { get; }

        public bool Includes(SymbolDefinitionPartFilter parts)
        {
            return (Parts & parts) == parts;
        }

        public bool Includes(WrapListOptions wrapListOptions)
        {
            return (WrapListOptions & wrapListOptions) == wrapListOptions;
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

            SymbolDisplayMiscellaneousOptions miscellaneousOptions = format.MiscellaneousOptions;

            if (AllowDefaultLiteral)
            {
                miscellaneousOptions |= SymbolDisplayMiscellaneousOptions.AllowDefaultLiteral;
            }
            else
            {
                miscellaneousOptions &= ~SymbolDisplayMiscellaneousOptions.AllowDefaultLiteral;
            }

            return format.Update(
                genericsOptions: genericsOptions,
                memberOptions: memberOptions,
                parameterOptions: parameterOptions,
                miscellaneousOptions: miscellaneousOptions);
        }

        internal SymbolDisplayFormat GetFormat()
        {
            return TypeSymbolDisplayFormats.GetFormat(includeNamespaces: Includes(SymbolDefinitionPartFilter.ContainingNamespace));
        }

        internal static class DefaultValues
        {
            public const SymbolDefinitionListLayout Layout = SymbolDefinitionListLayout.NamespaceList;
            public const SymbolDefinitionPartFilter Parts = SymbolDefinitionPartFilter.All;
            public const WrapListOptions WrapListOptions = Documentation.WrapListOptions.None;
            public const Visibility Visibility = Roslynator.Visibility.Private;
            public const SymbolGroupFilter SymbolGroupFilter = Roslynator.SymbolGroupFilter.TypeOrMember;
            public const string IndentChars = "  ";
            public const bool EmptyLineBetweenMemberGroups = true;
            public const bool EmptyLineBetweenMembers = false;
            public const bool OmitIEnumerable = true;
            public const bool AllowDefaultLiteral = true;
        }
    }
}
