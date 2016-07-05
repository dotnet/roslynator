// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Pihrtsoft.CodeAnalysis.CSharp.CSharpFactory;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class AccessorDeclarationSyntaxExtensions
    {
        public static AccessorDeclarationSyntax WithSemicolonToken(
            this AccessorDeclarationSyntax accessorDeclaration)
        {
            if (accessorDeclaration == null)
                throw new ArgumentNullException(nameof(accessorDeclaration));

            return accessorDeclaration.WithSemicolonToken(SemicolonToken());
        }

        public static AccessorDeclarationSyntax WithoutSemicolonToken(
            this AccessorDeclarationSyntax accessorDeclaration)
        {
            if (accessorDeclaration == null)
                throw new ArgumentNullException(nameof(accessorDeclaration));

            return accessorDeclaration.WithSemicolonToken(NoneToken());
        }

        public static AccessorDeclarationSyntax WithStatement(
            this AccessorDeclarationSyntax accessorDeclaration,
            StatementSyntax statement)
        {
            if (accessorDeclaration == null)
                throw new ArgumentNullException(nameof(accessorDeclaration));

            return accessorDeclaration.WithBody(Block(statement));
        }

        public static AccessorDeclarationSyntax WithStatements(
            this AccessorDeclarationSyntax accessorDeclaration,
            params StatementSyntax[] statements)
        {
            if (accessorDeclaration == null)
                throw new ArgumentNullException(nameof(accessorDeclaration));

            return accessorDeclaration.WithBody(Block(statements));
        }
    }
}
