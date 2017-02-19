// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;

namespace Roslynator.CSharp.Refactorings.UnusedSyntax
{
    internal class UnusedMethodParameterRefactoring : UnusedSyntaxRefactoring<MethodDeclarationSyntax, ParameterListSyntax, ParameterSyntax, IParameterSymbol>
    {
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
