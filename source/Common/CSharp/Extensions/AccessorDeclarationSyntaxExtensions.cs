// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Pihrtsoft.CodeAnalysis.CSharp.CSharpFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class AccessorDeclarationSyntaxExtensions
    {
        public static bool IsGetter(this AccessorDeclarationSyntax accessorDeclaration)
        {
            if (accessorDeclaration == null)
                throw new ArgumentNullException(nameof(accessorDeclaration));

            return accessorDeclaration.Keyword.IsKind(SyntaxKind.GetKeyword);
        }

        public static bool IsSetter(this AccessorDeclarationSyntax accessorDeclaration)
        {
            if (accessorDeclaration == null)
                throw new ArgumentNullException(nameof(accessorDeclaration));

            return accessorDeclaration.Keyword.IsKind(SyntaxKind.SetKeyword);
        }

        public static bool IsAutoGetter(this AccessorDeclarationSyntax accessorDeclaration)
        {
            if (accessorDeclaration == null)
                throw new ArgumentNullException(nameof(accessorDeclaration));

            return IsAutoAccessor(accessorDeclaration, SyntaxKind.GetKeyword);
        }

        public static bool IsAutoSetter(this AccessorDeclarationSyntax accessorDeclaration)
        {
            if (accessorDeclaration == null)
                throw new ArgumentNullException(nameof(accessorDeclaration));

            return IsAutoAccessor(accessorDeclaration, SyntaxKind.SetKeyword);
        }

        private static bool IsAutoAccessor(this AccessorDeclarationSyntax accessorDeclaration, SyntaxKind kind)
        {
            return accessorDeclaration.Keyword.IsKind(kind)
                && accessorDeclaration.SemicolonToken.IsKind(SyntaxKind.SemicolonToken)
                && accessorDeclaration.Body == null;
        }

        public static AccessorDeclarationSyntax WithoutSemicolonToken(
            this AccessorDeclarationSyntax accessorDeclaration)
        {
            if (accessorDeclaration == null)
                throw new ArgumentNullException(nameof(accessorDeclaration));

            return accessorDeclaration.WithSemicolonToken(NoneToken());
        }
    }
}
