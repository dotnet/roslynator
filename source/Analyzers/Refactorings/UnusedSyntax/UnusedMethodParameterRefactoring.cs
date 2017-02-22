// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Refactorings.UnusedSyntax
{
    internal class UnusedMethodParameterRefactoring : UnusedSyntaxRefactoring<MethodDeclarationSyntax, ParameterListSyntax, ParameterSyntax, IParameterSymbol>
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
                && GetBody(node) != null
                && !GetModifiers(node).ContainsAny(SyntaxKind.AbstractKeyword, SyntaxKind.VirtualKeyword, SyntaxKind.OverrideKeyword))
            {
                IMethodSymbol methodSymbol = semanticModel.GetDeclaredSymbol(node, cancellationToken);

                if (methodSymbol != null
                    && !SymbolUtility.IsEventHandlerMethod(methodSymbol, semanticModel)
                    && !methodSymbol.ImplementsInterfaceMember())
                {
                    return base.FindUnusedSyntax(node, list, separatedList, semanticModel, cancellationToken);
                }
            }

            return ImmutableArray<ParameterSyntax>.Empty;
        }

        protected override CSharpSyntaxNode GetBody(MethodDeclarationSyntax node)
        {
            return node.BodyOrExpressionBody();
        }

        protected override string GetIdentifier(ParameterSyntax syntax)
        {
            return syntax.Identifier.ValueText;
        }

        protected override ParameterListSyntax GetList(MethodDeclarationSyntax node)
        {
            return node.ParameterList;
        }

        protected override SyntaxTokenList GetModifiers(MethodDeclarationSyntax node)
        {
            return node.Modifiers;
        }

        protected override SeparatedSyntaxList<ParameterSyntax> GetSeparatedList(ParameterListSyntax list)
        {
            return list.Parameters;
        }
    }
}
