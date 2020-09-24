// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ConstructorDeclarationRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ConstructorDeclarationSyntax constructorDeclaration)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ConvertBlockBodyToExpressionBody)
                && context.SupportsCSharp6
                && ConvertBlockBodyToExpressionBodyRefactoring.CanRefactor(constructorDeclaration, context.Span))
            {
                context.RegisterRefactoring(
                    ConvertBlockBodyToExpressionBodyRefactoring.Title,
                    cancellationToken => ConvertBlockBodyToExpressionBodyRefactoring.RefactorAsync(context.Document, constructorDeclaration, cancellationToken),
                    RefactoringIdentifiers.ConvertBlockBodyToExpressionBody);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.CopyDocumentationCommentFromBaseMember)
                && constructorDeclaration.HeaderSpan().Contains(context.Span)
                && !constructorDeclaration.HasDocumentationComment())
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                CopyDocumentationCommentFromBaseMemberRefactoring.ComputeRefactoring(context, constructorDeclaration, semanticModel);
            }
        }
    }
}