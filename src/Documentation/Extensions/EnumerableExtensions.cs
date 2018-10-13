// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator.Documentation
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<INamedTypeSymbol> Sort(
            this IEnumerable<INamedTypeSymbol> typeSymbols,
            bool systemNamespaceFirst,
            bool includeContainingNamespace = true,
            bool includeContainingTypes = true)
        {
            return Sort(
                items: typeSymbols,
                selector: f => f,
                systemNamespaceFirst: systemNamespaceFirst,
                includeContainingNamespace: includeContainingNamespace,
                includeContainingTypes: includeContainingTypes);
        }

        public static IEnumerable<T> Sort<T>(
            this IEnumerable<T> items,
            Func<T, INamedTypeSymbol> selector,
            bool systemNamespaceFirst,
            bool includeContainingNamespace = true,
            bool includeContainingTypes = true)
        {
            return Sort(
                items: items,
                selector: selector,
                format: (includeContainingTypes) ? SymbolDisplayFormats.TypeNameAndContainingTypesAndTypeParameters : SymbolDisplayFormats.TypeNameAndTypeParameters,
                systemNamespaceFirst: includeContainingNamespace,
                includeContainingNamespace: systemNamespaceFirst);
        }

        public static IEnumerable<ISymbol> Sort(
            this IEnumerable<ISymbol> items,
            SymbolDisplayFormat format,
            bool systemNamespaceFirst,
            bool includeContainingNamespace = true,
            SymbolDisplayAdditionalMemberOptions additionalOptions = SymbolDisplayAdditionalMemberOptions.None)
        {
            return Sort(
                items: items,
                selector: f => f,
                format: format,
                systemNamespaceFirst: systemNamespaceFirst,
                includeContainingNamespace: includeContainingNamespace,
                additionalOptions: additionalOptions);
        }

        public static IEnumerable<T> Sort<T>(
            this IEnumerable<T> items,
            Func<T, ISymbol> selector,
            SymbolDisplayFormat format,
            bool systemNamespaceFirst,
            bool includeContainingNamespace = true,
            SymbolDisplayAdditionalMemberOptions additionalOptions = SymbolDisplayAdditionalMemberOptions.None)
        {
            if (includeContainingNamespace)
            {
                return items
                    .OrderBy(f => selector(f).ContainingNamespace, NamespaceSymbolComparer.GetInstance(systemNamespaceFirst))
                    .ThenBy(f => selector(f).ToDisplayString(format, additionalOptions));
            }
            else
            {
                return items
                    .OrderBy(f => selector(f).ToDisplayString(format, additionalOptions));
            }
        }
    }
}
