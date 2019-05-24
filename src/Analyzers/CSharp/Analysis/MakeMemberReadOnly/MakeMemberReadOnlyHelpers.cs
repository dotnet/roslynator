// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator.CSharp.Analysis.MakeMemberReadOnly
{
    internal static class MakeMemberReadOnlyHelpers
    {
        public static bool ValidateType(ITypeSymbol type)
        {
            if (type.Kind == SymbolKind.ErrorType)
                return false;

            return type.IsReferenceType
                || type.TypeKind == TypeKind.Enum
                || CSharpFacts.IsSimpleType(type.SpecialType);
        }
    }
}
