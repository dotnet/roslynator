// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal static class SymbolDisplayFormats
    {
        public static SymbolDisplayFormat Default { get; } = new SymbolDisplayFormat(
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes
                | SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers
                | SymbolDisplayMiscellaneousOptions.RemoveAttributeSuffix);

        public static SymbolDisplayFormat Test { get; } = new SymbolDisplayFormat(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            propertyStyle: SymbolDisplayPropertyStyle.ShowReadWriteDescriptor,
            localOptions: SymbolDisplayLocalOptions.IncludeType,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters
                | SymbolDisplayGenericsOptions.IncludeVariance,
            memberOptions: SymbolDisplayMemberOptions.IncludeParameters
                | SymbolDisplayMemberOptions.IncludeContainingType
                | SymbolDisplayMemberOptions.IncludeType
                | SymbolDisplayMemberOptions.IncludeRef
                | SymbolDisplayMemberOptions.IncludeExplicitInterface,
            kindOptions: SymbolDisplayKindOptions.IncludeMemberKeyword,
            parameterOptions: SymbolDisplayParameterOptions.IncludeOptionalBrackets
                | SymbolDisplayParameterOptions.IncludeDefaultValue
                | SymbolDisplayParameterOptions.IncludeParamsRefOut
                | SymbolDisplayParameterOptions.IncludeExtensionThis
                | SymbolDisplayParameterOptions.IncludeType
                | SymbolDisplayParameterOptions.IncludeName,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers
                | SymbolDisplayMiscellaneousOptions.UseErrorTypeSymbolName);
    }
}
