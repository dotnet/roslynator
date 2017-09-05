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
            if (expression is LiteralExpressionSyntax)
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            Optional<object> optional = semanticModel.GetConstantValue(expression, context.CancellationToken);

            if (!optional.HasValue)
                return;

            ExpressionSyntax newExpression = CSharpFactory.LiteralExpression(optional.Value);

            context.RegisterRefactoring(
                $"Replace expression with '{newExpression}'",
                cancellationToken => RefactorAsync(context.Document, expression, newExpression, cancellationToken));
        }

        private static Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            ExpressionSyntax newExpression,
            CancellationToken cancellationToken)
        {
            return document.ReplaceNodeAsync(expression, newExpression.WithTriviaFrom(expression), cancellationToken);
        }
    }
}