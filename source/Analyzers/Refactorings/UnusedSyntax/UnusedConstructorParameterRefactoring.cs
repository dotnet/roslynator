// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.UnusedSyntax
{
    internal class UnusedConstructorParameterRefactoring : UnusedSyntaxRefactoring<ConstructorDeclarationSyntax, ParameterListSyntax, ParameterSyntax, IParameterSymbol>
    {
        protected override CSharpSyntaxNode GetBody(ConstructorDeclarationSyntax node)
        {
            return node.Body;
        }

        protected override string GetIdentifier(ParameterSyntax syntax)
        {
            return syntax.Identifier.ValueText;
        }

        protected override ParameterListSyntax GetList(ConstructorDeclarationSyntax node)
        {
            return node.ParameterList;
        }

        protected override SyntaxTokenList GetModifiers(ConstructorDeclarationSyntax node)
        {
            return node.Modifiers;
        }

        protected override SeparatedSyntaxList<ParameterSyntax> GetSeparatedList(ParameterListSyntax list)
        {
            return list.Parameters;
        }
    }
}
