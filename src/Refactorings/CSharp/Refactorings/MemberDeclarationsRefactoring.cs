// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MemberDeclarationsRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, MemberDeclarationSyntax member)
        {
            MemberDeclarationListInfo info = SyntaxInfo.MemberDeclarationListInfo(member.Parent);

            if (!info.Success)
                return;

            SyntaxList<MemberDeclarationSyntax> members = info.Members;

            if (members.Count <= 1)
                return;

            int index = IndexOfMemberToSwap(member, members, context.Span);

            if (index == -1)
                return;

            SyntaxTree tree = member.SyntaxTree;

            FileLinePositionSpan fileLinePositionSpan = tree.GetLineSpan(context.Span, context.CancellationToken);

            int startLine = fileLinePositionSpan.StartLine();
            int endLine = fileLinePositionSpan.EndLine();

            if (startLine <= tree.GetEndLine(members[index].TrimmedSpan(), context.CancellationToken))
                return;

            if (endLine >= tree.GetStartLine(members[index + 1].TrimmedSpan(), context.CancellationToken))
                return;

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.RemoveMemberDeclarations))
            {
                context.RegisterRefactoring(
                    "Remove members above",
                    ct => ReplaceMembersAsync(context.Document, info, members.Skip(index + 1), ct));

                context.RegisterRefactoring(
                    "Remove members below",
                    ct => ReplaceMembersAsync(context.Document, info, members.Take(index + 1), ct));
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.SwapMemberDeclarations))
            {
                context.RegisterRefactoring(
                    "Swap members",
                    ct => SwapMembersAsync(context.Document, info, index, ct));
            }
        }

        private static int IndexOfMemberToSwap(
            MemberDeclarationSyntax member,
            SyntaxList<MemberDeclarationSyntax> members,
            TextSpan span)
        {
            int index = members.IndexOf(member);

            if (span.End < member.SpanStart)
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
                    && span.End < members[index + 1].SpanStart)
                {
                    return index;
                }
            }

            return -1;
        }

        private static Task<Document> SwapMembersAsync(
            Document document,
            MemberDeclarationListInfo info,
            int index,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxList<MemberDeclarationSyntax> members = info.Members;

            MemberDeclarationSyntax member = members[index];

            SyntaxList<MemberDeclarationSyntax> newMembers = members.Replace(member, members[index + 1]);

            newMembers = newMembers.Replace(newMembers[index + 1], member);

            return document.ReplaceMembersAsync(info, newMembers, cancellationToken);
        }

        private static Task<Document> ReplaceMembersAsync(
            Document document,
            MemberDeclarationListInfo info,
            IEnumerable<MemberDeclarationSyntax> newMembers,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return document.ReplaceMembersAsync(info, newMembers, cancellationToken);
        }
    }
}
