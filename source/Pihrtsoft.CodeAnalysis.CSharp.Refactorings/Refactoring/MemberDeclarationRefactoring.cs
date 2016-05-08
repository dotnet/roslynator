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

        public static void Remove(CodeRefactoringContext context, MemberDeclarationSyntax memberDeclaration)
        {
            if (CanBeRemoved(memberDeclaration))
            {
                context.RegisterRefactoring(
                    "Remove " + SyntaxHelper.GetSyntaxNodeName(memberDeclaration),
                    cancellationToken => RemoveAsync(context.Document, memberDeclaration, cancellationToken));
            }
        }

        public static async Task<Document> RemoveAsync(
            Document document,
            MemberDeclarationSyntax memberDeclaration,
            CancellationToken cancellationToken)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (memberDeclaration == null)
                throw new ArgumentNullException(nameof(memberDeclaration));

            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            SyntaxNode newRoot = oldRoot.RemoveNode(
                memberDeclaration,
                GetRemoveOptions(memberDeclaration.GetLeadingTrivia(), memberDeclaration.GetTrailingTrivia()));

            return document.WithSyntaxRoot(newRoot);
        }

        private static SyntaxRemoveOptions GetRemoveOptions(SyntaxTriviaList leading, SyntaxTriviaList trailing)
        {
            SyntaxRemoveOptions removeOptions = SyntaxRemoveOptions.KeepExteriorTrivia;

            if (leading.Count == 1
                && leading[0].IsKind(SyntaxKind.EndOfLineTrivia))
            {
                removeOptions &= ~SyntaxRemoveOptions.KeepLeadingTrivia;
            }
            else if (leading.Count == 2
                && leading[0].IsKind(SyntaxKind.EndOfLineTrivia)
                && leading[1].IsKind(SyntaxKind.WhitespaceTrivia))
            {
                removeOptions &= ~SyntaxRemoveOptions.KeepLeadingTrivia;
            }

            if (trailing.Count == 1
                && trailing[0].IsKind(SyntaxKind.EndOfLineTrivia))
            {
                removeOptions &= ~SyntaxRemoveOptions.KeepTrailingTrivia;
            }

            return removeOptions;
        }

        public static void Duplicate(CodeRefactoringContext context, MemberDeclarationSyntax memberDeclaration)
        {
            if (CanBeDuplicated(memberDeclaration))
            {
                context.RegisterRefactoring(
                    "Duplicate " + SyntaxHelper.GetSyntaxNodeName(memberDeclaration),
                    cancellationToken => DuplicateAsync(context.Document, memberDeclaration, cancellationToken));
            }
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
