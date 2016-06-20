// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ExpressionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, ExpressionSyntax expression)
        {
            if (expression.Span == context.Span
                && AddParenthesesRefactoring.CanRefactor(context, expression))
            {
                context.RegisterRefactoring(
                    "Add parentheses",
                    cancellationToken => AddParenthesesRefactoring.RefactorAsync(context.Document, expression, cancellationToken));
            }
        }
    }
}
