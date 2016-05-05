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
                    return true;
                case SpecialType.System_Boolean:
                    return true;
                case SpecialType.System_Char:
                    return true;
                case SpecialType.System_SByte:
                    return true;
                case SpecialType.System_Byte:
                    return true;
                case SpecialType.System_Int16:
                    return true;
                case SpecialType.System_UInt16:
                    return true;
                case SpecialType.System_Int32:
                    return true;
                case SpecialType.System_UInt32:
                    return true;
                case SpecialType.System_Int64:
                    return true;
                case SpecialType.System_UInt64:
                    return true;
                case SpecialType.System_Decimal:
                    return true;
                case SpecialType.System_Single:
                    return true;
                case SpecialType.System_Double:
                    return true;
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
    }
}
