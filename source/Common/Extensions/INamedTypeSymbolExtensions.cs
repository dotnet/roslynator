// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis
{
    public static class INamedTypeSymbolExtensions
    {
        public static bool IsNullableOf(this INamedTypeSymbol namedTypeSymbol, SpecialType specialType)
        {
            if (namedTypeSymbol == null)
                throw new ArgumentNullException(nameof(namedTypeSymbol));

            return namedTypeSymbol.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T
                && namedTypeSymbol.TypeArguments[0].SpecialType == specialType;
        }

        public static bool IsNullableOf(this INamedTypeSymbol namedTypeSymbol, ITypeSymbol typeSymbol)
        {
            if (namedTypeSymbol == null)
                throw new ArgumentNullException(nameof(namedTypeSymbol));

            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            return namedTypeSymbol.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T
                && namedTypeSymbol.TypeArguments[0] == typeSymbol;
        }

        public static bool IsAnyTypeArgumentAnonymousType(this INamedTypeSymbol namedType)
        {
            if (namedType == null)
                throw new ArgumentNullException(nameof(namedType));

            ImmutableArray<ITypeSymbol> typeArguments = namedType.TypeArguments;

            if (typeArguments.Length > 0)
            {
                var stack = new Stack<ITypeSymbol>(typeArguments);

                while (stack.Count > 0)
                {
                    ITypeSymbol type = stack.Pop();

                    if (type.IsAnonymousType)
                        return true;

                    if (type.IsNamedType())
                    {
                        typeArguments = ((INamedTypeSymbol)type).TypeArguments;

                        for (int i = 0; i < typeArguments.Length; i++)
                            stack.Push(typeArguments[i]);
                    }
                }
            }

            return false;
        }

        public static IEnumerable<ITypeSymbol> GetAllTypeArguments(this INamedTypeSymbol namedType)
        {
            if (namedType == null)
                throw new ArgumentNullException(nameof(namedType));

            ImmutableArray<ITypeSymbol> typeArguments = namedType.TypeArguments;

            if (typeArguments.Length > 0)
            {
                var stack = new Stack<ITypeSymbol>(typeArguments);

                while (stack.Count > 0)
                {
                    ITypeSymbol type = stack.Pop();

                    yield return type;

                    if (type.IsNamedType())
                    {
                        typeArguments = ((INamedTypeSymbol)type).TypeArguments;

                        for (int i = 0; i < typeArguments.Length; i++)
                            stack.Push(typeArguments[i]);
                    }
                }
            }
        }

        public static IEnumerable<ITypeSymbol> GetAllTypeArgumentsAndSelf(this INamedTypeSymbol namedType)
        {
            if (namedType == null)
                throw new ArgumentNullException(nameof(namedType));

            yield return namedType;

            foreach (ITypeSymbol typeSymbol in GetAllTypeArguments(namedType))
                yield return typeSymbol;
        }
    }
}
