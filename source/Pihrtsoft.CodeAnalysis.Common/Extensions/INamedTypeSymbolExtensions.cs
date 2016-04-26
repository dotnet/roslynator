// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis
{
    public static class INamedTypeSymbolExtensions
    {
        public static bool IsAnyTypeArgumentAnonymousType(this INamedTypeSymbol namedType)
        {
            if (namedType == null)
                throw new ArgumentNullException(nameof(namedType));

            if (namedType.TypeArguments.Length > 0)
            {
                var stack = new Stack<ITypeSymbol>(namedType.TypeArguments);

                while (stack.Count > 0)
                {
                    ITypeSymbol type = stack.Pop();

                    if (type.IsAnonymousType)
                        return true;

                    if (type.IsKind(SymbolKind.NamedType))
                    {
                        ImmutableArray<ITypeSymbol> typeArguments = ((INamedTypeSymbol)type).TypeArguments;

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

            if (namedType.TypeArguments.Length > 0)
            {
                var stack = new Stack<ITypeSymbol>(namedType.TypeArguments);

                while (stack.Count > 0)
                {
                    ITypeSymbol type = stack.Pop();

                    yield return type;

                    if (type.IsKind(SymbolKind.NamedType))
                    {
                        ImmutableArray<ITypeSymbol> typeArguments = ((INamedTypeSymbol)type).TypeArguments;

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
