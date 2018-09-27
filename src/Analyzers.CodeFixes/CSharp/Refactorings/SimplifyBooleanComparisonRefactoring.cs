// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SimplifyBooleanComparisonRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax newNode = await CreateNewNodeAsync(document, binaryExpression, cancellationToken).ConfigureAwait(false);

            return await document.ReplaceNodeAsync(binaryExpression, newNode.WithFormatterAnnotation(), cancellationToken).ConfigureAwait(false);
        }

        private static async Task<ExpressionSyntax> CreateNewNodeAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax left = binaryExpression.Left;
            ExpressionSyntax right = binaryExpression.Right;

            SyntaxTriviaList trivia = binaryExpression
                .DescendantTrivia(TextSpan.FromBounds(left.Span.End, right.SpanStart))
                .ToSyntaxTriviaList()
                .EmptyIfWhitespace();

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            if (CSharpFacts.IsBooleanLiteralExpression(left.Kind()))
            {
                SyntaxTriviaList leadingTrivia = binaryExpression.GetLeadingTrivia().AddRange(trivia);

                if (right.IsKind(SyntaxKind.LogicalNotExpression))
                {
                    var logicalNot = (PrefixUnaryExpressionSyntax)right;

                    ExpressionSyntax operand = logicalNot.Operand;

                    if (semanticModel.GetTypeInfo(operand, cancellationToken).ConvertedType.IsNullableOf(SpecialType.System_Boolean))
                    {
                        return binaryExpression
                            .WithLeft(Inverter.LogicallyNegate(left, semanticModel, cancellationToken))
                            .WithRight(operand.WithTriviaFrom(right));
                    }
                }

                return Inverter.LogicallyNegate(right, semanticModel, cancellationToken)
                    .WithLeadingTrivia(leadingTrivia);
            }
            else if (CSharpFacts.IsBooleanLiteralExpression(right.Kind()))
            {
                SyntaxTriviaList trailingTrivia = trivia.AddRange(binaryExpression.GetTrailingTrivia());

                if (left.IsKind(SyntaxKind.LogicalNotExpression))
                {
                    var logicalNot = (PrefixUnaryExpressionSyntax)left;

                    ExpressionSyntax operand = logicalNot.Operand;

                    if (semanticModel.GetTypeInfo(operand, cancellationToken).ConvertedType.IsNullableOf(SpecialType.System_Boolean))
                    {
                        return binaryExpression
                            .WithLeft(operand.WithTriviaFrom(left))
                            .WithRight(Inverter.LogicallyNegate(right, semanticModel, cancellationToken));
                    }
                }

                return Inverter.LogicallyNegate(left, semanticModel, cancellationToken)
                    .WithTrailingTrivia(trailingTrivia);
            }

            throw new InvalidOperationException();
        }
    }
}
