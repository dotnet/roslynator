// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveEnumMemberValueRefactoring
    {
        public static void ComputeRefactoring(
            RefactoringContext context,
            SeparatedSyntaxListSelection<EnumMemberDeclarationSyntax> selectedMembers)
        {
            int count = 0;

            for (int i = 0; i < selectedMembers.Count; i++)
            {
                if (selectedMembers[i].EqualsValue?.Value != null)
                {
                    count++;

                    if (count == 2)
                        break;
                }
            }

            if (count == 0)
                return;

            context.RegisterRefactoring(
                (count == 1) ? "Remove enum value" : "Remove enum values",
                cancellationToken => RefactorAsync(context.Document, selectedMembers, cancellationToken),
                RefactoringIdentifiers.RemoveEnumMemberValue);
        }

        private static Task<Document> RefactorAsync(
            Document document,
            SeparatedSyntaxListSelection<EnumMemberDeclarationSyntax> selectedMembers,
            CancellationToken cancellationToken)
        {
            IEnumerable<TextChange> textChanges = selectedMembers
                .Where(f => f.EqualsValue != null)
                .Select(f => new TextChange(TextSpan.FromBounds(f.Identifier.Span.End, f.EqualsValue.Span.End), ""));

            return document.WithTextChangesAsync(textChanges, cancellationToken);
        }
    }
}