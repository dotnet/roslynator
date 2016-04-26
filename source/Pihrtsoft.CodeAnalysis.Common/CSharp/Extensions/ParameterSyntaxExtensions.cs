// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class ParameterSyntaxExtensions
    {
        public static BlockSyntax GetMethodOrConstructorBody(this ParameterSyntax parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            SyntaxNode syntaxNode = parameter.FirstAncestorOrSelf(
                SyntaxKind.MethodDeclaration,
                SyntaxKind.ConstructorDeclaration);

            switch (syntaxNode?.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)syntaxNode).Body;
                case SyntaxKind.ConstructorDeclaration:
                    return ((ConstructorDeclarationSyntax)syntaxNode).Body;
                default:
                    return null;
            }
        }
    }
}
