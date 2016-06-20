// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class SplitAttributesRefactoring
    {
        public static void Refactor(RefactoringContext context, MemberDeclarationSyntax member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));

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
                            return SplitAttributesAsync(
                                context.Document,
                                member,
                                lists,
                                cancellationToken);
                        });
                }
            }
        }

        private static async Task<Document> SplitAttributesAsync(
            Document document,
            MemberDeclarationSyntax member,
            AttributeListSyntax[] attributeLists,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            SyntaxList<AttributeListSyntax> lists = member.GetAttributeLists();

            int index = lists.IndexOf(attributeLists[attributeLists.Length - 1]);

            for (int i = attributeLists.Length - 1; i >= 0; i--)
            {
                lists = lists.ReplaceRange(
                    lists[index],
                    AttributeRefactoring.SplitAttributes(attributeLists[i])
                        .Select(f => f.WithAdditionalAnnotations(Formatter.Annotation)));

                index--;
            }

            SyntaxNode newRoot = oldRoot.ReplaceNode(member, member.SetAttributeLists(lists));

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
