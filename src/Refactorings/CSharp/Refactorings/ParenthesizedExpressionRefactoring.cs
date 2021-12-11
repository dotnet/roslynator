// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ParenthesizedExpressionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, ParenthesizedExpressionSyntax parenthesizedExpression)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.RemoveParentheses)
                && ExtractExpressionFromParenthesesRefactoring.CanRefactor(context, parenthesizedExpression))
            {
                context.RegisterRefactoring(
                    "Remove parentheses",
                    ct =>
                    {
                        return ExtractExpressionFromParenthesesRefactoring.RefactorAsync(
                            context.Document,
                            parenthesizedExpression,
                            ct);
                    },
                    RefactoringDescriptors.RemoveParentheses);
            }
        }
    }
}
