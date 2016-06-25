// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class SwapMembersRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, MemberDeclarationSyntax member)
        {
            var containingMember = member.Parent as MemberDeclarationSyntax;

            if (containingMember != null)
            {
                SyntaxList<MemberDeclarationSyntax> members = containingMember.GetMembers();

                if (members.Count > 1)
                {
                    int index = IndexOfMemberToSwap(member, members, context.Span);

                    if (index != -1)
                    {
                        FileLinePositionSpan fileLinePositionSpan = member.SyntaxTree.GetLineSpan(context.Span, context.CancellationToken);
                        if (fileLinePositionSpan.StartLinePosition.Line > members[index].GetSpanEndLine()
                            && fileLinePositionSpan.EndLinePosition.Line < members[index + 1].GetSpanStartLine())
                        {
                            context.RegisterRefactoring(
                                "Swap members",
                                cancellationToken =>
                                {
                                    return RefactorAsync(
                                        context.Document,
                                        containingMember,
                                        members,
                                        index,
                                        cancellationToken);
                                });
                        }
                    }
                }
            }
        }

        private static int IndexOfMemberToSwap(
            MemberDeclarationSyntax member,
            SyntaxList<MemberDeclarationSyntax> members,
            TextSpan span)
        {
            int index = members.IndexOf(member);

            if (span.End < member.Span.Start)
            {
                if (index > 0
                    && span.Start > members[index - 1].Span.End)
                {
                    return index - 1;
                }
            }
            else if (span.End > member.Span.End)
            {
                if (index < members.Count - 1
                    && span.End < members[index + 1].Span.Start)
                {
                    return index;
                }
            }

            return -1;
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax containingMember,
            SyntaxList<MemberDeclarationSyntax> members,
            int index,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            MemberDeclarationSyntax member = members[index];

            SyntaxList<MemberDeclarationSyntax> newMembers = members
                .Replace(member, members[index + 1]);

            newMembers = newMembers
                .Replace(newMembers[index + 1], member);

            root = root.ReplaceNode(
                containingMember,
                containingMember.SetMembers(newMembers));

            return document.WithSyntaxRoot(root);
        }
    }
}
