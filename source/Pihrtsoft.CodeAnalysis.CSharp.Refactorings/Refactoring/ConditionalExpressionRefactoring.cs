// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ConditionalExpressionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, ConditionalExpressionSyntax conditionalExpression)
        {
            if (conditionalExpression.IsSingleline())
            {
                context.RegisterRefactoring(
                    "Format conditional expression on multiple lines",
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
                    "Format conditional expression on a single line",
                    cancellationToken =>
                    {
                        return FormatConditionalExpressionOnSingleLineRefactoring.RefactorAsync(
                            context.Document,
                            conditionalExpression,
                            cancellationToken);
                    });
            }

            ConvertConditionalExpressionToIfElseRefactoring.ComputeRefactoring(context, conditionalExpression);

            if (SwapExpressionsInConditionalExpressionRefactoring.CanRefactor(context, conditionalExpression))
            {
                context.RegisterRefactoring(
                    "Swap expressions in conditional expression",
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