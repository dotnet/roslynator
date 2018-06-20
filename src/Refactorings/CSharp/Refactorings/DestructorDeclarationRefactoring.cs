// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class DestructorDeclarationRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, DestructorDeclarationSyntax destructorDeclaration)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.UseExpressionBodiedMember)
                && context.SupportsCSharp6
                && UseExpressionBodiedMemberRefactoring.CanRefactor(destructorDeclaration, context.Span))
            {
                context.RegisterRefactoring(
                    UseExpressionBodiedMemberRefactoring.Title,
                    cancellationToken => UseExpressionBodiedMemberRefactoring.RefactorAsync(context.Document, destructorDeclaration, cancellationToken),
                    RefactoringIdentifiers.UseExpressionBodiedMember);
            }
        }
    }
}