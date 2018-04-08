// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SwapExpressionsInConditionalExpressionRefactoring
    {
        public static bool CanRefactor(ConditionalExpressionSyntax conditionalExpression)
        {
            return SyntaxInfo.ConditionalExpressionInfo(conditionalExpression).Success;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ConditionalExpressionSyntax conditionalExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ConditionalExpressionSyntax newNode = conditionalExpression.Update(
                condition: Negator.LogicallyNegate(conditionalExpression.Condition, semanticModel, cancellationToken),
                questionToken: conditionalExpression.QuestionToken,
                whenTrue: conditionalExpression.WhenFalse.WithTriviaFrom(conditionalExpression.WhenTrue),
                colonToken: conditionalExpression.ColonToken,
                whenFalse: conditionalExpression.WhenTrue.WithTriviaFrom(conditionalExpression.WhenFalse));

            newNode = newNode.WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(conditionalExpression, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}
