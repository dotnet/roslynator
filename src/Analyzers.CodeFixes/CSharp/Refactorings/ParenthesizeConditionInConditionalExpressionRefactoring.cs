// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ParenthesizeConditionInConditionalExpressionRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            ConditionalExpressionSyntax conditionalExpression,
            CancellationToken cancellationToken)
        {
            ConditionalExpressionSyntax newNode = conditionalExpression
                .WithCondition(
                    ParenthesizedExpression(conditionalExpression.Condition.WithoutTrivia())
                        .WithTriviaFrom(conditionalExpression.Condition))
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(conditionalExpression, newNode, cancellationToken);
        }
    }
}
