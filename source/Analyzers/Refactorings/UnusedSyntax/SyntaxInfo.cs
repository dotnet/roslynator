// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator.CSharp.Refactorings.UnusedSyntax
{
    internal struct SyntaxInfo<TSyntax, TSymbol>
        where TSyntax : SyntaxNode
        where TSymbol : ISymbol
    {
        public SyntaxInfo(TSyntax syntax, TSymbol symbol = default(TSymbol))
        {
            Syntax = syntax;
            Symbol = symbol;
        }

        public TSyntax Syntax { get; }

        public TSymbol Symbol { get; }

        public SyntaxInfo<TSyntax, TSymbol> WithSymbol(TSymbol symbol)
        {
            return new SyntaxInfo<TSyntax, TSymbol>(Syntax, symbol);
        }
    }
}
