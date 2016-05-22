// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis
{
    public static class ITypeSymbolExtensions
    {
        public static bool IsKind(this ITypeSymbol typeSymbol, SymbolKind symbolKind)
        {
            if (typeSymbol == null)
                throw new ArgumentNullException(nameof(typeSymbol));

            return typeSymbol.Kind == symbolKind;
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

            for (int i = 0; i < typeSymbol.AllInterfaces.Length; i++)
            {
                if (typeSymbol.AllInterfaces[i].SpecialType == specialType)
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
                if (symbol.IsKind(SymbolKind.Method)
                    && !symbol.IsStatic
                    && symbol.DeclaredAccessibility == Accessibility.Public)
                {
                    var methodSymbol = (IMethodSymbol)symbol;

                    if (methodSymbol.Parameters.Length == 1
                        && methodSymbol.Parameters[0].Type?.SpecialType == SpecialType.System_Int32)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
