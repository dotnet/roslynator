// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    public static class MemberDeclarationRefactoring
    {
        public static SyntaxRemoveOptions DefaultRemoveOptions { get; }
            = SyntaxRemoveOptions.KeepExteriorTrivia | SyntaxRemoveOptions.KeepUnbalancedDirectives;

        public static bool CanBeRemoved(CodeRefactoringContext context, MemberDeclarationSyntax memberDeclaration)
        {
            if (memberDeclaration == null)
                throw new ArgumentNullException(nameof(memberDeclaration));

            return CanBeRemovedOrDuplicated(context, memberDeclaration);
        }

        public static bool CanBeDuplicated(CodeRefactoringContext context, MemberDeclarationSyntax memberDeclaration)
        {
            if (memberDeclaration == null)
                throw new ArgumentNullException(nameof(memberDeclaration));

            return CanBeRemovedOrDuplicated(context, memberDeclaration);
        }

        private static bool CanBeRemovedOrDuplicated(CodeRefactoringContext context, MemberDeclarationSyntax member)
        {
            if (member.Parent?.IsAnyKind(
                    SyntaxKind.NamespaceDeclaration,
                    SyntaxKind.ClassDeclaration,
                    SyntaxKind.StructDeclaration,
                    SyntaxKind.InterfaceDeclaration) != true)
            {
                return false;
            }

            switch (member.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                    {
                        var declaration = (MethodDeclarationSyntax)member;

                        return declaration.Body?.CloseBraceToken.Span.Contains(context.Span) == true
                            || declaration.HeaderSpan().Contains(context.Span);
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        var declaration = (IndexerDeclarationSyntax)member;

                        return declaration.AccessorList?.CloseBraceToken.Span.Contains(context.Span) == true
                            || declaration.HeaderSpan().Contains(context.Span);
                    }
                case SyntaxKind.OperatorDeclaration:
                    {
                        var declaration = (OperatorDeclarationSyntax)member;

                        return declaration.Body?.CloseBraceToken.Span.Contains(context.Span) == true
                            || declaration.HeaderSpan().Contains(context.Span);
                    }
                case SyntaxKind.ConversionOperatorDeclaration:
                    {
                        var declaration = (ConversionOperatorDeclarationSyntax)member;

                        return declaration.Body?.CloseBraceToken.Span.Contains(context.Span) == true
                            || declaration.HeaderSpan().Contains(context.Span);
                    }
                case SyntaxKind.ConstructorDeclaration:
                    {
                        var declaration = (ConstructorDeclarationSyntax)member;

                        return declaration.Body?.CloseBraceToken.Span.Contains(context.Span) == true
                            || declaration.HeaderSpan().Contains(context.Span);
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        var declaration = (PropertyDeclarationSyntax)member;

                        return declaration.AccessorList?.CloseBraceToken.Span.Contains(context.Span) == true
                            || declaration.HeaderSpan().Contains(context.Span);
                    }
                case SyntaxKind.EventDeclaration:
                    {
                        var declaration = (EventDeclarationSyntax)member;

                        return declaration.AccessorList?.CloseBraceToken.Span.Contains(context.Span) == true
                            || declaration.HeaderSpan().Contains(context.Span);
                    }
                case SyntaxKind.NamespaceDeclaration:
                    {
                        var declaration = (NamespaceDeclarationSyntax)member;

                        return declaration.CloseBraceToken.Span.Contains(context.Span)
                            || declaration.HeaderSpan().Contains(context.Span);
                    }
                case SyntaxKind.ClassDeclaration:
                    {
                        var declaration = (ClassDeclarationSyntax)member;

                        return declaration.CloseBraceToken.Span.Contains(context.Span)
                            || declaration.HeaderSpan().Contains(context.Span);
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        var declaration = (StructDeclarationSyntax)member;

                        return declaration.CloseBraceToken.Span.Contains(context.Span)
                            || declaration.HeaderSpan().Contains(context.Span);
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        var declaration = (InterfaceDeclarationSyntax)member;

                        return declaration.CloseBraceToken.Span.Contains(context.Span)
                            || declaration.HeaderSpan().Contains(context.Span);
                    }
            }

            return false;
        }

        public static async Task<Document> RemoveMemberAsync(
            Document document,
            MemberDeclarationSyntax member,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            var parentMember = (MemberDeclarationSyntax)member.Parent;

            if (parentMember != null)
            {
                root = root.ReplaceNode(parentMember, parentMember.RemoveMember(member));
            }
            else
            {
                root = root.RemoveNode(member, DefaultRemoveOptions);
            }

            return document.WithSyntaxRoot(root);
        }

        internal static SyntaxRemoveOptions GetRemoveOptions(MemberDeclarationSyntax newMember)
        {
            SyntaxRemoveOptions removeOptions = DefaultRemoveOptions;

            if (newMember.GetLeadingTrivia().IsWhitespaceOrEndOfLine())
                removeOptions &= ~SyntaxRemoveOptions.KeepLeadingTrivia;

            if (newMember.GetTrailingTrivia().IsWhitespaceOrEndOfLine())
                removeOptions &= ~SyntaxRemoveOptions.KeepTrailingTrivia;

            return removeOptions;
        }

        public static async Task<Document> DuplicateMemberAsync(
            Document document,
            MemberDeclarationSyntax member,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (member == null)
                throw new ArgumentNullException(nameof(member));

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            root = root.ReplaceNode(member.Parent, DuplicateMember(member));

            return document.WithSyntaxRoot(root);
        }

        private static SyntaxNode DuplicateMember(MemberDeclarationSyntax member)
        {
            switch (member.Parent.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                    {
                        var parent = (NamespaceDeclarationSyntax)member.Parent;
                        int index = parent.Members.IndexOf(member);
                        return parent.WithMembers(parent.Members.Insert(index, member));
                    }
                case SyntaxKind.ClassDeclaration:
                    {
                        var parent = (ClassDeclarationSyntax)member.Parent;
                        int index = parent.Members.IndexOf(member);
                        return parent.WithMembers(parent.Members.Insert(index, member));
                    }
                case SyntaxKind.StructDeclaration:
                    {
                        var parent = (StructDeclarationSyntax)member.Parent;
                        int index = parent.Members.IndexOf(member);
                        return parent.WithMembers(parent.Members.Insert(index, member));
                    }
                case SyntaxKind.InterfaceDeclaration:
                    {
                        var parent = (InterfaceDeclarationSyntax)member.Parent;
                        int index = parent.Members.IndexOf(member);
                        return parent.WithMembers(parent.Members.Insert(index, member));
                    }
            }

            return null;
        }
    }
}
