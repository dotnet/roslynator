// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class NamespaceDeclarationSyntaxExtensions
    {
        public static NamespaceDeclarationSyntax WithMembers(
            this NamespaceDeclarationSyntax namespaceDeclaration,
            MemberDeclarationSyntax member)
        {
            if (namespaceDeclaration == null)
                throw new ArgumentNullException(nameof(namespaceDeclaration));

            return namespaceDeclaration.WithMembers(SingletonList(member));
        }

        public static TextSpan HeaderSpan(this NamespaceDeclarationSyntax namespaceDeclaration)
        {
            if (namespaceDeclaration == null)
                throw new ArgumentNullException(nameof(namespaceDeclaration));

            return TextSpan.FromBounds(
                namespaceDeclaration.Span.Start,
                namespaceDeclaration.Name?.Span.End ?? namespaceDeclaration.NamespaceKeyword.Span.End);
        }
    }
}
