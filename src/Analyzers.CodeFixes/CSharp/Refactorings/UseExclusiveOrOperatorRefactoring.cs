// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseExclusiveOrOperatorRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax logicalOr,
            CancellationToken cancellationToken)
        {
            var logicalAnd = (BinaryExpressionSyntax)logicalOr.Left.WalkDownParentheses();

            ExpressionSyntax left = logicalAnd.Left;
            ExpressionSyntax right = logicalAnd.Right;

            ExpressionSyntax newLeft = left.WalkDownParentheses();
            ExpressionSyntax newRight = right.WalkDownParentheses();

            if (newLeft.IsKind(SyntaxKind.LogicalNotExpression))
            {
                newLeft = ((PrefixUnaryExpressionSyntax)newLeft).Operand.WalkDownParentheses();
            }
            else if (newRight.IsKind(SyntaxKind.LogicalNotExpression))
            {
                newRight = ((PrefixUnaryExpressionSyntax)newRight).Operand.WalkDownParentheses();
            }

            ExpressionSyntax newNode = ExclusiveOrExpression(
                newLeft.WithTriviaFrom(left).Parenthesize(),
                CaretToken().WithTriviaFrom(logicalOr.OperatorToken),
                newRight.WithTriviaFrom(right).Parenthesize());

            newNode = newNode
                .WithTriviaFrom(logicalOr)
                .Parenthesize()
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(logicalOr, newNode, cancellationToken);
        }

        private readonly struct ExpressionPair
        {
            public ExpressionPair(ExpressionSyntax expression, ExpressionSyntax negatedExpression)
            {
                Expression = expression;
                NegatedExpression = negatedExpression;
            }

            public bool IsValid
            {
                get { return Expression != null && NegatedExpression != null; }
            }

            public ExpressionSyntax Expression { get; }
            public ExpressionSyntax NegatedExpression { get; }
        }
    }
}
