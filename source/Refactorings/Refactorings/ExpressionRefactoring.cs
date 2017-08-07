// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings.ExtractCondition;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ExpressionRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ExpressionSyntax expression)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ExtractExpressionFromCondition)
                && context.Span.IsBetweenSpans(expression))
            {
                ExtractConditionRefactoring.ComputeRefactoring(context, expression);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ParenthesizeExpression)
                && context.Span.IsBetweenSpans(expression)
                && ParenthesizeExpressionRefactoring.CanRefactor(context, expression))
            {
                context.RegisterRefactoring(
                    $"Parenthesize '{expression}'",
                    cancellationToken => ParenthesizeExpressionRefactoring.RefactorAsync(context.Document, expression, cancellationToken));
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceNullLiteralExpressionWithDefaultExpression))
                await ReplaceNullLiteralExpressionWithDefaultExpressionRefactoring.ComputeRefactoringAsync(context, expression).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceConditionalExpressionWithExpression))
                ReplaceConditionalExpressionWithExpressionRefactoring.ComputeRefactoring(context, expression);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.CheckExpressionForNull)
                && context.Span.IsContainedInSpanOrBetweenSpans(expression))
            {
                await CheckExpressionForNullRefactoring.ComputeRefactoringAsync(context, expression).ConfigureAwait(false);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceExpressionWithConstantValue)
                && context.Span.IsBetweenSpans(expression))
            {
                await ReplaceExpressionWithConstantValueRefactoring.ComputeRefactoringAsync(context, expression).ConfigureAwait(false);
            }
        }
    }
}
