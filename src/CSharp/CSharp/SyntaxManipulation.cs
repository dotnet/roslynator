// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp
{
    internal static class SyntaxManipulation
    {
        public static T AddAttributeLists<T>(
            T node,
            params AttributeListSyntax[] attributeLists) where T : SyntaxNode
        {
            return AddAttributeLists(node, keepDocumentationCommentOnTop: false, attributeLists);
        }

        public static T AddAttributeLists<T>(
            T node,
            bool keepDocumentationCommentOnTop,
            params AttributeListSyntax[] attributeLists) where T : SyntaxNode
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (attributeLists == null)
                throw new ArgumentNullException(nameof(attributeLists));

            switch (node.Kind())
            {
                case SyntaxKind.EnumDeclaration:
                    return (T)(SyntaxNode)AddAttributeLists((EnumDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
                case SyntaxKind.DelegateDeclaration:
                    return (T)(SyntaxNode)AddAttributeLists((DelegateDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
                case SyntaxKind.ClassDeclaration:
                    return (T)(SyntaxNode)AddAttributeLists((ClassDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
                case SyntaxKind.TypeParameter:
                    return (T)(SyntaxNode)AddAttributeLists((TypeParameterSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
                case SyntaxKind.StructDeclaration:
                    return (T)(SyntaxNode)AddAttributeLists((StructDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
                case SyntaxKind.PropertyDeclaration:
                    return (T)(SyntaxNode)AddAttributeLists((PropertyDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
                case SyntaxKind.Parameter:
                    return (T)(SyntaxNode)AddAttributeLists((ParameterSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
                case SyntaxKind.OperatorDeclaration:
                    return (T)(SyntaxNode)AddAttributeLists((OperatorDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
                case SyntaxKind.MethodDeclaration:
                    return (T)(SyntaxNode)AddAttributeLists((MethodDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
                case SyntaxKind.InterfaceDeclaration:
                    return (T)(SyntaxNode)AddAttributeLists((InterfaceDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
                case SyntaxKind.IndexerDeclaration:
                    return (T)(SyntaxNode)AddAttributeLists((IndexerDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
                case SyntaxKind.FieldDeclaration:
                    return (T)(SyntaxNode)AddAttributeLists((FieldDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
                case SyntaxKind.EventFieldDeclaration:
                    return (T)(SyntaxNode)AddAttributeLists((EventFieldDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
                case SyntaxKind.EventDeclaration:
                    return (T)(SyntaxNode)AddAttributeLists((EventDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
                case SyntaxKind.EnumMemberDeclaration:
                    return (T)(SyntaxNode)AddAttributeLists((EnumMemberDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
                case SyntaxKind.DestructorDeclaration:
                    return (T)(SyntaxNode)AddAttributeLists((DestructorDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
                case SyntaxKind.ConversionOperatorDeclaration:
                    return (T)(SyntaxNode)AddAttributeLists((ConversionOperatorDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
                case SyntaxKind.ConstructorDeclaration:
                    return (T)(SyntaxNode)AddAttributeLists((ConstructorDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
                case SyntaxKind.IncompleteMember:
                    return (T)(SyntaxNode)AddAttributeLists((IncompleteMemberSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                    return (T)(SyntaxNode)AddAttributeLists((AccessorDeclarationSyntax)(SyntaxNode)node, keepDocumentationCommentOnTop, attributeLists, f => f.AttributeLists.Any(), (f, g) => f.WithAttributeLists(g), (f, g) => f.AddAttributeLists(g));
                default:
                    throw new ArgumentException($"Cannot add attribute list to '{node.Kind()}'.", nameof(node));
            }
        }

        private static T AddAttributeLists<T>(
            this T node,
            bool keepDocumentationCommentOnTop,
            AttributeListSyntax[] attributeLists,
            Func<T, bool> hasAttributeLists,
            Func<T, SyntaxList<AttributeListSyntax>, T> withAttributeLists,
            Func<T, AttributeListSyntax[], T> addAttributeLists) where T : SyntaxNode
        {
            if (keepDocumentationCommentOnTop
                && !hasAttributeLists(node)
                && attributeLists.Length > 0)
            {
                SyntaxTriviaList leadingTrivia = node.GetLeadingTrivia();

                for (int i = 0; i < leadingTrivia.Count; i++)
                {
                    if (leadingTrivia[i].IsDocumentationCommentTrivia())
                    {
                        attributeLists[0] = attributeLists[0].PrependToLeadingTrivia(leadingTrivia.Take(i + 1));

                        node = node.WithLeadingTrivia(leadingTrivia.Skip(i + 1));

                        return withAttributeLists(node, SyntaxFactory.List(attributeLists));
                    }
                }
            }

            return addAttributeLists(node, attributeLists);
        }
    }
}
