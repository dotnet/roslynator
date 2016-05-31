// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    public static class MemberDeclarationRefactoring
    {
        public static SyntaxRemoveOptions DefaultRemoveOptions { get; }
            = SyntaxRemoveOptions.KeepExteriorTrivia | SyntaxRemoveOptions.KeepUnbalancedDirectives;

        public static bool CanBeRemoved(MemberDeclarationSyntax memberDeclaration)
        {
            if (memberDeclaration == null)
                throw new ArgumentNullException(nameof(memberDeclaration));

            return CanBeDeletedOrDuplicated(memberDeclaration);
        }

        public static bool CanBeDuplicated(MemberDeclarationSyntax memberDeclaration)
        {
            if (memberDeclaration == null)
                throw new ArgumentNullException(nameof(memberDeclaration));

            return memberDeclaration.Parent != null
                && memberDeclaration.Parent.IsAnyKind(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration)
                && CanBeDeletedOrDuplicated(memberDeclaration);
        }

        private static bool CanBeDeletedOrDuplicated(MemberDeclarationSyntax memberDeclaration)
        {
            switch (memberDeclaration.Kind())
            {
                case SyntaxKind.MethodDeclaration:
                case SyntaxKind.IndexerDeclaration:
                case SyntaxKind.OperatorDeclaration:
                case SyntaxKind.ConversionOperatorDeclaration:
                case SyntaxKind.ConstructorDeclaration:
                case SyntaxKind.PropertyDeclaration:
                case SyntaxKind.FieldDeclaration:
                case SyntaxKind.EventDeclaration:
                case SyntaxKind.EventFieldDeclaration:
                    return true;
                default:
                    return false;
            }
        }

        public static async Task<Document> RemoveAsync(
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

        public static async Task<Document> DuplicateAsync(
            Document document,
            MemberDeclarationSyntax memberDeclaration,
            CancellationToken cancellationToken)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (memberDeclaration == null)
                throw new ArgumentNullException(nameof(memberDeclaration));

            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            SyntaxNode newRoot = oldRoot.ReplaceNode(memberDeclaration.Parent, GetNewNode(memberDeclaration));

            return document.WithSyntaxRoot(newRoot);
        }

        private static SyntaxNode GetNewNode(MemberDeclarationSyntax memberDeclaration)
        {
            switch (memberDeclaration.Parent.Kind())
            {
                case SyntaxKind.ClassDeclaration:
                    {
                        var parent = (ClassDeclarationSyntax)memberDeclaration.Parent;
                        int index = parent.Members.IndexOf(memberDeclaration);
                        return parent.WithMembers(parent.Members.Insert(index, memberDeclaration));
                    }

                case SyntaxKind.StructDeclaration:
                    {
                        var parent = (StructDeclarationSyntax)memberDeclaration.Parent;
                        int index = parent.Members.IndexOf(memberDeclaration);
                        return parent.WithMembers(parent.Members.Insert(index, memberDeclaration));
                    }
            }

            return null;
        }
    }
}
