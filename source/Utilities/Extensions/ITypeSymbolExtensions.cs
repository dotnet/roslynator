// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace Pihrtsoft.CodeAnalysis
{
    public static class ITypeSymbolExtensions
    {
        public static bool SupportsExplicitDeclaration(this ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            if (!typeSymbol.IsAnonymousType)
            {
                switch (typeSymbol.Kind)
                {
                    case SymbolKind.TypeParameter:
                        return true;
                    case SymbolKind.ArrayType:
                        return SupportsExplicitDeclaration(((IArrayTypeSymbol)typeSymbol).ElementType);
                    case SymbolKind.NamedType:
                        return !((INamedTypeSymbol)typeSymbol).IsAnyTypeArgumentAnonymousType();
                }
            }

            return false;
        }

        public static bool IsPubliclyAccessible(this ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            Accessibility accessibility = typeSymbol.DeclaredAccessibility;

            return accessibility == Accessibility.Protected
                || accessibility == Accessibility.ProtectedOrInternal
                || accessibility == Accessibility.Public;
        }

        public static bool IsInt32(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.SpecialType == SpecialType.System_Int32;
        }

        public static bool IsVoid(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.SpecialType == SpecialType.System_Void;
        }

        public static bool IsBoolean(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.SpecialType == SpecialType.System_Boolean;
        }

        public static bool IsString(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.SpecialType == SpecialType.System_String;
        }

        public static IEnumerable<INamedTypeSymbol> BaseTypes(this ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            INamedTypeSymbol current = typeSymbol.BaseType;

            while (current != null)
            {
                yield return current;
                current = current.BaseType;
            }
        }

        public static IEnumerable<ITypeSymbol> BaseTypesAndSelf(this ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            ITypeSymbol current = typeSymbol;

            while (current != null)
            {
                yield return current;
                current = current.BaseType;
            }
        }

        public static bool IsPredefinedType(this ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            switch (typeSymbol.SpecialType)
            {
                case SpecialType.System_Object:
                case SpecialType.System_Boolean:
                case SpecialType.System_Char:
                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                case SpecialType.System_Decimal:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                case SpecialType.System_String:
                    return true;
                default:
                    return false;
            }
        }

        public static bool Implements(this ITypeSymbol typeSymbol, SpecialType specialType)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            ImmutableArray<INamedTypeSymbol> allInterfaces = typeSymbol.AllInterfaces;

            for (int i = 0; i < allInterfaces.Length; i++)
            {
                if (allInterfaces[i].SpecialType == specialType)
                    return true;
            }

            return false;
        }

        public static bool HasPublicIndexer(this ITypeSymbol typeSymbol)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            foreach (ISymbol symbol in typeSymbol.GetMembers("get_Item"))
            {
                if (symbol.IsMethod()
                    && !symbol.IsStatic
                    && symbol.IsPublic())
                {
                    var methodSymbol = (IMethodSymbol)symbol;

                    ImmutableArray<IParameterSymbol> parameters = methodSymbol.Parameters;

                    if (parameters.Length == 1
                        && parameters[0].Type?.IsInt32() == true)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool IsClass(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.TypeKind == TypeKind.Class;
        }

        public static bool IsStaticClass(this ITypeSymbol typeSymbol)
        {
            return typeSymbol.IsStatic && typeSymbol.IsClass();
        }

        public static bool IsInterface(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.TypeKind == TypeKind.Interface;
        }

        public static bool IsStruct(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.TypeKind == TypeKind.Struct;
        }

        public static bool IsEnum(this ITypeSymbol typeSymbol)
        {
            return typeSymbol?.TypeKind == TypeKind.Enum;
        }
    }
}
