// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SimplifyBooleanExpressionRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax logicalAnd,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax left = logicalAnd.Left;
            ExpressionSyntax right = logicalAnd.Right;

            var memberAccessExpression = (MemberAccessExpressionSyntax)left.WalkDownParentheses();
            ExpressionSyntax expression = memberAccessExpression.Expression;

            SyntaxTriviaList trailingTrivia = logicalAnd
                .DescendantTrivia(TextSpan.FromBounds(expression.Span.End, left.Span.End))
                .ToSyntaxTriviaList()
                .EmptyIfWhitespace()
                .AddRange(left.GetTrailingTrivia());

            BinaryExpressionSyntax equalsExpression = EqualsExpression(
                expression
                    .WithLeadingTrivia(left.GetLeadingTrivia())
                    .WithTrailingTrivia(trailingTrivia),
                EqualsEqualsToken().WithTriviaFrom(logicalAnd.OperatorToken),
                (right.WalkDownParentheses().IsKind(SyntaxKind.LogicalNotExpression, SyntaxKind.EqualsExpression))
                    ? FalseLiteralExpression()
                    : TrueLiteralExpression().WithTriviaFrom(right));

            return document.ReplaceNodeAsync(logicalAnd, equalsExpression, cancellationToken);
        }
    }
}
