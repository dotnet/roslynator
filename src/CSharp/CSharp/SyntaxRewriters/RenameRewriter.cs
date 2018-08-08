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
            SyntaxToken identifier = node.Identifier;

            if (string.Equals(identifier.ValueText, _name, StringComparison.Ordinal))
            {
                ISymbol symbol = SemanticModel.GetSymbol(node, CancellationToken);

                if (Symbol.Equals(symbol))
                {
                    SyntaxToken newIdentifier = SyntaxFactory.Identifier(
                        identifier.LeadingTrivia,
                        NewName,
                        identifier.TrailingTrivia);

                    return node.WithIdentifier(newIdentifier);
                }
            }

            return base.VisitIdentifierName(node);
        }
    }
}
