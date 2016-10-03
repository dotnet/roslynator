// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class AccessorListExtensions
    {
        public static AccessorDeclarationSyntax Getter(this AccessorListSyntax accessorList)
        {
            if (accessorList == null)
                throw new ArgumentNullException(nameof(accessorList));

            return Accessor(accessorList, SyntaxKind.GetAccessorDeclaration);
        }

        public static AccessorDeclarationSyntax Setter(this AccessorListSyntax accessorList)
        {
            if (accessorList == null)
                throw new ArgumentNullException(nameof(accessorList));

            return Accessor(accessorList, SyntaxKind.SetAccessorDeclaration);
        }

        public static bool ContainsGetter(this AccessorListSyntax accessorList)
        {
            return Getter(accessorList) != null;
        }

        public static bool ContainsSetter(this AccessorListSyntax accessorList)
        {
            return Setter(accessorList) != null;
        }

        private static AccessorDeclarationSyntax Accessor(this AccessorListSyntax accessorList, SyntaxKind kind)
        {
            return accessorList
                .Accessors
                .FirstOrDefault(accessor => accessor.IsKind(kind));
        }
    }
}
