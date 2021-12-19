// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Roslynator.Rename
{
    internal sealed class LocalSymbolComparer : IComparer<ISymbol>
    {
        public static LocalSymbolComparer Instance { get; } = new();

        public int Compare(ISymbol x, ISymbol y)
        {
            if (object.ReferenceEquals(x, y))
                return 0;

            int rank1 = GetRank(x);
            int rank2 = GetRank(y);

            int diff = rank1.CompareTo(rank2);

            if (diff == 0)
            {
                if (rank1 == 2
                    && rank2 == 2)
                {
                    ISymbol cs = x.ContainingSymbol;

                    while ((cs as IMethodSymbol)?.MethodKind == MethodKind.LocalFunction)
                    {
                        if (SymbolEqualityComparer.Default.Equals(cs, y))
                            return 1;

                        cs = cs.ContainingSymbol;
                    }

                    cs = y.ContainingSymbol;

                    while ((cs as IMethodSymbol)?.MethodKind == MethodKind.LocalFunction)
                    {
                        if (SymbolEqualityComparer.Default.Equals(cs, x))
                            return -1;

                        cs = cs.ContainingSymbol;
                    }
                }

                return x.Locations[0].SourceSpan.Start.CompareTo(y.Locations[0].SourceSpan.Start);
            }

            return diff;

            static int GetRank(ISymbol symbol)
            {
                if (symbol.Kind == SymbolKind.Local)
                    return 0;

                if (symbol.Kind == SymbolKind.Parameter)
                {
                    if ((symbol.ContainingSymbol as IMethodSymbol)?.MethodKind == MethodKind.AnonymousFunction)
                        return 0;

                    return 1;
                }

                if (symbol.Kind == SymbolKind.TypeParameter)
                {
                    Debug.Assert((symbol.ContainingSymbol as IMethodSymbol)?.MethodKind == MethodKind.LocalFunction);

                    return 1;
                }

                return 2;
            }
        }
    }
}
