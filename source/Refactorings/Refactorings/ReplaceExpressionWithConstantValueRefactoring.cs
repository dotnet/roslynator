// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceExpressionWithConstantValueRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, ExpressionSyntax expression)
        {
            if (!(expression is LiteralExpressionSyntax))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                Optional<object> optional = semanticModel.GetConstantValue(expression, context.CancellationToken);

                if (optional.HasValue)
                {
                    context.RegisterRefactoring(
                        "Replace with constant value",
                        cancellationToken => RefactorAsync(context.Document, expression, optional.Value, cancellationToken));
                }
            }
        }

        private static Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            object constantValue,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax newExpression = CSharpFactory.LiteralExpression(constantValue);

            return document.ReplaceNodeAsync(expression, newExpression.WithTriviaFrom(expression), cancellationToken);
        }
    }
}