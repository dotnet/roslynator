// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactorings
{
    internal static class ExpressionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, ExpressionSyntax expression)
        {
            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.ExtractExpressionFromCondition))
            {
                ExtractExpressionFromIfConditionRefactoring.ComputeRefactoring(context, expression);
                ExtractExpressionFromWhileConditionRefactoring.ComputeRefactoring(context, expression);
            }

            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.WrapExpressionInParentheses)
                && context.Span.IsBetweenSpans(expression)
                && WrapExpressionInParenthesesRefactoring.CanRefactor(context, expression))
            {
                context.RegisterRefactoring(
                    "Wrap in parentheses",
                    cancellationToken => WrapExpressionInParenthesesRefactoring.RefactorAsync(context.Document, expression, cancellationToken));
            }
        }
    }
}
