// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Roslynator
{
    internal static class IdentifierGenerator
    {
        public static string Generate(ITypeSymbol typeSymbol, bool firstCharToLower = false)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (typeSymbol.IsKind(SymbolKind.ErrorType, SymbolKind.DynamicType))
                return null;

            ITypeSymbol typeSymbol2 = ExtractFromNullableType(typeSymbol);

            ITypeSymbol typeSymbol3 = ExtractFromArrayOrGenericCollection(typeSymbol2);

            string name = GetName(typeSymbol3);

            if (string.IsNullOrEmpty(name))
                return null;

            if (typeSymbol3.TypeKind == TypeKind.Interface
                && name.Length > 1
                && name[0] == 'I')
            {
                name = name.Substring(1);
            }

            if (name.Length > 1
                && UsePlural(typeSymbol2))
            {
                name = TextUtility.RemoveSuffix(name, "Collection");

                if (name.EndsWith("s", StringComparison.Ordinal) || name.EndsWith("x", StringComparison.Ordinal))
                    name += "es";
                else if (name.EndsWith("y", StringComparison.Ordinal))
                    name = name.Remove(name.Length - 1) + "ies";
                else
                    name += "s";
            }

            if (firstCharToLower)
                name = TextUtility.FirstCharToLowerInvariant(name);

            return name;
        }

        private static ITypeSymbol ExtractFromNullableType(ITypeSymbol typeSymbol)
        {
            if (typeSymbol.IsNamedType())
            {
                var namedTypeSymbol = (INamedTypeSymbol)typeSymbol;

                if (namedTypeSymbol.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T)
                    return namedTypeSymbol.TypeArguments[0];
            }

            return typeSymbol;
        }

        private static ITypeSymbol ExtractFromArrayOrGenericCollection(ITypeSymbol typeSymbol)
        {
            switch (typeSymbol.Kind)
            {
                case SymbolKind.ArrayType:
                    {
                        return ((IArrayTypeSymbol)typeSymbol).ElementType;
                    }
                case SymbolKind.NamedType:
                    {
                        var namedTypeSymbol = (INamedTypeSymbol)typeSymbol;

                        if (namedTypeSymbol.TypeArguments.Length == 1
                            && namedTypeSymbol.Implements(SpecialType.System_Collections_IEnumerable))
                        {
                            return namedTypeSymbol.TypeArguments[0];
                        }

                        break;
                    }
            }

            return typeSymbol;
        }

        private static bool UsePlural(ITypeSymbol typeSymbol)
        {
            switch (typeSymbol.Kind)
            {
                case SymbolKind.ArrayType:
                    {
                        return true;
                    }
                case SymbolKind.NamedType:
                    {
                        var namedTypeSymbol = (INamedTypeSymbol)typeSymbol;

                        if (namedTypeSymbol.TypeArguments.Length <= 1)
                        {
                            ImmutableArray<INamedTypeSymbol> allInterfaces = typeSymbol.AllInterfaces;

                            return allInterfaces.Any(f => f.SpecialType == SpecialType.System_Collections_IEnumerable)
                                && !allInterfaces.Any(f => ImplementsIDictionary(f));
                        }

                        break;
                    }
            }

            return false;
        }

        private static bool ImplementsIDictionary(INamedTypeSymbol namedTypeSymbol)
        {
            return string.Equals(namedTypeSymbol.ContainingNamespace?.ToString(), MetadataNames.System_Collections, StringComparison.Ordinal)
                && string.Equals(namedTypeSymbol.MetadataName, "IDictionary", StringComparison.Ordinal);
        }

        private static string GetName(ITypeSymbol typeSymbol)
        {
            if (typeSymbol.IsTypeParameter())
            {
                if (typeSymbol.Name.Length > 1
                    && typeSymbol.Name[0] == 'T')
                {
                    return typeSymbol.Name.Substring(1);
                }
            }
            else if (typeSymbol.IsAnonymousType)
            {
                return null;
            }
            else if (typeSymbol.SupportsPredefinedType())
            {
                return null;
            }

            return typeSymbol.Name;
        }
    }
}
