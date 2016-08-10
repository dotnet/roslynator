// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Pihrtsoft.CodeAnalysis.CSharp.CSharpFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class PropertyDeclarationExtensions
    {
        public static bool IsReadOnlyAutoProperty(this PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            AccessorDeclarationSyntax getter = propertyDeclaration.Getter();

            return getter != null
                && getter.Body == null
                && getter.SemicolonToken.IsKind(SyntaxKind.SemicolonToken)
                && !propertyDeclaration.HasSetter();
        }

        public static PropertyDeclarationSyntax WithModifiers(
            this PropertyDeclarationSyntax propertyDeclaration,
            params SyntaxKind[] kinds)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration.WithModifiers(TokenList(kinds));
        }

        public static PropertyDeclarationSyntax WithAttributeLists(
            this PropertyDeclarationSyntax propertyDeclaration,
            params AttributeListSyntax[] attributeLists)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration.WithAttributeLists(List(attributeLists));
        }

        public static PropertyDeclarationSyntax WithAccessorList(
            this PropertyDeclarationSyntax propertyDeclaration,
            params AccessorDeclarationSyntax[] accessors)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration
                .WithAccessorList(
                    AccessorList(
                        List(accessors)));
        }

        public static PropertyDeclarationSyntax WithoutSemicolonToken(
            this PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration.WithSemicolonToken(NoneToken());
        }

        public static PropertyDeclarationSyntax WithoutInitializer(
            this PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration.WithInitializer(null);
        }

        public static TextSpan HeaderSpan(this PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return TextSpan.FromBounds(
                propertyDeclaration.Span.Start,
                propertyDeclaration.Identifier.Span.End);
        }

        public static AccessorDeclarationSyntax Getter(this PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration.AccessorList?.Getter();
        }

        public static AccessorDeclarationSyntax Setter(this PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration.AccessorList?.Setter();
        }

        public static bool HasGetter(this PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration.AccessorList?.ContainsGetter() == true;
        }

        public static bool HasSetter(this PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration.AccessorList?.ContainsSetter() == true;
        }
    }
}
