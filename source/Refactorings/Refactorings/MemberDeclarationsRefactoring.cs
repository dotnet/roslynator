// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MemberDeclarationsRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, MemberDeclarationSyntax member)
        {
            SyntaxNode parent = member.Parent;

            if (parent?.IsKind(
                SyntaxKind.NamespaceDeclaration,
                SyntaxKind.ClassDeclaration,
                SyntaxKind.StructDeclaration,
                SyntaxKind.InterfaceDeclaration) == true)
            {
                var parentMember = (MemberDeclarationSyntax)parent;

                SyntaxList<MemberDeclarationSyntax> members = parentMember.GetMembers();

                if (members.Count > 1)
                {
                    int index = IndexOfMemberToSwap(member, members, context.Span);

                    if (index != -1)
                    {
                        SyntaxTree tree = member.SyntaxTree;

                        FileLinePositionSpan fileLinePositionSpan = tree.GetLineSpan(context.Span, context.CancellationToken);

                        int startLine = fileLinePositionSpan.StartLine();
                        int endLine = fileLinePositionSpan.EndLine();

                        if (startLine > tree.GetEndLine(members[index].TrimmedSpan(), context.CancellationToken)
                            && endLine < tree.GetStartLine(members[index + 1].TrimmedSpan(), context.CancellationToken))
                        {
                            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveMemberDeclarations))
                            {
                                context.RegisterRefactoring(
                                    "Remove members above",
                                    cancellationToken =>
                                    {
                                        return ReplaceMembersAsync(
                                            context.Document,
                                            parentMember,
                                            List(members.Skip(index + 1)),
                                            cancellationToken);
                                    });

                                context.RegisterRefactoring(
                                    "Remove members below",
                                    cancellationToken =>
                                    {
                                        return ReplaceMembersAsync(
                                            context.Document,
                                            parentMember,
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
                                            parentMember,
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

        private static Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationSyntax parentMember,
            SyntaxList<MemberDeclarationSyntax> members,
            int index,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            MemberDeclarationSyntax member = members[index];

            SyntaxList<MemberDeclarationSyntax> newMembers = members
                .Replace(member, members[index + 1]);

            newMembers = newMembers
                .Replace(newMembers[index + 1], member);

            return document.ReplaceNodeAsync(
                parentMember,
                parentMember.WithMembers(newMembers),
                cancellationToken);
        }

        private static Task<Document> ReplaceMembersAsync(
            Document document,
            MemberDeclarationSyntax parentMember,
            SyntaxList<MemberDeclarationSyntax> newMembers,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return document.ReplaceNodeAsync(
                parentMember,
                parentMember.WithMembers(newMembers),
                cancellationToken);
        }
    }
}
