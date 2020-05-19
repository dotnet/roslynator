// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.SyntaxRewriters
{
    internal class RenameRewriter : CSharpSyntaxRewriter
    {
        private readonly string _name;

        public RenameRewriter(
            ISymbol symbol,
            string newName,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            Symbol = symbol;
            NewName = newName;
            SemanticModel = semanticModel;
            CancellationToken = cancellationToken;

            _name = symbol.Name;
        }

        public string NewName { get; }

        public ISymbol Symbol { get; }

        public SemanticModel SemanticModel { get; }

        public CancellationToken CancellationToken { get; }

        public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
        {
            if (string.Equals(node.Identifier.ValueText, _name, StringComparison.Ordinal))
            {
                ISymbol symbol = SemanticModel.GetSymbol(node, CancellationToken);

                if (SymbolEqualityComparer.Default.Equals(Symbol, symbol))
                    return Rename(node);
            }

            return base.VisitIdentifierName(node);
        }

        protected virtual SyntaxNode Rename(IdentifierNameSyntax node)
        {
            SyntaxToken newIdentifier = SyntaxFactory.Identifier(
                node.Identifier.LeadingTrivia,
                NewName,
                node.Identifier.TrailingTrivia);

            return node.WithIdentifier(newIdentifier);
        }
    }
}
