// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal static class SymbolDisplay
    {
        public static string GetString(ISymbol symbol)
        {
            return symbol.ToDisplayString(Format);
        }

        public static string GetMinimalString(ISymbol symbol, SemanticModel semanticModel, int position)
        {
            return symbol.ToMinimalDisplayString(semanticModel, position, Format);
        }

        public static SymbolDisplayFormat Format { get; } = new SymbolDisplayFormat(
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypes,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes
                | SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers);
    }
}
