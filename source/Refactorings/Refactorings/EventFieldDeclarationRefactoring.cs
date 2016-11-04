// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class EventFieldDeclarationRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, EventFieldDeclarationSyntax eventFieldDeclaration)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.MarkMemberAsStatic)
                && eventFieldDeclaration.Span.Contains(context.Span)
                && MarkMemberAsStaticRefactoring.CanRefactor(eventFieldDeclaration))
            {
                context.RegisterRefactoring(
                    "Mark event as static",
                    cancellationToken => MarkMemberAsStaticRefactoring.RefactorAsync(context.Document, eventFieldDeclaration, cancellationToken));

                MarkAllMembersAsStaticRefactoring.RegisterRefactoring(context, (ClassDeclarationSyntax)eventFieldDeclaration.Parent);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.GenerateOnEventMethod)
                && context.SupportsSemanticModel)
            {
                await GenerateOnEventMethodRefactoring.ComputeRefactoringAsync(context, eventFieldDeclaration).ConfigureAwait(false);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ExpandEvent)
                && eventFieldDeclaration.Span.Contains(context.Span)
                && context.SupportsSemanticModel
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
        }
    }
}