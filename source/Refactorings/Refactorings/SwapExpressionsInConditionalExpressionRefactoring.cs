// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SwapExpressionsInConditionalExpressionRefactoring
    {
        public static bool CanRefactor(RefactoringContext context, ConditionalExpressionSyntax conditionalExpression)
        {
            return conditionalExpression.Condition != null
                && conditionalExpression.WhenTrue != null
                && conditionalExpression.WhenFalse != null
                && context.Span.IsBetweenSpans(conditionalExpression);
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ConditionalExpressionSyntax conditionalExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            ConditionalExpressionSyntax newConditionalExpression = conditionalExpression
                .WithCondition(conditionalExpression.Condition.Negate())
                .WithWhenTrue(conditionalExpression.WhenFalse.WithTriviaFrom(conditionalExpression.WhenTrue))
                .WithWhenFalse(conditionalExpression.WhenTrue.WithTriviaFrom(conditionalExpression.WhenFalse))
                .WithFormatterAnnotation();

            SyntaxNode newRoot = oldRoot.ReplaceNode(conditionalExpression, newConditionalExpression);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
