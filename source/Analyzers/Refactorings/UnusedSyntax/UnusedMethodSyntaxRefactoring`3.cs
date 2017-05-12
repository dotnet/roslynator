// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.UnusedSyntax
{
    internal abstract class UnusedMethodSyntaxRefactoring<TListSyntax, TSyntax, TSymbol>
        : UnusedSyntaxRefactoring<MethodDeclarationSyntax, TListSyntax, TSyntax, TSymbol>
        where TListSyntax : SyntaxNode
        where TSyntax : SyntaxNode
        where TSymbol : ISymbol
    {
        protected override CSharpSyntaxNode GetBody(MethodDeclarationSyntax node)
        {
            return node.BodyOrExpressionBody();
        }

        protected override SyntaxTokenList GetModifiers(MethodDeclarationSyntax node)
        {
            return node.Modifiers;
        }
    }
}
