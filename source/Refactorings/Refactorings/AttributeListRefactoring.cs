// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AttributeListRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, MemberDeclarationSyntax member)
        {
            if (context.IsAnyRefactoringEnabled(
                RefactoringIdentifiers.SplitAttributes,
                RefactoringIdentifiers.MergeAttributes))
            {
                SyntaxList<AttributeListSyntax> lists = member.GetAttributeLists();

                if (lists.Count > 0)
                {
                    var info = new SelectedNodesInfo<AttributeListSyntax>(lists, context.Span);

                    if (context.IsRefactoringEnabled(RefactoringIdentifiers.SplitAttributes)
                        && info.SelectedNodes().Any(f => f.Attributes.Count > 1))
                    {
                        context.RegisterRefactoring(
                            "Split attributes",
                            cancellationToken =>
                            {
                                return SplitAsync(
                                    context.Document,
                                    member,
                                    info.SelectedNodes().ToArray(),
                                    cancellationToken);
                            });
                    }

                    if (context.IsRefactoringEnabled(RefactoringIdentifiers.MergeAttributes)
                        && info.AreManySelected)
                    {
                        context.RegisterRefactoring(
                            "Merge attributes",
                            cancellationToken =>
                            {
                                return MergeAsync(
                                    context.Document,
                                    member,
                                    info.SelectedNodes().ToArray(),
                                    cancellationToken);
                            });
                    }
                }
            }
        }

        public static async Task<Document> SplitAsync(
            Document document,
            MemberDeclarationSyntax member,
            AttributeListSyntax[] attributeLists,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxList<AttributeListSyntax> lists = member.GetAttributeLists();

            var newLists = new List<AttributeListSyntax>();

            int index = lists.IndexOf(attributeLists[0]);

            for (int i = 0; i < index; i++)
                newLists.Add(lists[i]);

            newLists.AddRange(attributeLists.SelectMany(f => AttributeRefactoring.SplitAttributes(f)));

            for (int i = index + attributeLists.Length; i < lists.Count; i++)
                newLists.Add(lists[i]);

            SyntaxNode newRoot = root.ReplaceNode(
                member,
                member.SetAttributeLists(newLists.ToSyntaxList()));

            return document.WithSyntaxRoot(newRoot);
        }

        public static async Task<Document> MergeAsync(
            Document document,
            MemberDeclarationSyntax member,
            AttributeListSyntax[] attributeLists,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxList<AttributeListSyntax> lists = member.GetAttributeLists();

            var newLists = new List<AttributeListSyntax>(lists.Count - attributeLists.Length + 1);

            int index = lists.IndexOf(attributeLists[0]);

            for (int i = 0; i < index; i++)
                newLists.Add(lists[i]);

            newLists.Add(AttributeRefactoring.MergeAttributes(attributeLists));

            for (int i = index + attributeLists.Length; i < lists.Count; i++)
                newLists.Add(lists[i]);

            SyntaxNode newRoot = root.ReplaceNode(
                member,
                member.SetAttributeLists(newLists.ToSyntaxList()));

            return document.WithSyntaxRoot(newRoot);
        }
    }
}