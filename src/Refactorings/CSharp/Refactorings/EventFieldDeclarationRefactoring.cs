// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class EventFieldDeclarationRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, EventFieldDeclarationSyntax eventFieldDeclaration)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.GenerateEventInvokingMethod))
                await GenerateOnEventMethodRefactoring.ComputeRefactoringAsync(context, eventFieldDeclaration).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ExpandEventDeclaration)
                && eventFieldDeclaration.Span.Contains(context.Span)
                && ExpandEventDeclarationRefactoring.CanRefactor(eventFieldDeclaration))
            {
                context.RegisterRefactoring(
                    "Expand event",
                    ct => ExpandEventDeclarationRefactoring.RefactorAsync(context.Document, eventFieldDeclaration, ct),
                    RefactoringDescriptors.ExpandEventDeclaration);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.CopyDocumentationCommentFromBaseMember)
                && eventFieldDeclaration.Span.Contains(context.Span)
                && !eventFieldDeclaration.HasDocumentationComment())
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);
                CopyDocumentationCommentFromBaseMemberRefactoring.ComputeRefactoring(context, eventFieldDeclaration, semanticModel);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.AddMemberToInterface))
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
