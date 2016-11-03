// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MemberDeclarationsRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, MemberDeclarationSyntax member)
        {
            var parent = member.Parent as MemberDeclarationSyntax;

            if (parent != null)
            {
                SyntaxList<MemberDeclarationSyntax> members = parent.GetMembers();

                if (members.Count > 1)
                {
                    int index = IndexOfMemberToSwap(member, members, context.Span);

                    if (index != -1)
                    {
                        FileLinePositionSpan fileSpan = member.SyntaxTree.GetLineSpan(context.Span, context.CancellationToken);
                        if (fileSpan.StartLine() > members[index].SyntaxTree.GetLineSpan(members[index].TrimmedSpan(), context.CancellationToken).EndLine()
                            && fileSpan.EndLine() < members[index + 1].SyntaxTree.GetLineSpan(members[index + 1].TrimmedSpan(), context.CancellationToken).StartLine())
                        {
                            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveMemberDeclarations))
                            {
                                context.RegisterRefactoring(
                                    "Remove members above",
                                    cancellationToken =>
                                    {
                                        return ReplaceMembersAsync(
                                            context.Document,
                                            parent,
                                            members,
                                            List(members.Skip(index + 1)),
                                            cancellationToken);
                                    });

                                context.RegisterRefactoring(
                                    "Remove members below",
                                    cancellationToken =>
                                    {
                                        return ReplaceMembersAsync(
                                            context.Document,
                                            parent,
                                            members,
                                            List(members.Take(index + 1)),
                                            cancellationToken);
                                    });
                            }

                            if (context.IsRefactoringEnabled(RefactoringIdentifiers.SwapMemberDeclarations))
                            {
                                context.RegisterRefactoring(
                                    "Swap members",
                                    cancellationToken =>
                                    {
                                        return RefactorAsync(
                                            context.Document,
                                            parent,
                                            members,
                                            index,
                                            cancellationToken);
                                    });
                            }
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
            MemberDeclarationSyntax parentMember,
            SyntaxList<MemberDeclarationSyntax> members,
            int index,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            MemberDeclarationSyntax member = members[index];

            SyntaxList<MemberDeclarationSyntax> newMembers = members
                .Replace(member, members[index + 1]);

            newMembers = newMembers
                .Replace(newMembers[index + 1], member);

            root = root.ReplaceNode(
                parentMember,
                parentMember.SetMembers(newMembers));

            return document.WithSyntaxRoot(root);
        }

        private static async Task<Document> ReplaceMembersAsync(
            Document document,
            MemberDeclarationSyntax parentMember,
            SyntaxList<MemberDeclarationSyntax> members,
            SyntaxList<MemberDeclarationSyntax> newMembers,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            root = root.ReplaceNode(
                parentMember,
                parentMember.SetMembers(newMembers));

            return document.WithSyntaxRoot(root);
        }
    }
}
