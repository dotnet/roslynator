// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.UnusedSyntax
{
    internal abstract class UnusedLocalFunctionSyntaxRefactoring<TListSyntax, TSyntax, TSymbol>
        : UnusedSyntaxRefactoring<LocalFunctionStatementSyntax, TListSyntax, TSyntax, TSymbol>
        where TListSyntax : SyntaxNode
        where TSyntax : SyntaxNode
        where TSymbol : ISymbol
    {
        protected override CSharpSyntaxNode GetBody(LocalFunctionStatementSyntax node)
        {
            return node.BodyOrExpressionBody();
        }

        protected override SyntaxTokenList GetModifiers(LocalFunctionStatementSyntax node)
        {
            return node.Modifiers;
        }
    }
}
