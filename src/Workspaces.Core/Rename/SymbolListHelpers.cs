// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator.Rename
{
    internal static class SymbolListHelpers
    {
        public static List<ISymbol> SortTypeSymbols(IEnumerable<ISymbol> symbols)
        {
            var results = new List<ISymbol>();
            var typeSymbols = new List<INamedTypeSymbol>();
            var baseTypeSymbols = new List<INamedTypeSymbol>();

            foreach (ISymbol symbol in symbols)
            {
                if (symbol is INamedTypeSymbol typeSymbol)
                {
                    if (typeSymbol.ContainingType == null)
                        baseTypeSymbols.Add(typeSymbol);

                    typeSymbols.Add(typeSymbol);
                }
            }
#if DEBUG
            baseTypeSymbols.Sort((x, y) => StringComparer.Ordinal.Compare(x.ToDisplayString(SymbolDisplayFormats.Test), y.ToDisplayString(SymbolDisplayFormats.Test)));
#endif
            INamedTypeSymbol baseType = null;

            for (int i = 0; i < baseTypeSymbols.Count; i++)
            {
                baseType = baseTypeSymbols[i];
                AnalyzeSymbol();
            }

            void AnalyzeSymbol()
            {
                typeSymbols.Remove(baseType);

                results.Add(baseType);

                IMethodSymbol delegateMethodSymbol = baseType.DelegateInvokeMethod;

                if (delegateMethodSymbol != null)
                    results.AddRange(delegateMethodSymbol.Parameters);

                results.AddRange(baseType.TypeParameters);

                foreach (INamedTypeSymbol containedType in typeSymbols
                    .Where(f => SymbolEqualityComparer.Default.Equals(f.ContainingType, baseType))
#if DEBUG
                    .OrderBy(f => f.ToDisplayString(SymbolDisplayFormats.Test))
#endif
                    .ToList())
                {
                    baseType = containedType;

                    AnalyzeSymbol();
                }
            }

            return results;
        }

        public static List<ISymbol> SortAndFilterMemberSymbols(IEnumerable<ISymbol> symbols)
        {
            var results = new List<ISymbol>();

            foreach (ISymbol symbol in symbols
                .Where(s =>
                {
                    return s.OverriddenSymbol() == null
                        && !s.ImplementsInterfaceMember(allInterfaces: true);
                })
#if DEBUG
                .OrderBy(s => s.ToDisplayString(SymbolDisplayFormats.Test))
#endif
                )
            {
                if (symbol is IPropertySymbol propertySymbol)
                {
                    if (!propertySymbol.IsIndexer)
                        results.Add(symbol);

                    results.AddRange(propertySymbol.Parameters);
                }
                else if (symbol is IMethodSymbol methodSymbol)
                {
                    if (!methodSymbol.MethodKind.Is(MethodKind.Constructor, MethodKind.UserDefinedOperator, MethodKind.Conversion))
                        results.Add(symbol);

                    results.AddRange(methodSymbol.TypeParameters);
                    results.AddRange(methodSymbol.Parameters);
                }
                else
                {
                    results.Add(symbol);
                }
            }

            return results;
        }
    }
}
