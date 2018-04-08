// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SimplifyConditionalExpressionRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            ConditionalExpressionSyntax conditionalExpression,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ExpressionSyntax condition = conditionalExpression.Condition;

            ExpressionSyntax newNode = (conditionalExpression.WhenTrue.WalkDownParentheses().IsKind(SyntaxKind.TrueLiteralExpression))
                ? condition
                : Negator.LogicallyNegate(condition, semanticModel, cancellationToken);

            SyntaxTriviaList trailingTrivia = conditionalExpression
                .DescendantTrivia(TextSpan.FromBounds(condition.Span.End, conditionalExpression.Span.End))
                .ToSyntaxTriviaList()
                .EmptyIfWhitespace()
                .AddRange(conditionalExpression.GetTrailingTrivia());

            newNode = newNode
                .WithLeadingTrivia(conditionalExpression.GetLeadingTrivia())
                .WithTrailingTrivia(trailingTrivia);

            return await document.ReplaceNodeAsync(conditionalExpression, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}
