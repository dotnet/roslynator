// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.UnusedSyntax
{
    internal class UnusedMethodParameterRefactoring : UnusedMethodSyntaxRefactoring<ParameterListSyntax, ParameterSyntax, IParameterSymbol>
    {
        private UnusedMethodParameterRefactoring()
        {
        }

        public static UnusedMethodParameterRefactoring Instance { get; } = new UnusedMethodParameterRefactoring();

        protected override ImmutableArray<ParameterSyntax> FindUnusedSyntax(
            MethodDeclarationSyntax node,
            ParameterListSyntax list,
            SeparatedSyntaxList<ParameterSyntax> separatedList,
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

                    if (methodSymbol?.IsEventHandler(semanticModel) == false
                        && methodSymbol.ExplicitInterfaceImplementations.IsDefaultOrEmpty
                        && !methodSymbol.ImplementsInterfaceMember())
                    {
                        return base.FindUnusedSyntax(node, list, separatedList, semanticModel, cancellationToken);
                    }
                }
            }

            return ImmutableArray<ParameterSyntax>.Empty;
        }

        protected override string GetIdentifier(ParameterSyntax syntax)
        {
            return syntax.Identifier.ValueText;
        }

        protected override ParameterListSyntax GetList(MethodDeclarationSyntax node)
        {
            return node.ParameterList;
        }

        protected override SeparatedSyntaxList<ParameterSyntax> GetSeparatedList(ParameterListSyntax list)
        {
            return list.Parameters;
        }
    }
}
