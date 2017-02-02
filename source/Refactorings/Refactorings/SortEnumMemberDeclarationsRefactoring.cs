// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SortEnumMemberDeclarationsRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, EnumDeclarationSyntax enumDeclaration)
        {
            ImmutableArray<EnumMemberDeclarationSyntax> selectedMembers = enumDeclaration.Members
                .SkipWhile(f => context.Span.Start > f.Span.Start)
                .TakeWhile(f => context.Span.End >= f.Span.End)
                .ToImmutableArray();

            if (selectedMembers.Length > 1)
            {
                if (!EnumMemberDeclarationComparer.IsListSorted(selectedMembers))
                {
                    context.RegisterRefactoring(
                        "Sort enum members by name",
                        cancellationToken => SortByNameAsync(context.Document, enumDeclaration, selectedMembers, cancellationToken));
                }

                if (selectedMembers.All(f => f.EqualsValue?.Value != null))
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    List<object> values = selectedMembers
                        .Select(f => semanticModel.GetDeclaredSymbol(f, context.CancellationToken))
                        .Where(f => f.HasConstantValue)
                        .Select(f => f.ConstantValue)
                        .ToList();

                    if (selectedMembers.Length == values.Count
                        && !EnumMemberValueComparer.IsListSorted(values))
                    {
                        context.RegisterRefactoring(
                            "Sort enum members by value",
                            cancellationToken => SortByValueAsync(context.Document, enumDeclaration, selectedMembers, values, cancellationToken));
                    }
                }
            }
        }

        private static async Task<Document> SortByNameAsync(
            Document document,
            EnumDeclarationSyntax enumDeclaration,
            ImmutableArray<EnumMemberDeclarationSyntax> selectedMembers,
            CancellationToken cancellationToken)
        {
            var comparer = new EnumMemberDeclarationComparer();

            SeparatedSyntaxList<EnumMemberDeclarationSyntax> members = enumDeclaration.Members;

            int firstIndex = members.IndexOf(selectedMembers[0]);
            int lastIndex = members.IndexOf(selectedMembers[selectedMembers.Length - 1]);

            SeparatedSyntaxList<EnumMemberDeclarationSyntax> newMembers = members
                .Take(firstIndex)
                .Concat(selectedMembers.OrderBy(f => f, comparer))
                .Concat(members.Skip(lastIndex + 1))
                .ToSeparatedSyntaxList();

            MemberDeclarationSyntax newNode = enumDeclaration.WithMembers(newMembers);

            return await document.ReplaceNodeAsync(enumDeclaration, newNode, cancellationToken).ConfigureAwait(false);
        }

        private static async Task<Document> SortByValueAsync(
            Document document,
            EnumDeclarationSyntax enumDeclaration,
            ImmutableArray<EnumMemberDeclarationSyntax> selectedMembers,
            List<object> values,
            CancellationToken cancellationToken)
        {
            var comparer = new EnumMemberValueComparer();

            SeparatedSyntaxList<EnumMemberDeclarationSyntax> members = enumDeclaration.Members;

            int firstIndex = members.IndexOf(selectedMembers[0]);
            int lastIndex = members.IndexOf(selectedMembers[selectedMembers.Length - 1]);

            IEnumerable<EnumMemberDeclarationSyntax> sorted = selectedMembers
                .Zip(values, (f, g) => new { EnumMember = f, Value = g })
                .OrderBy(f => f.Value, comparer)
                .Select(f => f.EnumMember);

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