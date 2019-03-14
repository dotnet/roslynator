// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal static class SymbolDefinitionDisplayFormats
    {
        public static SymbolDisplayFormat Default { get; } = new SymbolDisplayFormat(
            globalNamespaceStyle: DefaultGlobalNamespaceStyle,
            typeQualificationStyle: DefaultTypeQualificationStyle,
            genericsOptions: DefaultGenericsOptions | SymbolDisplayGenericsOptions.IncludeTypeParameters,
            memberOptions: DefaultMemberOptions,
            delegateStyle: DefaultDelegateStyle,
            extensionMethodStyle: DefaultExtensionMethodStyle,
            parameterOptions: DefaultParameterOptions,
            propertyStyle: DefaultPropertyStyle,
            localOptions: DefaultLocalOptions,
            kindOptions: DefaultKindOptions,
            miscellaneousOptions: DefaultMiscellaneousOptions | SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

        public static SymbolDisplayFormat TypeNameAndContainingTypes { get; } = Default.Update(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
            genericsOptions: SymbolDisplayGenericsOptions.None,
            memberOptions: SymbolDisplayMemberOptions.IncludeContainingType);

        public static SymbolDisplayFormat TypeNameAndContainingTypesAndTypeParameters { get; } = TypeNameAndContainingTypes.Update(
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters);

        public static SymbolDisplayFormat TypeNameAndContainingTypesAndNamespaces { get; } = Default.Update(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.None,
            memberOptions: SymbolDisplayMemberOptions.IncludeContainingType);

        public static SymbolDisplayFormat TypeNameAndContainingTypesAndNamespacesAndGlobalNamespace { get; } = TypeNameAndContainingTypesAndNamespaces.Update(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included);

        public static SymbolDisplayFormat TypeNameAndContainingTypesAndNamespacesAndTypeParameters { get; } = TypeNameAndContainingTypesAndNamespaces.Update(
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters);

        public static readonly SymbolDisplayFormat FullName = new SymbolDisplayFormat(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            memberOptions: SymbolDisplayMemberOptions.IncludeContainingType);

        public static SymbolDisplayFormat FullDefinition_NameOnly { get; } = Default.Update(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters
                | SymbolDisplayGenericsOptions.IncludeTypeConstraints
                | SymbolDisplayGenericsOptions.IncludeVariance,
            memberOptions: SymbolDisplayMemberOptions.IncludeType
                | SymbolDisplayMemberOptions.IncludeModifiers
                | SymbolDisplayMemberOptions.IncludeAccessibility
                | SymbolDisplayMemberOptions.IncludeExplicitInterface
                | SymbolDisplayMemberOptions.IncludeParameters
                | SymbolDisplayMemberOptions.IncludeConstantValue
                | SymbolDisplayMemberOptions.IncludeRef,
            delegateStyle: SymbolDisplayDelegateStyle.NameAndSignature,
            parameterOptions: SymbolDisplayParameterOptions.IncludeExtensionThis
                | SymbolDisplayParameterOptions.IncludeParamsRefOut
                | SymbolDisplayParameterOptions.IncludeType
                | SymbolDisplayParameterOptions.IncludeName
                | SymbolDisplayParameterOptions.IncludeDefaultValue,
            propertyStyle: SymbolDisplayPropertyStyle.ShowReadWriteDescriptor,
            kindOptions: SymbolDisplayKindOptions.IncludeNamespaceKeyword
                | SymbolDisplayKindOptions.IncludeTypeKeyword
                | SymbolDisplayKindOptions.IncludeMemberKeyword,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes
                | SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers
        );

        public static SymbolDisplayFormat FullDefinition_NameAndContainingTypes { get; } = FullDefinition_NameOnly.Update(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes);

        public static SymbolDisplayFormat FullDefinition_NameAndContainingTypesAndNamespaces { get; } = FullDefinition_NameOnly.Update(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

        internal const SymbolDisplayGlobalNamespaceStyle DefaultGlobalNamespaceStyle
            = SymbolDisplayGlobalNamespaceStyle.Omitted;
        //= SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining;
        //= SymbolDisplayGlobalNamespaceStyle.Included;

        internal const SymbolDisplayTypeQualificationStyle DefaultTypeQualificationStyle
            = SymbolDisplayTypeQualificationStyle.NameOnly;
        //= SymbolDisplayTypeQualificationStyle.NameAndContainingTypes;
        //= SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces;

        internal const SymbolDisplayGenericsOptions DefaultGenericsOptions = SymbolDisplayGenericsOptions.None;
        //| SymbolDisplayGenericsOptions.IncludeTypeParameters
        //| SymbolDisplayGenericsOptions.IncludeTypeConstraints
        //| SymbolDisplayGenericsOptions.IncludeVariance

        internal const SymbolDisplayMemberOptions DefaultMemberOptions = SymbolDisplayMemberOptions.None;
        //| SymbolDisplayMemberOptions.IncludeType
        //| SymbolDisplayMemberOptions.IncludeModifiers
        //| SymbolDisplayMemberOptions.IncludeAccessibility
        //| SymbolDisplayMemberOptions.IncludeExplicitInterface
        //| SymbolDisplayMemberOptions.IncludeParameters
        //| SymbolDisplayMemberOptions.IncludeContainingType
        //| SymbolDisplayMemberOptions.IncludeConstantValue
        //| SymbolDisplayMemberOptions.IncludeRef

        internal const SymbolDisplayDelegateStyle DefaultDelegateStyle
            = SymbolDisplayDelegateStyle.NameOnly;
        //= SymbolDisplayDelegateStyle.NameAndParameters;
        //= SymbolDisplayDelegateStyle.NameAndSignature;

        internal const SymbolDisplayExtensionMethodStyle DefaultExtensionMethodStyle
            = SymbolDisplayExtensionMethodStyle.Default;
        //= SymbolDisplayExtensionMethodStyle.InstanceMethod;
        //= SymbolDisplayExtensionMethodStyle.StaticMethod;

        internal const SymbolDisplayParameterOptions DefaultParameterOptions = SymbolDisplayParameterOptions.None;
        //| SymbolDisplayParameterOptions.IncludeExtensionThis
        //| SymbolDisplayParameterOptions.IncludeParamsRefOut
        //| SymbolDisplayParameterOptions.IncludeType
        //| SymbolDisplayParameterOptions.IncludeName
        //| SymbolDisplayParameterOptions.IncludeDefaultValue
        //| SymbolDisplayParameterOptions.IncludeOptionalBrackets

        internal const SymbolDisplayPropertyStyle DefaultPropertyStyle
            = SymbolDisplayPropertyStyle.NameOnly;
        //= SymbolDisplayPropertyStyle.ShowReadWriteDescriptor;

        internal const SymbolDisplayLocalOptions DefaultLocalOptions = SymbolDisplayLocalOptions.None;
        //| SymbolDisplayLocalOptions.IncludeType
        //| SymbolDisplayLocalOptions.IncludeConstantValue
        //| SymbolDisplayLocalOptions.IncludeRef

        internal const SymbolDisplayKindOptions DefaultKindOptions = SymbolDisplayKindOptions.None;
        //| SymbolDisplayKindOptions.IncludeNamespaceKeyword
        //| SymbolDisplayKindOptions.IncludeTypeKeyword
        //| SymbolDisplayKindOptions.IncludeMemberKeyword

        internal const SymbolDisplayMiscellaneousOptions DefaultMiscellaneousOptions = SymbolDisplayMiscellaneousOptions.None;
        //| SymbolDisplayMiscellaneousOptions.UseSpecialTypes
        //| SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers
        //| SymbolDisplayMiscellaneousOptions.UseAsterisksInMultiDimensionalArrays
        //| SymbolDisplayMiscellaneousOptions.UseErrorTypeSymbolName
        //| SymbolDisplayMiscellaneousOptions.RemoveAttributeSuffix
        //| SymbolDisplayMiscellaneousOptions.ExpandNullable
    }
}
