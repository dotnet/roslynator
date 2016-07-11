// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp
{
    public static class MemberDeclarationSyntaxExtensions
    {
        public static MemberDeclarationSyntax RemoveSingleLineDocumentationComment(this MemberDeclarationSyntax member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

            SyntaxTriviaList triviaList = member.GetLeadingTrivia();

            SyntaxTriviaList.Reversed.Enumerator en = triviaList.Reverse().GetEnumerator();

            int i = 0;
            while (en.MoveNext())
            {
                if (en.Current.IsWhitespaceOrEndOfLineTrivia())
                {
                    i++;
                }
                else if (en.Current.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                {
                    return member.WithLeadingTrivia(triviaList.Take(triviaList.Count - (i + 1)));
                }
                else
                {
                    return member;
                }
            }

            return member;
        }

        public static MemberDeclarationSyntax RemoveMemberAt(this MemberDeclarationSyntax declaration, int index)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            switch (declaration.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                    return ((NamespaceDeclarationSyntax)declaration).RemoveMemberAt(index);
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)declaration).RemoveMemberAt(index);
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)declaration).RemoveMemberAt(index);
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)declaration).RemoveMemberAt(index);
            }

            return declaration;
        }

        public static MemberDeclarationSyntax RemoveMember(this MemberDeclarationSyntax declaration, MemberDeclarationSyntax member)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            switch (declaration.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                    return ((NamespaceDeclarationSyntax)declaration).RemoveMember(member);
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)declaration).RemoveMember(member);
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)declaration).RemoveMember(member);
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)declaration).RemoveMember(member);
            }

            return declaration;
        }

        public static MemberDeclarationSyntax GetMember(this MemberDeclarationSyntax declaration, int index)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            return GetMembers(declaration)[index];
        }

        public static SyntaxList<MemberDeclarationSyntax> GetMembers(this MemberDeclarationSyntax declaration)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            switch (declaration.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                    return ((NamespaceDeclarationSyntax)declaration).Members;
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)declaration).Members;
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)declaration).Members;
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)declaration).Members;
                default:
                    return default(SyntaxList<MemberDeclarationSyntax>);
            }
        }

        public static MemberDeclarationSyntax SetMembers(this MemberDeclarationSyntax declaration, SyntaxList<MemberDeclarationSyntax> newMembers)
        {
            if (declaration == null)
                throw new ArgumentNullException(nameof(declaration));

            switch (declaration.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                    return ((NamespaceDeclarationSyntax)declaration).WithMembers(newMembers);
                case SyntaxKind.ClassDeclaration:
                    return ((ClassDeclarationSyntax)declaration).WithMembers(newMembers);
                case SyntaxKind.StructDeclaration:
                    return ((StructDeclarationSyntax)declaration).WithMembers(newMembers);
                case SyntaxKind.InterfaceDeclaration:
                    return ((InterfaceDeclarationSyntax)declaration).WithMembers(newMembers);
                default:
                    return declaration;
            }
        }
    }
}
