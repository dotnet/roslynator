// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class EventFieldDeclarationRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, EventFieldDeclarationSyntax eventFieldDeclaration)
        {
            if (context.IsAnyRefactoringEnabled(RefactoringIdentifiers.MarkMemberAsStatic, RefactoringIdentifiers.MarkAllMembersAsStatic)
                && eventFieldDeclaration.Span.Contains(context.Span)
                && MarkMemberAsStaticRefactoring.CanRefactor(eventFieldDeclaration))
            {
                if (context.IsRefactoringEnabled(RefactoringIdentifiers.MarkMemberAsStatic))
                {
                    context.RegisterRefactoring(
                        "Mark event as static",
                        cancellationToken => MarkMemberAsStaticRefactoring.RefactorAsync(context.Document, eventFieldDeclaration, cancellationToken));
                }

                if (context.IsRefactoringEnabled(RefactoringIdentifiers.MarkAllMembersAsStatic))
                    MarkAllMembersAsStaticRefactoring.RegisterRefactoring(context, (ClassDeclarationSyntax)eventFieldDeclaration.Parent);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.MarkContainingClassAsAbstract)
                && eventFieldDeclaration.Span.Contains(context.Span))
            {
                MarkContainingClassAsAbstractRefactoring.ComputeRefactoring(context, eventFieldDeclaration);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.GenerateEventInvokingMethod))
                await GenerateOnEventMethodRefactoring.ComputeRefactoringAsync(context, eventFieldDeclaration).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ExpandEvent)
                && eventFieldDeclaration.Span.Contains(context.Span)
                && ExpandEventRefactoring.CanRefactor(eventFieldDeclaration))
            {
                context.RegisterRefactoring(
                    "Expand event",
                    cancellationToken =>
                    {
                        return ExpandEventRefactoring.RefactorAsync(
                            context.Document,
                            eventFieldDeclaration,
                            cancellationToken);
                    });
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.CopyDocumentationCommentFromBaseMember)
                && eventFieldDeclaration.Span.Contains(context.Span))
            {
                await CopyDocumentationCommentFromBaseMemberRefactoring.ComputeRefactoringAsync(context, eventFieldDeclaration).ConfigureAwait(false);
            }
        }
    }
}