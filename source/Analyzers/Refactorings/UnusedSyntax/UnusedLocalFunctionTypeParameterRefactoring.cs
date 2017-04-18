// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.UnusedSyntax
{
    internal class UnusedLocalFunctionTypeParameterRefactoring : UnusedSyntaxRefactoring<LocalFunctionStatementSyntax, TypeParameterListSyntax, TypeParameterSyntax, ITypeParameterSymbol>
    {
        private UnusedLocalFunctionTypeParameterRefactoring()
        {
        }

        public static UnusedLocalFunctionTypeParameterRefactoring Instance { get; } = new UnusedLocalFunctionTypeParameterRefactoring();

        protected override ImmutableArray<TypeParameterSyntax> FindUnusedSyntax(
            LocalFunctionStatementSyntax node,
            TypeParameterListSyntax list,
            SeparatedSyntaxList<TypeParameterSyntax> separatedList,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (GetBody(node) != null)
            {
                return base.FindUnusedSyntax(node, list, separatedList, semanticModel, cancellationToken);
            }
            else
            {
                return ImmutableArray<TypeParameterSyntax>.Empty;
            }
        }

        protected override CSharpSyntaxNode GetBody(LocalFunctionStatementSyntax node)
        {
            return node.BodyOrExpressionBody();
        }

        protected override string GetIdentifier(TypeParameterSyntax syntax)
        {
            return syntax.Identifier.ValueText;
        }

        protected override TypeParameterListSyntax GetList(LocalFunctionStatementSyntax node)
        {
            return node.TypeParameterList;
        }

        protected override SyntaxTokenList GetModifiers(LocalFunctionStatementSyntax node)
        {
            return node.Modifiers;
        }

        protected override SeparatedSyntaxList<TypeParameterSyntax> GetSeparatedList(TypeParameterListSyntax list)
        {
            return list.Parameters;
        }
    }
}
