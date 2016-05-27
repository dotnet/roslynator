// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class CSharpSyntaxNodeExtensions
    {
        public static SyntaxTriviaList GetLeadingAndTrailingTrivia(this CSharpSyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            return node
                .GetLeadingTrivia()
                .AddRange(node.GetTrailingTrivia());
        }

        public static SyntaxList<AttributeListSyntax> GetAttributeLists(this CSharpSyntaxNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            switch (node.Kind())
            {
                case SyntaxKind.EnumDeclaration:
                    return ((EnumDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.DelegateDeclaration:
                    return ((DelegateDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.TypeParameter:
                    return ((TypeParameterSyntax)node).AttributeLists;
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.Parameter:
                    return ((ParameterSyntax)node).AttributeLists;
                case SyntaxKind.OperatorDeclaration:
                    return ((OperatorDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.IndexerDeclaration:
                    return ((IndexerDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.FieldDeclaration:
                    return ((FieldDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.EventFieldDeclaration:
                    return ((EventFieldDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.EventDeclaration:
                    return ((EventDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.EnumMemberDeclaration:
                    return ((EnumMemberDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.DestructorDeclaration:
                    return ((DestructorDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.ConversionOperatorDeclaration:
                    return ((ConversionOperatorDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.ConstructorDeclaration:
                    return ((ConstructorDeclarationSyntax)node).AttributeLists;
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                    return ((AccessorDeclarationSyntax)node).AttributeLists;
            }

            return default(SyntaxList<AttributeListSyntax>);
        }

        public static CSharpSyntaxNode SetAttributeLists(this CSharpSyntaxNode node, SyntaxList<AttributeListSyntax> attributeLists)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            switch (node.Kind())
            {
                case SyntaxKind.EnumDeclaration:
                    return ((EnumDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.DelegateDeclaration:
                    return ((DelegateDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.TypeParameter:
                    return ((TypeParameterSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.PropertyDeclaration:
                    return ((PropertyDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.Parameter:
                    return ((ParameterSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.OperatorDeclaration:
                    return ((OperatorDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.MethodDeclaration:
                    return ((MethodDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.IndexerDeclaration:
                    return ((IndexerDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.FieldDeclaration:
                    return ((FieldDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.EventFieldDeclaration:
                    return ((EventFieldDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.EventDeclaration:
                    return ((EventDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.EnumMemberDeclaration:
                    return ((EnumMemberDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.DestructorDeclaration:
                    return ((DestructorDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.ConversionOperatorDeclaration:
                    return ((ConversionOperatorDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.ConstructorDeclaration:
                    return ((ConstructorDeclarationSyntax)node).WithAttributeLists(attributeLists);
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                    return ((AccessorDeclarationSyntax)node).WithAttributeLists(attributeLists);
            }

            return node;
        }
    }
}
