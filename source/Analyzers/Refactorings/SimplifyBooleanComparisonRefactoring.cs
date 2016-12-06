// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SimplifyBooleanComparisonRefactoring
    {
        private static DiagnosticDescriptor FadeOutDescriptor
        {
            get { return DiagnosticDescriptors.SimplifyBooleanComparisonFadeOut; }
        }

        public static void ReportDiagnostic(SyntaxNodeAnalysisContext context, BinaryExpressionSyntax binaryExpression)
        {
            if (!binaryExpression.SpanContainsDirectives())
            {
                context.ReportDiagnostic(DiagnosticDescriptors.SimplifyBooleanComparison, binaryExpression.GetLocation());

                FadeOut(context, binaryExpression);
            }
        }

        private static void FadeOut(
            SyntaxNodeAnalysisContext context,
            BinaryExpressionSyntax binaryExpression)
        {
            context.FadeOutToken(FadeOutDescriptor, binaryExpression.OperatorToken);

            ExpressionSyntax left = binaryExpression.Left;
            ExpressionSyntax right = binaryExpression.Right;

            if (binaryExpression.IsKind(SyntaxKind.EqualsExpression))
            {
                if (left.IsKind(SyntaxKind.FalseLiteralExpression))
                {
                    context.FadeOutNode(FadeOutDescriptor, left);

                    if (right.IsKind(SyntaxKind.LogicalNotExpression))
                        context.FadeOutToken(FadeOutDescriptor, ((PrefixUnaryExpressionSyntax)right).OperatorToken);
                }
                else if (right.IsKind(SyntaxKind.FalseLiteralExpression))
                {
                    context.FadeOutNode(FadeOutDescriptor, right);

                    if (left.IsKind(SyntaxKind.LogicalNotExpression))
                        context.FadeOutToken(FadeOutDescriptor, ((PrefixUnaryExpressionSyntax)left).OperatorToken);
                }
            }
            else if (binaryExpression.IsKind(SyntaxKind.NotEqualsExpression))
            {
                if (left.IsKind(SyntaxKind.TrueLiteralExpression))
                {
                    context.FadeOutNode(FadeOutDescriptor, left);

                    if (right.IsKind(SyntaxKind.LogicalNotExpression))
                        context.FadeOutToken(FadeOutDescriptor, ((PrefixUnaryExpressionSyntax)right).OperatorToken);
                }
                else if (right.IsKind(SyntaxKind.TrueLiteralExpression))
                {
                    context.FadeOutNode(FadeOutDescriptor, right);

                    if (left.IsKind(SyntaxKind.LogicalNotExpression))
                        context.FadeOutToken(FadeOutDescriptor, ((PrefixUnaryExpressionSyntax)left).OperatorToken);
                }
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax left = binaryExpression.Left;
            ExpressionSyntax right = binaryExpression.Right;
            SyntaxToken operatorToken = binaryExpression.OperatorToken;

            ExpressionSyntax newNode = binaryExpression;

            TextSpan span = TextSpan.FromBounds(left.Span.End, right.Span.Start);

            IEnumerable<SyntaxTrivia> trivia = binaryExpression.DescendantTrivia(span);

            bool isWhiteSpaceOrEndOfLine = trivia.All(f => f.IsWhitespaceOrEndOfLineTrivia());

            if (left.IsBooleanLiteralExpression())
            {
                SyntaxTriviaList leadingTrivia = binaryExpression.GetLeadingTrivia();

                if (!isWhiteSpaceOrEndOfLine)
                    leadingTrivia = leadingTrivia.AddRange(trivia);

                newNode = right
                    .LogicallyNegate()
                    .WithLeadingTrivia(leadingTrivia);
            }
            else if (right.IsBooleanLiteralExpression())
            {
                SyntaxTriviaList trailingTrivia = binaryExpression.GetTrailingTrivia();

                if (!isWhiteSpaceOrEndOfLine)
                    trailingTrivia = trailingTrivia.InsertRange(0, trivia);

                newNode = left
                    .LogicallyNegate()
                    .WithTrailingTrivia(trailingTrivia);
            }
#if DEBUG
            else
            {
                Debug.Assert(false, binaryExpression.ToString());
            }
#endif

            return await document.ReplaceNodeAsync(binaryExpression, newNode.WithFormatterAnnotation(), cancellationToken).ConfigureAwait(false);
        }
    }
}
