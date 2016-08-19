// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings
{
    internal static class MergeAttributesRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, MemberDeclarationSyntax member)
        {
            if (!context.IsRefactoringEnabled(RefactoringIdentifiers.MergeAttributes))
                return;

            SyntaxList<AttributeListSyntax> attributeLists = member.GetAttributeLists();

            if (attributeLists.Count > 0)
            {
                AttributeListSyntax[] lists = AttributeRefactoring.GetSelectedAttributeLists(attributeLists, context.Span).ToArray();

                if (lists.Length > 1)
                {
                    context.RegisterRefactoring(
                        "Merge attributes",
                        cancellationToken =>
                        {
                            return RefactorAsync(
                                context.Document,
                                member,
                                lists,
                                cancellationToken);
                        });
                }
            }
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax member,
            AttributeListSyntax[] attributeLists,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxList<AttributeListSyntax> lists = member.GetAttributeLists();

            int index = lists.IndexOf(attributeLists[0]);

            for (int i = attributeLists.Length - 1; i >= 1; i--)
                lists = lists.RemoveAt(index);

            AttributeListSyntax list = AttributeRefactoring.MergeAttributes(attributeLists)
                .WithFormatterAnnotation();

            lists = lists.Replace(lists[index], list);

            root = root.ReplaceNode(member, member.SetAttributeLists(lists));

            return document.WithSyntaxRoot(root);
        }
    }
}
