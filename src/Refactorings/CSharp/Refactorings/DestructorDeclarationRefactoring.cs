// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.Refactorings
{
    internal static class DestructorDeclarationRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, DestructorDeclarationSyntax destructorDeclaration)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.UseExpressionBodiedMember)
                && destructorDeclaration.Body?.Span.Contains(context.Span) == true
                && context.SupportsCSharp6
                && UseExpressionBodiedMemberAnalysis.IsFixable(destructorDeclaration))
            {
                context.RegisterRefactoring(
                    "Use expression-bodied member",
                    cancellationToken => UseExpressionBodiedMemberRefactoring.RefactorAsync(context.Document, destructorDeclaration, cancellationToken));
            }
        }
    }
}