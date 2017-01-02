// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;

namespace Roslynator.CSharp.Refactorings
{
    internal static class EventDeclarationRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, EventDeclarationSyntax eventDeclaration)
        {
            if (context.IsAnyRefactoringEnabled(RefactoringIdentifiers.MarkMemberAsStatic, RefactoringIdentifiers.MarkAllMembersAsStatic)
                && eventDeclaration.Span.Contains(context.Span)
                && MarkMemberAsStaticRefactoring.CanRefactor(eventDeclaration))
            {
                if (context.IsRefactoringEnabled(RefactoringIdentifiers.MarkMemberAsStatic))
                {
                    context.RegisterRefactoring(
                   "Mark event as static",
                   cancellationToken => MarkMemberAsStaticRefactoring.RefactorAsync(context.Document, eventDeclaration, cancellationToken));
                }

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.MarkAllMembersAsStatic))
                    MarkAllMembersAsStaticRefactoring.RegisterRefactoring(context, (ClassDeclarationSyntax)eventDeclaration.Parent);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.CopyDocumentationCommentFromBaseMember)
                && eventDeclaration.HeaderSpan().Contains(context.Span))
            {
                await CopyDocumentationCommentFromBaseMemberRefactoring.ComputeRefactoringAsync(context, eventDeclaration).ConfigureAwait(false);
            }
        }
    }
}