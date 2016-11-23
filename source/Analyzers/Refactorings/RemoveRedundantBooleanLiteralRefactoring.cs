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
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveRedundantBooleanLiteralRefactoring
    {
        private static DiagnosticDescriptor FadeOutDescriptor
        {
            get { return DiagnosticDescriptors.RemoveRedundantBooleanLiteralFadeOut; }
        }

        public static void ReportDiagnostic(SyntaxNodeAnalysisContext context, BinaryExpressionSyntax binaryExpression, ExpressionSyntax expression)
        {
            if (!binaryExpression.SpanContainsDirectives())
            {
                context.ReportDiagnostic(
               DiagnosticDescriptors.RemoveRedundantBooleanLiteral,
               expression.GetLocation());

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

            switch (binaryExpression.Kind())
            {
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.LogicalAndExpression:
                    {
                        if (left.IsKind(SyntaxKind.TrueLiteralExpression))
                        {
                            context.FadeOutNode(FadeOutDescriptor, left);
                        }
                        else if (right.IsKind(SyntaxKind.TrueLiteralExpression))
                        {
                            context.FadeOutNode(FadeOutDescriptor, right);
                        }

                        break;
                    }
                case SyntaxKind.NotEqualsExpression:
                case SyntaxKind.LogicalOrExpression:
                    {
                        if (left.IsKind(SyntaxKind.FalseLiteralExpression))
                        {
                            context.FadeOutNode(FadeOutDescriptor, left);
                        }
                        else if (right.IsKind(SyntaxKind.FalseLiteralExpression))
                        {
                            context.FadeOutNode(FadeOutDescriptor, right);
                        }

                        break;
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

                newNode = right.WithLeadingTrivia(leadingTrivia);
            }
            else if (right.IsBooleanLiteralExpression())
            {
                SyntaxTriviaList trailingTrivia = binaryExpression.GetTrailingTrivia();

                if (!isWhiteSpaceOrEndOfLine)
                    trailingTrivia = trailingTrivia.InsertRange(0, trivia);

                newNode = left.WithTrailingTrivia(trailingTrivia);
            }
#if DEBUG
            else
            {
                Debug.Assert(false, binaryExpression.ToString());
            }
#endif

            return await document.ReplaceNodeAsync(binaryExpression, newNode.WithFormatterAnnotation(), cancellationToken).ConfigureAwait(false);
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ForStatementSyntax forStatement,
            CancellationToken cancellationToken)
        {
            ForStatementSyntax newForStatement = forStatement;

            if (forStatement
                .DescendantTrivia(TextSpan.FromBounds(forStatement.FirstSemicolonToken.Span.End, forStatement.SecondSemicolonToken.Span.Start))
                .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                newForStatement = forStatement.Update(
                    forStatement.ForKeyword,
                    forStatement.OpenParenToken,
                    forStatement.Declaration,
                    forStatement.Initializers,
                    forStatement.FirstSemicolonToken.WithTrailingTrivia(SpaceTrivia()),
                    default(ExpressionSyntax),
                    forStatement.SecondSemicolonToken.WithoutLeadingTrivia(),
                    forStatement.Incrementors,
                    forStatement.CloseParenToken,
                    forStatement.Statement);
            }
            else
            {
                newForStatement = forStatement.RemoveNode(forStatement.Condition, SyntaxRemoveOptions.KeepExteriorTrivia);
            }

            return await document.ReplaceNodeAsync(forStatement, newForStatement, cancellationToken).ConfigureAwait(false);
        }
    }
}
