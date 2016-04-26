// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
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

            return GetAccessor(accessorList, SyntaxKind.GetAccessorDeclaration);
        }

        public static AccessorDeclarationSyntax Setter(this AccessorListSyntax accessorList)
        {
            if (accessorList == null)
                throw new ArgumentNullException(nameof(accessorList));

            return GetAccessor(accessorList, SyntaxKind.SetAccessorDeclaration);
        }

        public static bool ContainsGetter(this AccessorListSyntax accessorList)
            => Getter(accessorList) != null;

        public static bool ContainsSetter(this AccessorListSyntax accessorList)
            => Setter(accessorList) != null;

        private static AccessorDeclarationSyntax GetAccessor(this AccessorListSyntax accessorList, SyntaxKind accessorKind)
        {
            SyntaxList<AccessorDeclarationSyntax> accessors = accessorList.Accessors;

            for (int i = 0; i < accessors.Count; i++)
            {
                if (accessors[i].IsKind(accessorKind))
                    return accessors[i];
            }

            return null;
        }
    }
}
