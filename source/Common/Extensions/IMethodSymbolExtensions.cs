// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis
{
    public static class IMethodSymbolExtensions
    {
        public static bool IsOrdinary(this IMethodSymbol methodSymbol)
        {
            if (methodSymbol == null)
                throw new ArgumentNullException(nameof(methodSymbol));

            return methodSymbol.MethodKind == MethodKind.Ordinary;
        }

        public static bool IsReducedExtension(this IMethodSymbol methodSymbol)
        {
            if (methodSymbol == null)
                throw new ArgumentNullException(nameof(methodSymbol));

            return methodSymbol.MethodKind == MethodKind.ReducedExtension;
        }
    }
}
