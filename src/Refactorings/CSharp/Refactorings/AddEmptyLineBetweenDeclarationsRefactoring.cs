// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AddEmptyLineBetweenDeclarationsRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, MemberDeclarationListSelection selectedMembers)
        {
            if (selectedMembers.Count == 1)
                return;

            int start = selectedMembers.First().Span.End;

            SyntaxTree syntaxTree = selectedMembers.Parent.SyntaxTree;

            for (int i = selectedMembers.FirstIndex + 1; i <= selectedMembers.LastIndex; i++)
            {
                MemberDeclarationSyntax member = selectedMembers.UnderlyingList[i];

                SyntaxTrivia documentationCommentTrivia = member.GetDocumentationCommentTrivia();

                TextSpan span = (documentationCommentTrivia.IsDocumentationCommentTrivia())
                    ? documentationCommentTrivia.Span
                    : member.Span;

                if (syntaxTree.GetLineCount(TextSpan.FromBounds(start, span.Start), context.CancellationToken) != 2)
                    return;

                start = member.Span.End;
            }

            context.RegisterRefactoring(
                "Add empty line between declarations",
                ct => RefactorAsync(context.Document, selectedMembers, ct),
                RefactoringIdentifiers.AddEmptyLineBetweenDeclarations);
        }

        private static Task<Document> RefactorAsync(
            Document document,
            MemberDeclarationListSelection selectedMembers,
            CancellationToken cancellationToken)
        {
            SyntaxList<MemberDeclarationSyntax> members = selectedMembers.UnderlyingList;

            IEnumerable<MemberDeclarationSyntax> newMembers = members.ModifyRange(
                selectedMembers.FirstIndex,
                selectedMembers.Count - 1,
                member => member.AppendToTrailingTrivia(CSharpFactory.NewLine()));

            MemberDeclarationListInfo membersInfo = SyntaxInfo.MemberDeclarationListInfo(selectedMembers);

            return document.ReplaceMembersAsync(membersInfo, newMembers, cancellationToken);
        }
    }
}