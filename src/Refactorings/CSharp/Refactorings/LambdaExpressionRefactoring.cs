// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.Refactorings
{
    internal static class LambdaExpressionRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, LambdaExpressionSyntax lambda)
        {
            if (context.IsRefactoringEnabled(RefactoringDescriptors.ConvertLambdaExpressionBodyToBlockBody)
                && ConvertLambdaExpressionBodyToBlockBodyRefactoring.CanRefactor(context, lambda))
            {
                context.RegisterRefactoring(
                    "Use block body for lambda expressions",
                    ct =>
                    {
                        return ConvertLambdaExpressionBodyToBlockBodyRefactoring.RefactorAsync(
                            context.Document,
                            lambda,
                            (ExpressionSyntax)lambda.Body,
                            ct);
                    },
                    RefactoringDescriptors.ConvertLambdaExpressionBodyToBlockBody);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ConvertLambdaBlockBodyToExpressionBody)
                && ConvertLambdaExpressionBodyToExpressionBodyAnalysis.IsFixable(lambda))
            {
                context.RegisterRefactoring(
                    ConvertLambdaBlockBodyToExpressionBodyRefactoring.Title,
                    ct =>
                    {
                        return ConvertLambdaBlockBodyToExpressionBodyRefactoring.RefactorAsync(
                            context.Document,
                            lambda,
                            ct);
                    },
                    RefactoringDescriptors.ConvertLambdaBlockBodyToExpressionBody);
            }

            if (context.IsRefactoringEnabled(RefactoringDescriptors.ExtractEventHandlerMethod)
                && context.Span.IsBetweenSpans(lambda)
                && lambda is ParenthesizedLambdaExpressionSyntax parenthesizedLambda)
            {
                await ExtractEventHandlerMethodRefactoring.ComputeRefactoringAsync(context, parenthesizedLambda).ConfigureAwait(false);
            }
        }
    }
}
