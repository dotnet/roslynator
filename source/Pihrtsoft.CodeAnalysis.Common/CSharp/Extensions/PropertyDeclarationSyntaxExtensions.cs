// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Pihrtsoft.CodeAnalysis.CSharp.CSharpFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class PropertyDeclarationExtensions
    {
        public static PropertyDeclarationSyntax WithModifiers(
            this PropertyDeclarationSyntax propertyDeclaration,
            params SyntaxKind[] tokenKinds)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration.WithModifiers(TokenList(tokenKinds));
        }

        public static PropertyDeclarationSyntax WithAttributeLists(
            this PropertyDeclarationSyntax propertyDeclaration,
            params AttributeListSyntax[] attributeLists)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration.WithAttributeLists(List(attributeLists));
        }

        public static PropertyDeclarationSyntax WithAttributes(
            this PropertyDeclarationSyntax propertyDeclaration,
            params AttributeSyntax[] attributes)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration
                .WithAttributeLists(
                    AttributeList(
                        SeparatedList(attributes)));
        }

        public static PropertyDeclarationSyntax WithAttribute(
            this PropertyDeclarationSyntax propertyDeclaration,
            AttributeSyntax attribute)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration
                .WithAttributeLists(
                    AttributeList(
                        SingletonSeparatedList(attribute)));
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

        public static PropertyDeclarationSyntax WithInitializer(
            this PropertyDeclarationSyntax propertyDeclaration,
            ExpressionSyntax value)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration.WithInitializer(EqualsValueClause(value));
        }

        public static PropertyDeclarationSyntax WithSemicolonToken(
            this PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            return propertyDeclaration.WithSemicolonToken(SemicolonToken());
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

            if (propertyDeclaration.AccessorList == null)
                return null;

            return propertyDeclaration.AccessorList.Getter();
        }

        public static AccessorDeclarationSyntax Setter(this PropertyDeclarationSyntax propertyDeclaration)
        {
            if (propertyDeclaration == null)
                throw new ArgumentNullException(nameof(propertyDeclaration));

            if (propertyDeclaration.AccessorList == null)
                return null;

            return propertyDeclaration.AccessorList.Setter();
        }

        public static bool ContainsGetter(this PropertyDeclarationSyntax propertyDeclaration)
            => Getter(propertyDeclaration) != null;

        public static bool ContainsSetter(this PropertyDeclarationSyntax propertyDeclaration)
            => Setter(propertyDeclaration) != null;
    }
}
