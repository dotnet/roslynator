// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis.UnusedMember
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
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

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string DebuggerDisplay
        {
            get { return $"{Node.Kind()} {Name}"; }
        }

        internal bool CanBeInDebuggerDisplayAttribute
        {
            get
            {
                switch (Node.Kind())
                {
                    case SyntaxKind.PropertyDeclaration:
                    case SyntaxKind.FieldDeclaration:
                        {
                            return true;
                        }
                    case SyntaxKind.MethodDeclaration:
                        {
                            var methodDeclaration = (MethodDeclarationSyntax)Node;

                            return methodDeclaration.ParameterList?.Parameters.Count == 0;
                        }
                }

                return false;
            }
        }
    }
}
