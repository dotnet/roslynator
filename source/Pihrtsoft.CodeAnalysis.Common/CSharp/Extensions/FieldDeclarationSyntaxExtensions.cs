// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class FieldDeclarationSyntaxExtensions
    {
        public static FieldDeclarationSyntax WithModifiers(
            this FieldDeclarationSyntax fieldDeclaration,
            params SyntaxKind[] tokenKinds)
        {
            if (fieldDeclaration == null)
                throw new ArgumentNullException(nameof(fieldDeclaration));

            return fieldDeclaration.WithModifiers(CSharpFactory.TokenList(tokenKinds));
        }

        public static FieldDeclarationSyntax WithModifier(
            this FieldDeclarationSyntax fieldDeclaration,
            SyntaxKind tokenKind)
        {
            if (fieldDeclaration == null)
                throw new ArgumentNullException(nameof(fieldDeclaration));

            return fieldDeclaration.WithModifiers(TokenList(Token(tokenKind)));
        }
    }
}
