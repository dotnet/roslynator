// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator.Documentation
{
    internal static class SymbolDisplayFormats
    {
        internal static SymbolDisplayFormat Default { get; } = new SymbolDisplayFormat(
             globalNamespaceStyle: DefaultGlobalNamespaceStyle,
             typeQualificationStyle: DefaultTypeQualificationStyle,
             genericsOptions: DefaultGenericsOptions,
             memberOptions: DefaultMemberOptions,
             delegateStyle: DefaultDelegateStyle,
             extensionMethodStyle: DefaultExtensionMethodStyle,
             parameterOptions: DefaultParameterOptions,
             propertyStyle: DefaultPropertyStyle,
             localOptions: DefaultLocalOptions,
             kindOptions: DefaultKindOptions,
             miscellaneousOptions: DefaultMiscellaneousOptions);

        public static SymbolDisplayFormat TypeName { get; } = Default.Update(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
            genericsOptions: SymbolDisplayGenericsOptions.None,
            memberOptions: SymbolDisplayMemberOptions.IncludeContainingType);

        public static SymbolDisplayFormat TypeNameAndTypeParameters { get; } = Default.Update(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            memberOptions: SymbolDisplayMemberOptions.IncludeContainingType);

        public static SymbolDisplayFormat TypeNameAndContainingTypes { get; } = Default.Update(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
            genericsOptions: SymbolDisplayGenericsOptions.None,
            memberOptions: SymbolDisplayMemberOptions.IncludeContainingType);

        public static SymbolDisplayFormat TypeNameAndContainingTypesAndTypeParameters { get; } = Default.Update(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            memberOptions: SymbolDisplayMemberOptions.IncludeContainingType);

        public static SymbolDisplayFormat TypeNameAndContainingTypesAndNamespaces { get; } = Default.Update(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.None,
            memberOptions: SymbolDisplayMemberOptions.IncludeContainingType);

        public static SymbolDisplayFormat TypeNameAndContainingTypesAndNamespacesAndTypeParameters { get; } = Default.Update(
            globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            memberOptions: SymbolDisplayMemberOptions.IncludeContainingType);

        public static SymbolDisplayFormat NamespaceDeclaration { get; } = Default.Update(
             typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
             kindOptions: SymbolDisplayKindOptions.IncludeNamespaceKeyword);

        public static SymbolDisplayFormat FullDeclaration { get; } = Default.Update(
             typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
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

        public static SymbolDisplayFormat SimpleDeclaration { get; } = Default.Update(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            memberOptions: SymbolDisplayMemberOptions.IncludeExplicitInterface
                | SymbolDisplayMemberOptions.IncludeParameters,
            delegateStyle: SymbolDisplayDelegateStyle.NameAndParameters,
            parameterOptions: SymbolDisplayParameterOptions.IncludeType);

        public static SymbolDisplayFormat SortDeclarationList { get; } = Default.Update(
             typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
             genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters
                | SymbolDisplayGenericsOptions.IncludeTypeConstraints
                | SymbolDisplayGenericsOptions.IncludeVariance,
             memberOptions: SymbolDisplayMemberOptions.IncludeParameters
                | SymbolDisplayMemberOptions.IncludeConstantValue
                | SymbolDisplayMemberOptions.IncludeRef,
             delegateStyle: SymbolDisplayDelegateStyle.NameAndParameters,
             extensionMethodStyle: SymbolDisplayExtensionMethodStyle.StaticMethod,
             parameterOptions: SymbolDisplayParameterOptions.IncludeExtensionThis
                | SymbolDisplayParameterOptions.IncludeParamsRefOut
                | SymbolDisplayParameterOptions.IncludeType
                | SymbolDisplayParameterOptions.IncludeName
                | SymbolDisplayParameterOptions.IncludeDefaultValue,
             propertyStyle: SymbolDisplayPropertyStyle.NameOnly,
             miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes
        );

        public static SymbolDisplayFormat ExplicitImplementationFullName { get; } = Default.Update(
             typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
             genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
             memberOptions: SymbolDisplayMemberOptions.IncludeExplicitInterface
                | SymbolDisplayMemberOptions.IncludeContainingType);

        public static SymbolDisplayFormat MemberTitle { get; } = Default.Update(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            memberOptions: SymbolDisplayMemberOptions.IncludeExplicitInterface
                | SymbolDisplayMemberOptions.IncludeParameters
                | SymbolDisplayMemberOptions.IncludeContainingType,
            delegateStyle: SymbolDisplayDelegateStyle.NameAndParameters,
            parameterOptions: SymbolDisplayParameterOptions.IncludeType);

        public static SymbolDisplayFormat OverloadedMemberTitle { get; } = Default.Update(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
            genericsOptions: SymbolDisplayGenericsOptions.None,
            memberOptions: SymbolDisplayMemberOptions.IncludeExplicitInterface
                | SymbolDisplayMemberOptions.IncludeContainingType);

        internal const SymbolDisplayGlobalNamespaceStyle DefaultGlobalNamespaceStyle
            = SymbolDisplayGlobalNamespaceStyle.Omitted;
        //= SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining;
        //= SymbolDisplayGlobalNamespaceStyle.Included;

        internal const SymbolDisplayTypeQualificationStyle DefaultTypeQualificationStyle
            = SymbolDisplayTypeQualificationStyle.NameOnly;
        //= SymbolDisplayTypeQualificationStyle.NameAndContainingTypes;
        //= SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces;

        internal const SymbolDisplayGenericsOptions DefaultGenericsOptions = SymbolDisplayGenericsOptions.None
            | SymbolDisplayGenericsOptions.IncludeTypeParameters;
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
