// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal sealed class NamedTypeSymbolDefinitionComparer : IComparer<INamedTypeSymbol>
    {
        internal NamedTypeSymbolDefinitionComparer(SymbolDefinitionComparer symbolComparer)
        {
            SymbolComparer = symbolComparer;
        }

        public SymbolDefinitionComparer SymbolComparer { get; }

        public int Compare(INamedTypeSymbol x, INamedTypeSymbol y)
        {
            if (object.ReferenceEquals(x, y))
                return 0;

            if (x == null)
                return -1;

            if (y == null)
                return 1;

            int diff;

            if ((SymbolComparer.Options & SymbolDefinitionSortOptions.OmitContainingNamespace) == 0)
            {
                diff = SymbolComparer.CompareContainingNamespace(x, y);

                if (diff != 0)
                    return diff;
            }

            diff = GetRank(x).CompareTo(GetRank(y));

            if (diff != 0)
                return diff;

            int count1 = CountContainingTypes(x);
            int count2 = CountContainingTypes(y);

            while (true)
            {
                INamedTypeSymbol containingType1 = GetContainingType(x, count1);
                INamedTypeSymbol containingType2 = GetContainingType(y, count2);

                diff = SymbolDefinitionComparer.CompareName(containingType1, containingType2);

                if (diff != 0)
                    return diff;

                diff = containingType1.TypeParameters.Length.CompareTo(containingType2.TypeParameters.Length);

                if (diff != 0)
                    return diff;

                if (count1 == 0)
                {
                    if (count2 == 0)
                    {
                        return SymbolComparer.CompareContainingNamespace(x, y);
                    }
                    else
                    {
                        return -1;
                    }
                }

                if (count2 == 0)
                    return 1;

                count1--;
                count2--;
            }

            static int CountContainingTypes(INamedTypeSymbol namedType)
            {
                int count = 0;

                while (true)
                {
                    namedType = namedType.ContainingType;

                    if (namedType == null)
                        break;

                    count++;
                }

                return count;
            }

            static INamedTypeSymbol GetContainingType(INamedTypeSymbol namedType, int count)
            {
                while (count > 0)
                {
                    namedType = namedType.ContainingType;
                    count--;
                }

                return namedType;
            }

            static int GetRank(INamedTypeSymbol symbol)
            {
                switch (symbol.TypeKind)
                {
                    case TypeKind.Module:
                        return 1;
                    case TypeKind.Class:
                        return 2;
                    case TypeKind.Struct:
                        return 3;
                    case TypeKind.Interface:
                        return 4;
                    case TypeKind.Enum:
                        return 5;
                    case TypeKind.Delegate:
                        return 6;
                }

                Debug.Fail(symbol.ToDisplayString(SymbolDisplayFormats.Test));

                return 0;
            }
        }
    }
}
