// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SimplifyConditionalExpressionRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            ConditionalExpressionSyntax conditionalExpression,
            CancellationToken cancellationToken)
        {
            ConditionalExpressionInfo info = SyntaxInfo.ConditionalExpressionInfo(conditionalExpression);

            ExpressionSyntax whenTrue = info.WhenTrue;
            ExpressionSyntax whenFalse = info.WhenFalse;

            SyntaxKind trueKind = whenTrue.Kind();
            SyntaxKind falseKind = whenFalse.Kind();

            ExpressionSyntax newNode = null;

            if (trueKind == SyntaxKind.TrueLiteralExpression)
            {
                if (falseKind == SyntaxKind.FalseLiteralExpression)
                {
                    newNode = CreateNewNode(conditionalExpression, info.Condition);
                }
                else
                {
                    SyntaxTriviaList trailingTrivia = info
                        .QuestionToken
                        .LeadingTrivia
                        .AddRange(info.QuestionToken.TrailingTrivia)
                        .AddRange(whenTrue.GetLeadingTrivia())
                        .EmptyIfWhitespace();

                    newNode = LogicalOrExpression(
                        conditionalExpression.Condition.Parenthesize().AppendToTrailingTrivia(trailingTrivia),
                        Token(info.ColonToken.LeadingTrivia, SyntaxKind.BarBarToken, info.ColonToken.TrailingTrivia),
                        whenFalse.Parenthesize());
                }
            }
            else if (falseKind == SyntaxKind.FalseLiteralExpression)
            {
                SyntaxTriviaList trailingTrivia = whenTrue
                    .GetTrailingTrivia()
                    .AddRange(info.ColonToken.LeadingTrivia)
                    .AddRange(info.ColonToken.TrailingTrivia)
                    .AddRange(whenFalse.GetLeadingTrivia())
                    .EmptyIfWhitespace()
                    .AddRange(whenFalse.GetTrailingTrivia());

                newNode = LogicalAndExpression(
                    conditionalExpression.Condition.Parenthesize(),
                    Token(info.QuestionToken.LeadingTrivia, SyntaxKind.AmpersandAmpersandToken, info.QuestionToken.TrailingTrivia),
                    whenTrue.WithTrailingTrivia(trailingTrivia).Parenthesize());
            }
            else if (trueKind == SyntaxKind.FalseLiteralExpression
                && falseKind == SyntaxKind.TrueLiteralExpression)
            {
                SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

                newNode = CreateNewNode(conditionalExpression, SyntaxInverter.LogicallyInvert(info.Condition, semanticModel, cancellationToken));
            }

            newNode = newNode.Parenthesize();

            return await document.ReplaceNodeAsync(conditionalExpression, newNode, cancellationToken).ConfigureAwait(false);
        }

        private static ExpressionSyntax CreateNewNode(
            ConditionalExpressionSyntax conditionalExpression,
            ExpressionSyntax newNode)
        {
            SyntaxTriviaList trailingTrivia = conditionalExpression
                .DescendantTrivia(TextSpan.FromBounds(conditionalExpression.Condition.Span.End, conditionalExpression.Span.End))
                .ToSyntaxTriviaList()
                .EmptyIfWhitespace()
                .AddRange(conditionalExpression.GetTrailingTrivia());

            return newNode
                .WithLeadingTrivia(conditionalExpression.GetLeadingTrivia())
                .WithTrailingTrivia(trailingTrivia)
                .WithSimplifierAnnotation();
        }
    }
}
