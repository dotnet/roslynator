// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator
{
    public static class SymbolDisplay
    {
        public static string GetString(ITypeSymbol typeSymbol)
        {
            return typeSymbol.ToDisplayString(Format);
        }

        public static string GetMinimalString(ITypeSymbol typeSymbol, SemanticModel semanticModel, int position)
        {
            return typeSymbol.ToMinimalDisplayString(semanticModel, position, Format);
        }

        public static SymbolDisplayFormat Format { get; } = new SymbolDisplayFormat(
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes
                | SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers);
    }
}
