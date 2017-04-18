// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class DuplicateMemberDeclarationRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax member,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            return document.ReplaceNodeAsync(member.Parent, Refactor(member), cancellationToken);
        }

        private static SyntaxNode Refactor(MemberDeclarationSyntax member)
        {
            switch (member.Parent.Kind())
            {
                case SyntaxKind.CompilationUnit:
                    {
                        var parent = (CompilationUnitSyntax)member.Parent;
                        SyntaxList<MemberDeclarationSyntax> members = parent.Members;
                        int index = members.IndexOf(member);

                        return parent.WithMembers(members.Insert(index + 1, member));
                    }
                case SyntaxKind.NamespaceDeclaration:
                    {
                        var parent = (NamespaceDeclarationSyntax)member.Parent;
                        SyntaxList<MemberDeclarationSyntax> members = parent.Members;
                        int index = members.IndexOf(member);

                        if (index == 0
                            && parent.OpenBraceToken.GetFullSpanEndLine() == member.GetFullSpanStartLine())
                        {
                            member = member.WithLeadingTrivia(member.GetLeadingTrivia().Insert(0, NewLine()));
                        }

                        return parent.WithMembers(members.Insert(index + 1, member));
                    }
                case SyntaxKind.ClassDeclaration:
                    {
                        var parent = (ClassDeclarationSyntax)member.Parent;
                        SyntaxList<MemberDeclarationSyntax> members = parent.Members;
                        int index = members.IndexOf(member);

                        if (index == 0
                            && parent.OpenBraceToken.GetFullSpanEndLine() == member.GetFullSpanStartLine())
                        {
                            member = member.WithLeadingTrivia(member.GetLeadingTrivia().Insert(0, NewLine()));
                        }

                        return parent.WithMembers(members.Insert(index + 1, member));
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        var parent = (StructDeclarationSyntax)member.Parent;
                        SyntaxList<MemberDeclarationSyntax> members = parent.Members;
                        int index = members.IndexOf(member);

                        if (index == 0
                            && parent.OpenBraceToken.GetFullSpanEndLine() == member.GetFullSpanStartLine())
                        {
                            member = member.WithLeadingTrivia(member.GetLeadingTrivia().Insert(0, NewLine()));
                        }

                        return parent.WithMembers(members.Insert(index + 1, member));
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        var parent = (InterfaceDeclarationSyntax)member.Parent;
                        SyntaxList<MemberDeclarationSyntax> members = parent.Members;
                        int index = members.IndexOf(member);

                        if (index == 0
                            && parent.OpenBraceToken.GetFullSpanEndLine() == member.GetFullSpanStartLine())
                        {
                            member = member.WithLeadingTrivia(member.GetLeadingTrivia().Insert(0, NewLine()));
                        }

                        return parent.WithMembers(members.Insert(index + 1, member));
                    }
            }

            return null;
        }
    }
}
