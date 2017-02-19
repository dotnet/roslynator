// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.UnusedSyntax
{
    internal class UnusedIndexerParameterRefactoring : UnusedSyntaxRefactoring<IndexerDeclarationSyntax, BracketedParameterListSyntax, ParameterSyntax, IParameterSymbol>
    {
        protected override CSharpSyntaxNode GetBody(IndexerDeclarationSyntax node)
        {
            return node.AccessorList;
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
