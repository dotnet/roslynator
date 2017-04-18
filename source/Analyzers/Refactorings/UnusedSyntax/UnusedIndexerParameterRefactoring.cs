// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
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
    }
}
