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
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SimplifyBooleanComparisonRefactoring
    {
        private static DiagnosticDescriptor FadeOutDescriptor
        {
            get { return DiagnosticDescriptors.SimplifyBooleanComparisonFadeOut; }
        }

        public static void ReportDiagnostic(
            SyntaxNodeAnalysisContext context,
            BinaryExpressionSyntax binaryExpression,
            ExpressionSyntax left,
            ExpressionSyntax right,
            bool fadeOut)
        {
            if (!binaryExpression.SpanContainsDirectives())
            {
                context.ReportDiagnostic(DiagnosticDescriptors.SimplifyBooleanComparison, binaryExpression);

                if (fadeOut)
                {
                    context.ReportToken(FadeOutDescriptor, binaryExpression.OperatorToken);

                    switch (binaryExpression.Kind())
                    {
                        case SyntaxKind.EqualsExpression:
                            {
                                if (left.IsKind(SyntaxKind.FalseLiteralExpression))
                                {
                                    context.ReportNode(FadeOutDescriptor, left);

                                    if (right.IsKind(SyntaxKind.LogicalNotExpression))
                                        context.ReportToken(FadeOutDescriptor, ((PrefixUnaryExpressionSyntax)right).OperatorToken);
                                }
                                else if (right.IsKind(SyntaxKind.FalseLiteralExpression))
                                {
                                    context.ReportNode(FadeOutDescriptor, right);

                                    if (left.IsKind(SyntaxKind.LogicalNotExpression))
                                        context.ReportToken(FadeOutDescriptor, ((PrefixUnaryExpressionSyntax)left).OperatorToken);
                                }

                                break;
                            }
                        case SyntaxKind.NotEqualsExpression:
                            {
                                if (left.IsKind(SyntaxKind.TrueLiteralExpression))
                                {
                                    context.ReportNode(FadeOutDescriptor, left);

                                    if (right.IsKind(SyntaxKind.LogicalNotExpression))
                                        context.ReportToken(FadeOutDescriptor, ((PrefixUnaryExpressionSyntax)right).OperatorToken);
                                }
                                else if (right.IsKind(SyntaxKind.TrueLiteralExpression))
                                {
                                    context.ReportNode(FadeOutDescriptor, right);

                                    if (left.IsKind(SyntaxKind.LogicalNotExpression))
                                        context.ReportToken(FadeOutDescriptor, ((PrefixUnaryExpressionSyntax)left).OperatorToken);
                                }

                                break;
                            }
                    }
                }
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax newNode = await CreateNewNode(document, binaryExpression, cancellationToken).ConfigureAwait(false);

            return await document.ReplaceNodeAsync(binaryExpression, newNode.WithFormatterAnnotation(), cancellationToken).ConfigureAwait(false);
        }

        private static async Task<ExpressionSyntax> CreateNewNode(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax left = binaryExpression.Left;
            ExpressionSyntax right = binaryExpression.Right;

            TextSpan span = TextSpan.FromBounds(left.Span.End, right.Span.Start);

            IEnumerable<SyntaxTrivia> trivia = binaryExpression.DescendantTrivia(span);

            bool isWhiteSpaceOrEndOfLine = trivia.All(f => f.IsWhitespaceOrEndOfLineTrivia());

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            if (left.Kind().IsBooleanLiteralExpression())
            {
                SyntaxTriviaList leadingTrivia = binaryExpression.GetLeadingTrivia();

                if (!isWhiteSpaceOrEndOfLine)
                    leadingTrivia = leadingTrivia.AddRange(trivia);

                if (right.IsKind(SyntaxKind.LogicalNotExpression))
                {
                    var logicalNot = (PrefixUnaryExpressionSyntax)right;

                    ExpressionSyntax operand = logicalNot.Operand;

                    if (semanticModel.GetTypeInfo(operand, cancellationToken).ConvertedType.IsNullableOf(SpecialType.System_Boolean))
                    {
                        return binaryExpression
                            .WithLeft(CSharpUtility.LogicallyNegate(left, semanticModel, cancellationToken))
                            .WithRight(operand.WithTriviaFrom(right));
                    }
                }

                return CSharpUtility.LogicallyNegate(right, semanticModel, cancellationToken)
                    .WithLeadingTrivia(leadingTrivia);
            }
            else if (right.Kind().IsBooleanLiteralExpression())
            {
                SyntaxTriviaList trailingTrivia = binaryExpression.GetTrailingTrivia();

                if (!isWhiteSpaceOrEndOfLine)
                    trailingTrivia = trailingTrivia.InsertRange(0, trivia);

                if (left.IsKind(SyntaxKind.LogicalNotExpression))
                {
                    var logicalNot = (PrefixUnaryExpressionSyntax)left;

                    ExpressionSyntax operand = logicalNot.Operand;

                    if (semanticModel.GetTypeInfo(operand, cancellationToken).ConvertedType.IsNullableOf(SpecialType.System_Boolean))
                    {
                        return binaryExpression
                            .WithLeft(operand.WithTriviaFrom(left))
                            .WithRight(CSharpUtility.LogicallyNegate(right, semanticModel, cancellationToken));
                    }
                }

                return CSharpUtility.LogicallyNegate(left, semanticModel, cancellationToken)
                    .WithTrailingTrivia(trailingTrivia);
            }

            Debug.Fail(binaryExpression.ToString());

            return binaryExpression;
        }
    }
}
