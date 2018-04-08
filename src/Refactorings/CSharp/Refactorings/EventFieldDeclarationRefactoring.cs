// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class EventFieldDeclarationRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, EventFieldDeclarationSyntax eventFieldDeclaration)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.GenerateEventInvokingMethod))
                await GenerateOnEventMethodRefactoring.ComputeRefactoringAsync(context, eventFieldDeclaration).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ExpandEvent)
                && eventFieldDeclaration.Span.Contains(context.Span)
                && ExpandEventRefactoring.CanRefactor(eventFieldDeclaration))
            {
                context.RegisterRefactoring(
                    "Expand event",
                    ct => ExpandEventRefactoring.RefactorAsync(context.Document, eventFieldDeclaration, ct));
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.CopyDocumentationCommentFromBaseMember)
                && eventFieldDeclaration.Span.Contains(context.Span))
            {
                await CopyDocumentationCommentFromBaseMemberRefactoring.ComputeRefactoringAsync(context, eventFieldDeclaration).ConfigureAwait(false);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddMemberToInterface))
            {
                VariableDeclaratorSyntax variableDeclarator = eventFieldDeclaration.Declaration?.Variables.SingleOrDefault(shouldThrow: false);

                if (variableDeclarator != null
                    && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(variableDeclarator.Identifier))
                {
                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    AddMemberToInterfaceRefactoring.ComputeRefactoring(context, eventFieldDeclaration, semanticModel);
                }
            }
        }
    }
}