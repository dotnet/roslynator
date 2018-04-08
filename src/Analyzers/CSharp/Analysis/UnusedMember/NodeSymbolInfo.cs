// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;

namespace Roslynator.CSharp.Analysis.UnusedMember
{
    internal readonly struct NodeSymbolInfo
    {
        public NodeSymbolInfo(string name, SyntaxNode node, ISymbol symbol = null)
        {
            Name = name;
            Node = node;
            Symbol = symbol;
        }

        public string Name { get; }

        public SyntaxNode Node { get; }

        public ISymbol Symbol { get; }
    }
}
