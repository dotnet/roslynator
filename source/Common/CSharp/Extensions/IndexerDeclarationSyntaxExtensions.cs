// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class IndexerDeclarationSyntaxExtensions
    {
        public static TextSpan HeaderSpan(this IndexerDeclarationSyntax indexerDeclaration)
        {
            if (indexerDeclaration == null)
                throw new ArgumentNullException(nameof(indexerDeclaration));

            return TextSpan.FromBounds(
                indexerDeclaration.Span.Start,
                indexerDeclaration.ParameterList?.Span.End ?? indexerDeclaration.ThisKeyword.Span.End);
        }

        public static AccessorDeclarationSyntax Getter(this IndexerDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            if (propertyDeclaration.AccessorList == null)
                return null;

            SyntaxList<AccessorDeclarationSyntax> accessors = propertyDeclaration.AccessorList.Accessors;

            for (int i = 0; i < accessors.Count; i++)
            {
                if (accessors[i].IsKind(SyntaxKind.GetAccessorDeclaration))
                    return accessors[i];
            }

            return null;
        }

        public static AccessorDeclarationSyntax Setter(this IndexerDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            if (propertyDeclaration.AccessorList == null)
                return null;

            SyntaxList<AccessorDeclarationSyntax> accessors = propertyDeclaration.AccessorList.Accessors;

            for (int i = 0; i < accessors.Count; i++)
            {
                if (accessors[i].IsKind(SyntaxKind.SetAccessorDeclaration))
                    return accessors[i];
            }

            return null;
        }
    }
}
