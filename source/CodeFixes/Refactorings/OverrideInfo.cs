// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator.CSharp.Refactorings
{
    internal class OverrideInfo
    {
        public OverrideInfo(ISymbol symbol, ISymbol overriddenSymbol)
        {
            Symbol = symbol;
            OverriddenSymbol = overriddenSymbol;
        }

        public ISymbol Symbol { get; }
        public ISymbol OverriddenSymbol { get; }
    }
}
