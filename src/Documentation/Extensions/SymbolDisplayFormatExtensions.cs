// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator.Documentation
{
    internal static class SymbolDisplayFormatExtensions
    {
        public static SymbolDisplayFormat Update(
            this SymbolDisplayFormat format,
            SymbolDisplayGlobalNamespaceStyle? globalNamespaceStyle = null,
            SymbolDisplayTypeQualificationStyle? typeQualificationStyle = null,
            SymbolDisplayGenericsOptions? genericsOptions = null,
            SymbolDisplayMemberOptions? memberOptions = null,
            SymbolDisplayDelegateStyle? delegateStyle = null,
            SymbolDisplayExtensionMethodStyle? extensionMethodStyle = null,
            SymbolDisplayParameterOptions? parameterOptions = null,
            SymbolDisplayPropertyStyle? propertyStyle = null,
            SymbolDisplayLocalOptions? localOptions = null,
            SymbolDisplayKindOptions? kindOptions = null,
            SymbolDisplayMiscellaneousOptions? miscellaneousOptions = null)
        {
            return new SymbolDisplayFormat(
                globalNamespaceStyle: globalNamespaceStyle ?? format.GlobalNamespaceStyle,
                typeQualificationStyle: typeQualificationStyle ?? format.TypeQualificationStyle,
                genericsOptions: genericsOptions ?? format.GenericsOptions,
                memberOptions: memberOptions ?? format.MemberOptions,
                delegateStyle: delegateStyle ?? format.DelegateStyle,
                extensionMethodStyle: extensionMethodStyle ?? format.ExtensionMethodStyle,
                parameterOptions: parameterOptions ?? format.ParameterOptions,
                propertyStyle: propertyStyle ?? format.PropertyStyle,
                localOptions: localOptions ?? format.LocalOptions,
                kindOptions: kindOptions ?? format.KindOptions,
                miscellaneousOptions: miscellaneousOptions ?? format.MiscellaneousOptions
            );
        }
    }
}
