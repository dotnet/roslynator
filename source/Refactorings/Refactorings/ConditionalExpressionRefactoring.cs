// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ConditionalExpressionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, ConditionalExpressionSyntax conditionalExpression)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.FormatConditionalExpression)
                && (context.Span.IsEmpty || context.Span.IsBetweenSpans(conditionalExpression)))
            {
                if (conditionalExpression.IsSingleLine())
                {
                    context.RegisterRefactoring(
                        "Format ?: on multiple lines",
                        cancellationToken =>
                        {
                            return FormatConditionalExpressionOnMultipleLinesRefactoring.RefactorAsync(
                                context.Document,
                                conditionalExpression,
                                cancellationToken);
                        });
                }
                else
                {
                    context.RegisterRefactoring(
                        "Format ?: on a single line",
                        cancellationToken =>
                        {
                            return FormatConditionalExpressionOnSingleLineRefactoring.RefactorAsync(
                                context.Document,
                                conditionalExpression,
                                cancellationToken);
                        });
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceConditionalExpressionWithIfElse))
                ReplaceConditionalExpressionWithIfElseRefactoring.ComputeRefactoring(context, conditionalExpression);

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.SwapExpressionsInConditionalExpression)
                && SwapExpressionsInConditionalExpressionRefactoring.CanRefactor(context, conditionalExpression))
            {
                context.RegisterRefactoring(
                    "Swap expressions in ?:",
                    cancellationToken =>
                    {
                        return SwapExpressionsInConditionalExpressionRefactoring.RefactorAsync(
                            context.Document,
                            conditionalExpression,
                            cancellationToken);
                    });
            }
        }
    }
}