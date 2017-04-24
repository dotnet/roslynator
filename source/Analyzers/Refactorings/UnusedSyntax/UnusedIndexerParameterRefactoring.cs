// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.UnusedSyntax
{
    internal class UnusedIndexerParameterRefactoring : UnusedSyntaxRefactoring<IndexerDeclarationSyntax, BracketedParameterListSyntax, ParameterSyntax, IParameterSymbol>
    {
        private UnusedIndexerParameterRefactoring()
        {
        }

        public static UnusedIndexerParameterRefactoring Instance { get; } = new UnusedIndexerParameterRefactoring();

        protected override ImmutableArray<ParameterSyntax> FindUnusedSyntax(
            IndexerDeclarationSyntax node,
            BracketedParameterListSyntax list,
            SeparatedSyntaxList<ParameterSyntax> separatedList,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (!node.IsParentKind(SyntaxKind.InterfaceDeclaration)
                && GetBody(node) != null
                && !GetModifiers(node).ContainsAny(SyntaxKind.AbstractKeyword, SyntaxKind.VirtualKeyword, SyntaxKind.OverrideKeyword)
                && semanticModel.GetDeclaredSymbol(node, cancellationToken)?.ImplementsInterfaceMember() == false)
            {
                return base.FindUnusedSyntax(node, list, separatedList, semanticModel, cancellationToken);
            }
            else
            {
                return ImmutableArray<ParameterSyntax>.Empty;
            }
        }

        protected override CSharpSyntaxNode GetBody(IndexerDeclarationSyntax node)
        {
            return (CSharpSyntaxNode)node.AccessorList ?? node.ExpressionBody;
        }

        protected override string GetIdentifier(ParameterSyntax syntax)
        {
            return syntax.Identifier.ValueText;
        }

        protected override BracketedParameterListSyntax GetList(IndexerDeclarationSyntax node)
        {
            return node.ParameterList;
        }

        protected override SyntaxTokenList GetModifiers(IndexerDeclarationSyntax node)
        {
            return node.Modifiers;
        }

        protected override SeparatedSyntaxList<ParameterSyntax> GetSeparatedList(BracketedParameterListSyntax list)
        {
            return list.Parameters;
        }

        public override ISymbol GetSymbol(IdentifierNameSyntax identifierName, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            ISymbol symbol = base.GetSymbol(identifierName, semanticModel, cancellationToken);

            if (symbol != null)
            {
                ISymbol containingSymbol = symbol.ContainingSymbol;

                if (containingSymbol?.IsMethod() == true)
                {
                    var methodSymbol = (IMethodSymbol)containingSymbol;

                    ISymbol associatedSymbol = methodSymbol.AssociatedSymbol;

                    if (associatedSymbol?.IsKind(SymbolKind.Property) == true)
                    {
                        var propertySymbol = (IPropertySymbol)associatedSymbol;

                        if (propertySymbol.IsIndexer)
                        {
                            ImmutableArray<IParameterSymbol> parameters = propertySymbol.Parameters;

                            if (parameters.Any())
                            {
                                string name = identifierName.Identifier.ValueText;

                                foreach (IParameterSymbol parameterSymbol in parameters)
                                {
                                    if (string.Equals(name, parameterSymbol.Name, StringComparison.Ordinal))
                                        return parameterSymbol;
                                }
                            }
                        }
                    }
                }
            }

            return default(ISymbol);
        }
    }
}
