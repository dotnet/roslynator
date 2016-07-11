// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class ClassDeclarationSyntaxExtensions
    {
        public static ClassDeclarationSyntax WithModifiers(
            this ClassDeclarationSyntax classDeclaration,
            params SyntaxKind[] tokenKinds)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            return classDeclaration.WithModifiers(CSharpFactory.TokenList(tokenKinds));
        }

        public static ClassDeclarationSyntax WithMembers(
            this ClassDeclarationSyntax classDeclaration,
            IEnumerable<MemberDeclarationSyntax> memberDeclarations)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            return classDeclaration.WithMembers(List(memberDeclarations));
        }

        public static ClassDeclarationSyntax WithMember(
            this ClassDeclarationSyntax classDeclaration,
            MemberDeclarationSyntax memberDeclaration)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            return classDeclaration.WithMembers(SingletonList(memberDeclaration));
        }

        public static TextSpan HeaderSpan(this ClassDeclarationSyntax classDeclaration)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            return TextSpan.FromBounds(
                classDeclaration.Span.Start,
                classDeclaration.Identifier.Span.End);
        }

        public static MemberDeclarationSyntax RemoveMemberAt(this ClassDeclarationSyntax declaration, int index)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            return RemoveMember(declaration, declaration.Members[index], index);
        }

        public static MemberDeclarationSyntax RemoveMember(this ClassDeclarationSyntax declaration, MemberDeclarationSyntax member)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return RemoveMember(declaration, member, declaration.Members.IndexOf(member));
        }

        private static MemberDeclarationSyntax RemoveMember(
            ClassDeclarationSyntax declaration,
            MemberDeclarationSyntax member,
            int index)
        {
            MemberDeclarationSyntax newMember = member.RemoveSingleLineDocumentationComment();

            declaration = declaration
                .WithMembers(declaration.Members.Replace(member, newMember));

            return declaration
                .RemoveNode(declaration.Members[index], RemoveMemberDeclarationRefactoring.GetRemoveOptions(newMember));
        }

        public static ClassDeclarationSyntax WithoutSemicolonToken(this ClassDeclarationSyntax classDeclaration)
        {
            if (classDeclaration == null)
                throw new ArgumentNullException(nameof(classDeclaration));

            return classDeclaration.WithSemicolonToken(CSharpFactory.NoneToken());
        }
    }
}
