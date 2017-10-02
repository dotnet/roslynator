// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Comparers;
using Roslynator.CSharp.SyntaxRewriters.SortMembers;

namespace Roslynator.CSharp.Refactorings.SortMemberDeclarations
{
    internal static class SortMemberDeclarationsRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, NamespaceDeclarationSyntax namespaceDeclaration)
        {
            MemberDeclarationSelection selectedMembers;
            if (MemberDeclarationSelection.TryCreate(namespaceDeclaration, context.Span, out selectedMembers))
                ComputeRefactoring(context, selectedMembers);
        }

        public static void ComputeRefactoring(RefactoringContext context, ClassDeclarationSyntax classDeclaration)
        {
            MemberDeclarationSelection selectedMembers;
            if (MemberDeclarationSelection.TryCreate(classDeclaration, context.Span, out selectedMembers))
                ComputeRefactoring(context, selectedMembers);
        }

        public static void ComputeRefactoring(RefactoringContext context, StructDeclarationSyntax structDeclaration)
        {
            MemberDeclarationSelection selectedMembers;
            if (MemberDeclarationSelection.TryCreate(structDeclaration, context.Span, out selectedMembers))
                ComputeRefactoring(context, selectedMembers);
        }

        public static void ComputeRefactoring(RefactoringContext context, InterfaceDeclarationSyntax interfaceDeclaration)
        {
            MemberDeclarationSelection selectedMembers;
            if (MemberDeclarationSelection.TryCreate(interfaceDeclaration, context.Span, out selectedMembers))
                ComputeRefactoring(context, selectedMembers);
        }

        private static void ComputeRefactoring(RefactoringContext context, MemberDeclarationSelection selectedMembers)
        {
            if (selectedMembers.Count > 1)
            {
                ImmutableArray<MemberDeclarationSyntax> members = selectedMembers.SelectedItems;

                SyntaxKind kind = GetSingleKindOrDefault(members);

                if (kind != SyntaxKind.None)
                {
                    if (MemberDeclarationComparer.CanBeSortedAlphabetically(kind))
                    {
                        ComputeRefactoring(
                            context,
                            MemberDeclarationSortMode.ByKindThenByName,
                            "Sort members by name",
                            selectedMembers,
                            members);
                    }
                }
                else
                {
                    ComputeRefactoring(
                        context,
                        MemberDeclarationSortMode.ByKind,
                        "Sort members by kind",
                        selectedMembers,
                        members);

                    ComputeRefactoring(
                        context,
                        MemberDeclarationSortMode.ByKindThenByName,
                        "Sort members by kind then by name",
                        selectedMembers,
                        members);
                }
            }
        }

        private static void ComputeRefactoring(
            RefactoringContext context,
            MemberDeclarationSortMode sortMode,
            string title,
            MemberDeclarationSelection selectedMembers,
            ImmutableArray<MemberDeclarationSyntax> members)
        {
            if (MemberDeclarationComparer.IsSorted(members, sortMode))
                return;

            context.RegisterRefactoring(
                title,
                cancellationToken => RefactorAsync(context.Document, selectedMembers, sortMode, cancellationToken));
        }

        private static Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSelection selectedMembers,
            MemberDeclarationSortMode sortMode,
            CancellationToken cancellationToken)
        {
            MemberDeclarationComparer comparer = MemberDeclarationComparer.GetInstance(sortMode);

            MemberDeclarationSyntax containingMember = selectedMembers.ContainingMember;

            SyntaxList<MemberDeclarationSyntax> members = containingMember.GetMembers();

            SyntaxList<MemberDeclarationSyntax> newMembers = members
                .Take(selectedMembers.StartIndex)
                .Concat(selectedMembers.OrderBy(f => f, comparer))
                .Concat(members.Skip(selectedMembers.EndIndex + 1))
                .ToSyntaxList();

            MemberDeclarationSyntax newNode = containingMember.WithMembers(newMembers);

            return document.ReplaceNodeAsync(containingMember, newNode, cancellationToken);
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            IComparer<MemberDeclarationSyntax> memberComparer,
            IComparer<EnumMemberDeclarationSyntax> enumMemberComparer,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            if (root.IsKind(SyntaxKind.CompilationUnit))
            {
                var rewriter = new SortMemberDeclarationsRewriter(memberComparer, enumMemberComparer);

                SyntaxNode newRoot = rewriter.VisitCompilationUnit((CompilationUnitSyntax)root);

                return document.WithSyntaxRoot(newRoot);
            }

            return document;
        }

        private static SyntaxKind GetSingleKindOrDefault(ImmutableArray<MemberDeclarationSyntax> members)
        {
            SyntaxKind kind = members[0].Kind();

            for (int i = 1; i < members.Length; i++)
            {
                if (members[i].Kind() != kind)
                    return SyntaxKind.None;
            }

            return kind;
        }
    }
}