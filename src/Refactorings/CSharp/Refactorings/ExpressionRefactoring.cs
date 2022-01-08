// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Refactorings.ExtractCondition;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ExpressionRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ExpressionSyntax expression)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.ExtractExpressionFromCondition)
                && context.Span.IsBetweenSpans(expression))
            {
                ExtractConditionRefactoring.ComputeRefactoring(context, expression);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ParenthesizeExpression)
                && context.Span.IsBetweenSpans(expression)
                && ParenthesizeExpressionRefactoring.CanRefactor(context, expression))
            {
                context.RegisterRefactoring(
                    $"Parenthesize '{expression}'",
                    ct => ParenthesizeExpressionRefactoring.RefactorAsync(context.Document, expression, ct),
                    RefactoringDescriptors.ParenthesizeExpression);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ReplaceNullLiteralWithDefaultExpression))
                await ReplaceNullLiteralWithDefaultExpressionRefactoring.ComputeRefactoringAsync(context, expression).ConfigureAwait(false);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ReplaceConditionalExpressionWithTrueOrFalseBranch))
                ReplaceConditionalExpressionWithTrueOrFalseBranchRefactoring.ComputeRefactoring(context, expression);

            if (context.IsRefactoringEnabled(RefactoringDescriptors.CheckExpressionForNull)
                && context.Span.IsContainedInSpanOrBetweenSpans(expression))
            {
                await CheckExpressionForNullRefactoring.ComputeRefactoringAsync(context, expression).ConfigureAwait(false);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.InlineConstantValue)
                && context.Span.IsBetweenSpans(expression))
            {
                await InlineConstantValueRefactoring.ComputeRefactoringAsync(context, expression).ConfigureAwait(false);
            }
        }
    }
}
