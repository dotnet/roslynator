// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AccessorDeclarationRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, AccessorDeclarationSyntax accessor)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.UseExpressionBodiedMember)
                && context.SupportsCSharp6
                && UseExpressionBodiedMemberRefactoring.CanRefactor(accessor, context.Span))
            {
                context.RegisterRefactoring(
                    UseExpressionBodiedMemberRefactoring.Title,
                    ct => UseExpressionBodiedMemberRefactoring.RefactorAsync(context.Document, accessor, ct),
                    RefactoringIdentifiers.UseExpressionBodiedMember);
            }
        }
    }
}
