// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class EventDeclarationRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, EventDeclarationSyntax eventDeclaration)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.MarkMemberAsStatic)
                && eventDeclaration.Span.Contains(context.Span)
                && MarkMemberAsStaticRefactoring.CanRefactor(eventDeclaration))
            {
                context.RegisterRefactoring(
                    "Mark event as static",
                    cancellationToken => MarkMemberAsStaticRefactoring.RefactorAsync(context.Document, eventDeclaration, cancellationToken));
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.CopyDocumentationCommentFromBaseMember)
                && eventDeclaration.HeaderSpan().Contains(context.Span))
            {
                await CopyDocumentationCommentFromBaseMemberRefactoring.ComputeRefactoringAsync(context, eventDeclaration).ConfigureAwait(false);
            }
        }
    }
}