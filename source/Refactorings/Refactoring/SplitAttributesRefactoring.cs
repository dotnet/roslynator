// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class SplitAttributesRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, MemberDeclarationSyntax member)
        {
            if (!context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.SplitAttributes))
                return;

            SyntaxList<AttributeListSyntax> attributeLists = member.GetAttributeLists();

            if (attributeLists.Count > 0)
            {
                AttributeListSyntax[] lists = AttributeRefactoring.GetSelectedAttributeLists(attributeLists, context.Span).ToArray();

                if (lists.Length > 0
                    && lists.Any(f => f.Attributes.Count > 1))
                {
                    context.RegisterRefactoring("Split attributes",
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
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxList<AttributeListSyntax> lists = member.GetAttributeLists();

            int index = lists.IndexOf(attributeLists[attributeLists.Length - 1]);

            for (int i = attributeLists.Length - 1; i >= 0; i--)
            {
                lists = lists.ReplaceRange(
                    lists[index],
                    AttributeRefactoring.SplitAttributes(attributeLists[i])
                        .Select(f => f.WithFormatterAnnotation()));

                index--;
            }

            SyntaxNode newRoot = oldRoot.ReplaceNode(member, member.SetAttributeLists(lists));

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
