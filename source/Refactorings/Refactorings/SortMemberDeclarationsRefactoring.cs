// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SortMemberDeclarationsRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, NamespaceDeclarationSyntax namespaceDeclaration)
        {
            ComputeRefactoring(context, SelectedMemberDeclarationCollection.Create(namespaceDeclaration, context.Span));
        }

        public static void ComputeRefactoring(RefactoringContext context, ClassDeclarationSyntax classDeclaration)
        {
            ComputeRefactoring(context, SelectedMemberDeclarationCollection.Create(classDeclaration, context.Span));
        }

        public static void ComputeRefactoring(RefactoringContext context, StructDeclarationSyntax structDeclaration)
        {
            ComputeRefactoring(context, SelectedMemberDeclarationCollection.Create(structDeclaration, context.Span));
        }

        public static void ComputeRefactoring(RefactoringContext context, InterfaceDeclarationSyntax interfaceDeclaration)
        {
            ComputeRefactoring(context, SelectedMemberDeclarationCollection.Create(interfaceDeclaration, context.Span));
        }

        private static void ComputeRefactoring(RefactoringContext context, SelectedMemberDeclarationCollection selectedMembers)
        {
            if (selectedMembers.IsMultiple)
            {
                ImmutableArray<MemberDeclarationSyntax> selectedMemberArray = selectedMembers.ToImmutableArray();

                SyntaxKind kind = GetSingleKindOrDefault(selectedMemberArray);

                if (kind != SyntaxKind.None)
                {
                    if (MemberDeclarationComparer.CanBeSortedAlphabetically(kind))
                    {
                        ComputeRefactoring(
                            context,
                            MemberDeclarationSortMode.ByKindThenByName,
                            "Sort members by name",
                            selectedMembers,
                            selectedMemberArray);
                    }
                }
                else
                {
                    ComputeRefactoring(
                        context,
                        MemberDeclarationSortMode.ByKind,
                        "Sort members by kind",
                        selectedMembers,
                        selectedMemberArray);

                    ComputeRefactoring(
                        context,
                        MemberDeclarationSortMode.ByKindThenByName,
                        "Sort members by kind then by name",
                        selectedMembers,
                        selectedMemberArray);
                }
            }
        }

        private static void ComputeRefactoring(
            RefactoringContext context,
            MemberDeclarationSortMode sortMode,
            string title,
            SelectedMemberDeclarationCollection selectedMembers,
            ImmutableArray<MemberDeclarationSyntax> selectedMemberArray)
        {
            if (!MemberDeclarationComparer.IsListSorted(selectedMemberArray, sortMode))
            {
                context.RegisterRefactoring(
                    title,
                    cancellationToken => RefactorAsync(context.Document, selectedMembers, sortMode, cancellationToken));
            }
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            SelectedMemberDeclarationCollection selectedMembers,
            MemberDeclarationSortMode sortMode,
            CancellationToken cancellationToken)
        {
            var comparer = new MemberDeclarationComparer(sortMode);

            MemberDeclarationSyntax containingMember = selectedMembers.ContainingMember;

            SyntaxList<MemberDeclarationSyntax> members = containingMember.GetMembers();

            SyntaxList<MemberDeclarationSyntax> newMembers = members
                .Take(selectedMembers.FirstIndex)
                .Concat(selectedMembers.OrderBy(f => f, comparer))
                .Concat(members.Skip(selectedMembers.LastIndex + 1))
                .ToSyntaxList();

            MemberDeclarationSyntax newNode = containingMember.SetMembers(newMembers);

            return await document.ReplaceNodeAsync(containingMember, newNode, cancellationToken).ConfigureAwait(false);
        }

        private static SyntaxKind GetSingleKindOrDefault(ImmutableArray<MemberDeclarationSyntax> members)
        {
            SyntaxKind kind = members.First().Kind();

            for (int i = 1; i < members.Length; i++)
            {
                if (members[i].Kind() != kind)
                    return SyntaxKind.None;
            }

            return kind;
        }
    }
}