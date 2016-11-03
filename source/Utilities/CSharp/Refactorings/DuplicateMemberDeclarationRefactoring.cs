// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    public static class DuplicateMemberDeclarationRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax member,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            root = root.ReplaceNode(member.Parent, Refactor(member));

            return document.WithSyntaxRoot(root);
        }

        private static SyntaxNode Refactor(MemberDeclarationSyntax member)
        {
            switch (member.Parent.Kind())
            {
                case SyntaxKind.CompilationUnit:
                    {
                        var parent = (CompilationUnitSyntax)member.Parent;
                        int index = parent.Members.IndexOf(member);

                        return parent.WithMembers(parent.Members.Insert(index + 1, member));
                    }
                case SyntaxKind.NamespaceDeclaration:
                    {
                        var parent = (NamespaceDeclarationSyntax)member.Parent;
                        int index = parent.Members.IndexOf(member);

                        if (index == 0
                            && parent.OpenBraceToken.GetFullSpanEndLine() == member.GetFullSpanStartLine())
                        {
                            member = member.WithLeadingTrivia(member.GetLeadingTrivia().Insert(0, CSharpFactory.NewLineTrivia()));
                        }

                        return parent.WithMembers(parent.Members.Insert(index + 1, member));
                    }
                case SyntaxKind.ClassDeclaration:
                    {
                        var parent = (ClassDeclarationSyntax)member.Parent;
                        int index = parent.Members.IndexOf(member);

                        if (index == 0
                            && parent.OpenBraceToken.GetFullSpanEndLine() == member.GetFullSpanStartLine())
                        {
                            member = member.WithLeadingTrivia(member.GetLeadingTrivia().Insert(0, CSharpFactory.NewLineTrivia()));
                        }

                        return parent.WithMembers(parent.Members.Insert(index + 1, member));
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        var parent = (StructDeclarationSyntax)member.Parent;
                        int index = parent.Members.IndexOf(member);

                        if (index == 0
                            && parent.OpenBraceToken.GetFullSpanEndLine() == member.GetFullSpanStartLine())
                        {
                            member = member.WithLeadingTrivia(member.GetLeadingTrivia().Insert(0, CSharpFactory.NewLineTrivia()));
                        }

                        return parent.WithMembers(parent.Members.Insert(index + 1, member));
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        var parent = (InterfaceDeclarationSyntax)member.Parent;
                        int index = parent.Members.IndexOf(member);

                        if (index == 0
                            && parent.OpenBraceToken.GetFullSpanEndLine() == member.GetFullSpanStartLine())
                        {
                            member = member.WithLeadingTrivia(member.GetLeadingTrivia().Insert(0, CSharpFactory.NewLineTrivia()));
                        }

                        return parent.WithMembers(parent.Members.Insert(index + 1, member));
                    }
            }

            return null;
        }
    }
}
