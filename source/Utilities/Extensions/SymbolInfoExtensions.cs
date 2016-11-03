// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator
{
    public static class SymbolInfoExtensions
    {
        public static bool IsEmpty(this SymbolInfo symbolInfo)
        {
            return symbolInfo.Symbol == null
                && symbolInfo.CandidateSymbols.Length == 0;
        }
    }
}
