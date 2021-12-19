// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal static class TypeSymbolDisplayFormats
    {
        public static SymbolDisplayFormat Default { get; } = new();

        public static SymbolDisplayFormat Name { get; } = new(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
            genericsOptions: SymbolDisplayGenericsOptions.None,
            memberOptions: SymbolDisplayMemberOptions.IncludeContainingType);

        public static SymbolDisplayFormat Name_TypeParameters { get; } = new(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            memberOptions: SymbolDisplayMemberOptions.IncludeContainingType);

        public static SymbolDisplayFormat Name_ContainingTypes { get; } = new(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
            genericsOptions: SymbolDisplayGenericsOptions.None,
            memberOptions: SymbolDisplayMemberOptions.IncludeContainingType);

        public static SymbolDisplayFormat Name_ContainingTypes_TypeParameters { get; } = new(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            memberOptions: SymbolDisplayMemberOptions.IncludeContainingType);

        public static SymbolDisplayFormat Name_ContainingTypes_Namespaces { get; } = new(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.None,
            memberOptions: SymbolDisplayMemberOptions.IncludeContainingType);

        public static SymbolDisplayFormat Name_ContainingTypes_Namespaces_TypeParameters { get; } = new(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            memberOptions: SymbolDisplayMemberOptions.IncludeContainingType);

        public static SymbolDisplayFormat Name_ContainingTypes_Namespaces_GlobalNamespace { get; } = new(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.None,
            memberOptions: SymbolDisplayMemberOptions.IncludeContainingType);

        public static SymbolDisplayFormat Name_ContainingTypes_Namespaces_GlobalNamespace_TypeParameters { get; } = new(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            memberOptions: SymbolDisplayMemberOptions.IncludeContainingType);

        public static SymbolDisplayFormat Name_SpecialTypes { get; } = new(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
            genericsOptions: SymbolDisplayGenericsOptions.None,
            memberOptions: SymbolDisplayMemberOptions.IncludeContainingType,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

        public static SymbolDisplayFormat Name_TypeParameters_SpecialTypes { get; } = new(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            memberOptions: SymbolDisplayMemberOptions.IncludeContainingType,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

        public static SymbolDisplayFormat Name_ContainingTypes_SpecialTypes { get; } = new(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
            genericsOptions: SymbolDisplayGenericsOptions.None,
            memberOptions: SymbolDisplayMemberOptions.IncludeContainingType,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

        public static SymbolDisplayFormat Name_ContainingTypes_TypeParameters_SpecialTypes { get; } = new(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            memberOptions: SymbolDisplayMemberOptions.IncludeContainingType,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

        public static SymbolDisplayFormat Name_ContainingTypes_Namespaces_SpecialTypes { get; } = new(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.None,
            memberOptions: SymbolDisplayMemberOptions.IncludeContainingType,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

        public static SymbolDisplayFormat Name_ContainingTypes_Namespaces_TypeParameters_SpecialTypes { get; } = new(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            memberOptions: SymbolDisplayMemberOptions.IncludeContainingType,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

        public static SymbolDisplayFormat Name_ContainingTypes_Namespaces_GlobalNamespace_SpecialTypes { get; } = new(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.None,
            memberOptions: SymbolDisplayMemberOptions.IncludeContainingType,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

        public static SymbolDisplayFormat Name_ContainingTypes_Namespaces_GlobalNamespace_TypeParameters_SpecialTypes { get; } = new(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            memberOptions: SymbolDisplayMemberOptions.IncludeContainingType,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

        public static SymbolDisplayFormat GetFormat(
            bool includeGlobalNamespace = false,
            bool includeNamespaces = true,
            bool includeContainingTypes = true,
            bool includeTypeParameters = true,
            bool useSpecialTypes = false)
        {
            if (includeNamespaces)
            {
                if (!includeContainingTypes)
                    throw new InvalidOperationException();

                if (includeGlobalNamespace)
                {
                    if (includeTypeParameters)
                    {
                        return (useSpecialTypes)
                            ? Name_ContainingTypes_Namespaces_GlobalNamespace_TypeParameters_SpecialTypes
                            : Name_ContainingTypes_Namespaces_GlobalNamespace_TypeParameters;
                    }
                    else
                    {
                        return (useSpecialTypes)
                            ? Name_ContainingTypes_Namespaces_GlobalNamespace_SpecialTypes
                            : Name_ContainingTypes_Namespaces_GlobalNamespace;
                    }
                }
                else if (includeTypeParameters)
                {
                    return (useSpecialTypes)
                        ? Name_ContainingTypes_Namespaces_TypeParameters_SpecialTypes
                        : Name_ContainingTypes_Namespaces_TypeParameters;
                }
                else
                {
                    return (useSpecialTypes)
                        ? Name_ContainingTypes_Namespaces_SpecialTypes
                        : Name_ContainingTypes_Namespaces;
                }
            }
            else if (includeContainingTypes)
            {
                if (includeTypeParameters)
                {
                    return (useSpecialTypes)
                        ? Name_ContainingTypes_TypeParameters_SpecialTypes
                        : Name_ContainingTypes_TypeParameters;
                }
                else
                {
                    return (useSpecialTypes)
                        ? Name_ContainingTypes_SpecialTypes
                        : Name_ContainingTypes;
                }
            }
            else if (includeTypeParameters)
            {
                return (useSpecialTypes)
                    ? Name_TypeParameters_SpecialTypes
                    : Name_TypeParameters;
            }
            else
            {
                return (useSpecialTypes)
                    ? Name_SpecialTypes
                    : Name;
            }
        }
    }
}
