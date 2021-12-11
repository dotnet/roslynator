// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class AccessorDeclarationRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, AccessorDeclarationSyntax accessor)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.ConvertBlockBodyToExpressionBody)
                && context.SupportsCSharp6
                && ConvertBlockBodyToExpressionBodyRefactoring.CanRefactor(accessor, context.Span))
            {
                context.RegisterRefactoring(
                    ConvertBlockBodyToExpressionBodyRefactoring.Title,
                    ct => ConvertBlockBodyToExpressionBodyRefactoring.RefactorAsync(context.Document, accessor, ct),
                    RefactoringDescriptors.ConvertBlockBodyToExpressionBody);
            }
        }
    }
}
