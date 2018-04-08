// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class EventDeclarationRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, EventDeclarationSyntax eventDeclaration)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.CopyDocumentationCommentFromBaseMember)
                && eventDeclaration.HeaderSpan().Contains(context.Span))
            {
                await CopyDocumentationCommentFromBaseMemberRefactoring.ComputeRefactoringAsync(context, eventDeclaration).ConfigureAwait(false);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.AddMemberToInterface)
                && context.Span.IsEmptyAndContainedInSpanOrBetweenSpans(eventDeclaration.Identifier))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                AddMemberToInterfaceRefactoring.ComputeRefactoring(context, eventDeclaration, semanticModel);
            }
        }
    }
}