// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.UnusedSyntax
{
    internal class UnusedMethodTypeParameterRefactoring : UnusedMethodSyntaxRefactoring<TypeParameterListSyntax, TypeParameterSyntax, ITypeParameterSymbol>
    {
        private UnusedMethodTypeParameterRefactoring()
        {
        }

        public static UnusedMethodTypeParameterRefactoring Instance { get; } = new UnusedMethodTypeParameterRefactoring();

        protected override ImmutableArray<TypeParameterSyntax> FindUnusedSyntax(
            MethodDeclarationSyntax node,
            TypeParameterListSyntax list,
            SeparatedSyntaxList<TypeParameterSyntax> separatedList,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (!node.IsParentKind(SyntaxKind.InterfaceDeclaration)
                && !GetModifiers(node).ContainsAny(SyntaxKind.AbstractKeyword, SyntaxKind.VirtualKeyword, SyntaxKind.OverrideKeyword))
            {
                CSharpSyntaxNode bodyOrExpressionBody = GetBody(node);

                if (bodyOrExpressionBody != null
                    && !UnusedSyntaxHelper.ContainsOnlyThrowNewException(bodyOrExpressionBody, semanticModel, cancellationToken))
                {
                    IMethodSymbol methodSymbol = semanticModel.GetDeclaredSymbol(node, cancellationToken);

                    if (methodSymbol?.ExplicitInterfaceImplementations.IsDefaultOrEmpty == true
                        && !methodSymbol.ImplementsInterfaceMember())
                    {
                        return base.FindUnusedSyntax(node, list, separatedList, semanticModel, cancellationToken);
                    }
                }
            }

            return ImmutableArray<TypeParameterSyntax>.Empty;
        }

        protected override string GetIdentifier(TypeParameterSyntax syntax)
        {
            return syntax.Identifier.ValueText;
        }

        protected override TypeParameterListSyntax GetList(MethodDeclarationSyntax node)
        {
            return node.TypeParameterList;
        }

        protected override SeparatedSyntaxList<TypeParameterSyntax> GetSeparatedList(TypeParameterListSyntax list)
        {
            return list.Parameters;
        }
    }
}
