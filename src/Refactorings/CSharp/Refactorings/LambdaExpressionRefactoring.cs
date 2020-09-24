// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.Refactorings
{
    internal static class LambdaExpressionRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, LambdaExpressionSyntax lambda)
        {
            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ConvertLambdaExpressionBodyToBlockBody)
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
                    RefactoringIdentifiers.ConvertLambdaExpressionBodyToBlockBody);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ConvertLambdaExpressionBodyToExpressionBody)
                && ConvertLambdaExpressionBodyToExpressionBodyAnalysis.IsFixable(lambda))
            {
                context.RegisterRefactoring(
                    ConvertLambdaExpressionBodyToExpressionBodyRefactoring.Title,
                    ct =>
                    {
                        return ConvertLambdaExpressionBodyToExpressionBodyRefactoring.RefactorAsync(
                            context.Document,
                            lambda,
                            ct);
                    },
                    RefactoringIdentifiers.ConvertLambdaExpressionBodyToExpressionBody);
            }

            if (context.IsRefactoringEnabled(RefactoringIdentifiers.ExtractEventHandlerMethod)
                && context.Span.IsBetweenSpans(lambda)
                && lambda is ParenthesizedLambdaExpressionSyntax parenthesizedLambda)
            {
                await ExtractEventHandlerMethodRefactoring.ComputeRefactoringAsync(context, parenthesizedLambda).ConfigureAwait(false);
            }
        }
    }
}
