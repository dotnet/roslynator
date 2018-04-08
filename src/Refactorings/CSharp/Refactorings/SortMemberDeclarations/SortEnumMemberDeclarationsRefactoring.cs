// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Comparers;

namespace Roslynator.CSharp.Refactorings.SortMemberDeclarations
{
    internal static class SortEnumMemberDeclarationsRefactoring
    {
        public static async Task ComputeRefactoringAsync(
            RefactoringContext context,
            EnumDeclarationSyntax enumDeclaration,
            SeparatedSyntaxListSelection<EnumMemberDeclarationSyntax> selectedMembers)
        {
            if (!selectedMembers.IsSorted(EnumMemberDeclarationNameComparer.Instance))
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
            SeparatedSyntaxListSelection<EnumMemberDeclarationSyntax> selectedMembers,
            CancellationToken cancellationToken)
        {
            IEnumerable<EnumMemberDeclarationSyntax> sorted = selectedMembers.OrderBy(f => f, EnumMemberDeclarationNameComparer.Instance);

            SeparatedSyntaxList<EnumMemberDeclarationSyntax> newMembers = enumDeclaration
                .Members
                .ReplaceRangeAt(selectedMembers.FirstIndex, selectedMembers.Count, sorted);

            MemberDeclarationSyntax newNode = enumDeclaration.WithMembers(newMembers);

            return document.ReplaceNodeAsync(enumDeclaration, newNode, cancellationToken);
        }

        private static async Task<Document> SortByValueAsync(
            Document document,
            EnumDeclarationSyntax enumDeclaration,
            SeparatedSyntaxListSelection<EnumMemberDeclarationSyntax> selectedMembers,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            SpecialType enumSpecialType = semanticModel.GetDeclaredSymbol(enumDeclaration, cancellationToken).EnumUnderlyingType.SpecialType;

            var comparer = new EnumMemberDeclarationValueComparer(EnumValueComparer.GetInstance(enumSpecialType), semanticModel, cancellationToken);

            IEnumerable<EnumMemberDeclarationSyntax> sorted = selectedMembers.OrderBy(f => f, comparer);

            SeparatedSyntaxList<EnumMemberDeclarationSyntax> newMembers = enumDeclaration
                .Members
                .ReplaceRangeAt(selectedMembers.FirstIndex, selectedMembers.Count, sorted);

            MemberDeclarationSyntax newNode = enumDeclaration.WithMembers(newMembers);

            return await document.ReplaceNodeAsync(enumDeclaration, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}