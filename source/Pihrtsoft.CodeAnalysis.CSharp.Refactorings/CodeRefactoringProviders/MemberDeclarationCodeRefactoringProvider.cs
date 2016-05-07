// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.CodeRefactoringProviders
{
    [ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(MemberDeclarationCodeRefactoringProvider))]
    public class MemberDeclarationCodeRefactoringProvider : CodeRefactoringProvider
    {
        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            SyntaxNode root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

            MemberDeclarationSyntax memberDeclaration = root
                .FindNode(context.Span, getInnermostNodeForTie: true)?
                .FirstAncestorOrSelf<MemberDeclarationSyntax>();

            if (memberDeclaration == null)
                return;

            DeleteMember(context, memberDeclaration);

            DuplicateMember(context, memberDeclaration);
        }

        private static void DeleteMember(CodeRefactoringContext context, MemberDeclarationSyntax memberDeclaration)
        {
            if (CanBeDeletedOrDuplicated(memberDeclaration))
            {
                context.RegisterRefactoring(
                    "Remove " + SyntaxHelper.GetSyntaxNodeName(memberDeclaration),
                    cancellationToken => DeleteMemberAsync(context.Document, memberDeclaration, cancellationToken));
            }
        }

        private static void DuplicateMember(CodeRefactoringContext context, MemberDeclarationSyntax memberDeclaration)
        {
            if (memberDeclaration.Parent == null)
                return;

            if (!memberDeclaration.Parent.IsAnyKind(SyntaxKind.ClassDeclaration, SyntaxKind.StructDeclaration))
                return;

            if (CanBeDeletedOrDuplicated(memberDeclaration))
            {
                context.RegisterRefactoring(
                    "Duplicate " + SyntaxHelper.GetSyntaxNodeName(memberDeclaration),
                    cancellationToken => DuplicateMemberAsync(context.Document, memberDeclaration, cancellationToken));
            }
        }

        private static async Task<Document> DeleteMemberAsync(
            Document document,
            MemberDeclarationSyntax memberDeclaration,
            CancellationToken cancellationToken)
        {
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

        private static async Task<Document> DuplicateMemberAsync(
            Document document,
            MemberDeclarationSyntax memberDeclaration,
            CancellationToken cancellationToken)
        {
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
    }
}