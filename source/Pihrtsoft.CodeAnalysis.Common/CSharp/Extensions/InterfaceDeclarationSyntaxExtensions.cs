// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Pihrtsoft.CodeAnalysis.CSharp.Refactoring;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class InterfaceDeclarationSyntaxExtensions
    {
        public static TextSpan HeaderSpan(this InterfaceDeclarationSyntax interfaceDeclaration)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            return TextSpan.FromBounds(
                interfaceDeclaration.Span.Start,
                interfaceDeclaration.Identifier.Span.End);
        }

        public static MemberDeclarationSyntax RemoveMemberAt(this InterfaceDeclarationSyntax declaration, int index)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            return RemoveMember(declaration, declaration.Members[index], index);
        }

        public static MemberDeclarationSyntax RemoveMember(this InterfaceDeclarationSyntax declaration, MemberDeclarationSyntax member)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return RemoveMember(declaration, member, declaration.Members.IndexOf(member));
        }

        private static MemberDeclarationSyntax RemoveMember(
            InterfaceDeclarationSyntax declaration,
            MemberDeclarationSyntax member,
            int index)
        {
            MemberDeclarationSyntax newMember = member.RemoveSingleLineDocumentationComment();

            declaration = declaration
                .WithMembers(declaration.Members.Replace(member, newMember));

            return declaration
                .RemoveNode(declaration.Members[index], RemoveMemberDeclarationRefactoring.GetRemoveOptions(newMember));
        }

        public static InterfaceDeclarationSyntax WithoutSemicolonToken(this InterfaceDeclarationSyntax interfaceDeclaration)
        {
            if (interfaceDeclaration == null)
                throw new ArgumentNullException(nameof(interfaceDeclaration));

            return interfaceDeclaration.WithSemicolonToken(CSharpFactory.NoneToken());
        }
    }
}
