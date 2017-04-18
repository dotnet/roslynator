// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ConditionalExpressionRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, ConditionalExpressionSyntax conditionalExpression)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.FormatConditionalExpression)
                && (context.Span.IsEmpty || context.Span.IsBetweenSpans(conditionalExpression)))
            {
                if (conditionalExpression.IsSingleLine())
                {
                    context.RegisterRefactoring(
                        "Format ?: on separate lines",
                        cancellationToken =>
                        {
                            return CSharpFormatter.ToMultiLineAsync(
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
                            return CSharpFormatter.ToSingleLineAsync(
                                context.Document,
                                conditionalExpression,
                                cancellationToken);
                        });
                }
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ReplaceConditionalExpressionWithIfElse)
                && context.Span.IsBetweenSpans(conditionalExpression))
            {
                await ReplaceConditionalExpressionWithIfElseRefactoring.ComputeRefactoringAsync(context, conditionalExpression).ConfigureAwait(false);
            }

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