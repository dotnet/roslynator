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
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.UseExpressionBodiedMember)
                && context.SupportsCSharp6
                && UseExpressionBodiedMemberRefactoring.CanRefactor(constructorDeclaration, context.Span))
            {
                context.RegisterRefactoring(
                    UseExpressionBodiedMemberRefactoring.Title,
                    cancellationToken => UseExpressionBodiedMemberRefactoring.RefactorAsync(context.Document, constructorDeclaration, cancellationToken),
                    RefactoringIdentifiers.UseExpressionBodiedMember);
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