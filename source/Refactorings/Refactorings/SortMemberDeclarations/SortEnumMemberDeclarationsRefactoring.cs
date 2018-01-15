// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Comparers;

namespace Roslynator.CSharp.Refactorings.SortMemberDeclarations
{
    internal static class SortEnumMemberDeclarationsRefactoring
    {
        public static async Task ComputeRefactoringAsync(
            RefactoringContext context,
            EnumDeclarationSyntax enumDeclaration,
            SeparatedSyntaxListSelection<EnumMemberDeclarationSyntax> selection)
        {
            ImmutableArray<EnumMemberDeclarationSyntax> selectedMembers = selection.SelectedItems;

            if (!EnumMemberDeclarationNameComparer.IsSorted(selectedMembers))
            {
                context.RegisterRefactoring(
                    "Sort enum members by name",
                    cancellationToken => SortByNameAsync(context.Document, enumDeclaration, selectedMembers, cancellationToken));
            }

            if (selectedMembers.Any(f => f.EqualsValue?.Value != null))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                if (!EnumMemberDeclarationValueComparer.IsSorted(selectedMembers, semanticModel, context.CancellationToken))
                {
                    context.RegisterRefactoring(
                        "Sort enum members by value",
                        cancellationToken => SortByValueAsync(context.Document, enumDeclaration, selectedMembers, cancellationToken));
                }
            }
        }

        private static Task<Document> SortByNameAsync(
            Document document,
            EnumDeclarationSyntax enumDeclaration,
            ImmutableArray<EnumMemberDeclarationSyntax> selectedMembers,
            CancellationToken cancellationToken)
        {
            SeparatedSyntaxList<EnumMemberDeclarationSyntax> members = enumDeclaration.Members;

            int firstIndex = members.IndexOf(selectedMembers[0]);
            int lastIndex = members.IndexOf(selectedMembers[selectedMembers.Length - 1]);

            SeparatedSyntaxList<EnumMemberDeclarationSyntax> newMembers = members
                .Take(firstIndex)
                .Concat(selectedMembers.OrderBy(f => f, EnumMemberDeclarationNameComparer.Instance))
                .Concat(members.Skip(lastIndex + 1))
                .ToSeparatedSyntaxList();

            MemberDeclarationSyntax newNode = enumDeclaration.WithMembers(newMembers);

            return document.ReplaceNodeAsync(enumDeclaration, newNode, cancellationToken);
        }

        private static async Task<Document> SortByValueAsync(
            Document document,
            EnumDeclarationSyntax enumDeclaration,
            ImmutableArray<EnumMemberDeclarationSyntax> selectedMembers,
            CancellationToken cancellationToken)
        {
            SeparatedSyntaxList<EnumMemberDeclarationSyntax> members = enumDeclaration.Members;

            int firstIndex = members.IndexOf(selectedMembers[0]);
            int lastIndex = members.IndexOf(selectedMembers[selectedMembers.Length - 1]);

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            var comparer = new EnumMemberDeclarationValueComparer(semanticModel, cancellationToken);

            IEnumerable<EnumMemberDeclarationSyntax> sorted = selectedMembers.OrderBy(f => f, comparer);

            SeparatedSyntaxList<EnumMemberDeclarationSyntax> newMembers = members
                .Take(firstIndex)
                .Concat(sorted)
                .Concat(members.Skip(lastIndex + 1))
                .ToSeparatedSyntaxList();

            MemberDeclarationSyntax newNode = enumDeclaration.WithMembers(newMembers);

            return await document.ReplaceNodeAsync(enumDeclaration, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}