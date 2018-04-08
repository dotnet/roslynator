// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveEnumMemberValueRefactoring
    {
        public static void ComputeRefactoring(
            RefactoringContext context,
            EnumDeclarationSyntax enumDeclaration,
            SeparatedSyntaxListSelection<EnumMemberDeclarationSyntax> selection)
        {
            int count = 0;

            for (int i = 0; i < selection.Count; i++)
            {
                if (selection[i].EqualsValue?.Value != null)
                {
                    count++;

                    if (count  == 2)
                        break;
                }
            }

            if (count == 0)
                return;

            context.RegisterRefactoring(
                (count == 1) ? "Remove enum value" : "Remove enum values",
                cancellationToken => RefactorAsync(context.Document, enumDeclaration, selection, cancellationToken));
        }

        private static Task<Document> RefactorAsync(
            Document document,
            EnumDeclarationSyntax enumDeclaration,
            SeparatedSyntaxListSelection<EnumMemberDeclarationSyntax> selection,
            CancellationToken cancellationToken)
        {
            SeparatedSyntaxList<EnumMemberDeclarationSyntax> members = enumDeclaration.Members;
            SeparatedSyntaxList<EnumMemberDeclarationSyntax> newMembers = members;

            for (int i = selection.FirstIndex; i <= selection.LastIndex; i++)
            {
                EnumMemberDeclarationSyntax newMember = members[i]
                    .WithEqualsValue(null)
                    .WithTrailingTrivia(members[i].GetTrailingTrivia())
                    .WithFormatterAnnotation();

                newMembers = newMembers.ReplaceAt(i, newMember);
            }

            EnumDeclarationSyntax newEnumDeclaration = enumDeclaration.WithMembers(newMembers);

            return document.ReplaceNodeAsync(enumDeclaration, newEnumDeclaration, cancellationToken);
        }
    }
}