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
    internal static class RemoveRedundantBooleanLiteralRefactoring
    {
        public static void ReportDiagnostic(
            SyntaxNodeAnalysisContext context,
            BinaryExpressionSyntax binaryExpression,
            ExpressionSyntax left,
            ExpressionSyntax right)
        {
            if (!binaryExpression.SpanContainsDirectives())
            {
                TextSpan span = GetSpanToRemove(binaryExpression, left, right);

                context.ReportDiagnostic(
                    DiagnosticDescriptors.RemoveRedundantBooleanLiteral,
                    Location.Create(binaryExpression.SyntaxTree, span),
                    binaryExpression.ToString(span));
            }
        }

        public static TextSpan GetSpanToRemove(BinaryExpressionSyntax binaryExpression, ExpressionSyntax left, ExpressionSyntax right)
        {
            SyntaxToken operatorToken = binaryExpression.OperatorToken;

            if (left.Kind().IsBooleanLiteralExpression())
            {
                return TextSpan.FromBounds(left.SpanStart, operatorToken.Span.End);
            }
            else
            {
                return TextSpan.FromBounds(operatorToken.SpanStart, right.Span.End);
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax left = binaryExpression.Left;
            ExpressionSyntax right = binaryExpression.Right;

            ExpressionSyntax newNode = binaryExpression;

            TextSpan span = TextSpan.FromBounds(left.Span.End, right.Span.Start);

            IEnumerable<SyntaxTrivia> trivia = binaryExpression.DescendantTrivia(span);

            bool isWhiteSpaceOrEndOfLine = trivia.All(f => f.IsWhitespaceOrEndOfLineTrivia());

            if (left.Kind().IsBooleanLiteralExpression())
            {
                SyntaxTriviaList leadingTrivia = binaryExpression.GetLeadingTrivia();

                if (!isWhiteSpaceOrEndOfLine)
                    leadingTrivia = leadingTrivia.AddRange(trivia);

                newNode = right.WithLeadingTrivia(leadingTrivia);
            }
            else if (right.Kind().IsBooleanLiteralExpression())
            {
                SyntaxTriviaList trailingTrivia = binaryExpression.GetTrailingTrivia();

                if (!isWhiteSpaceOrEndOfLine)
                    trailingTrivia = trailingTrivia.InsertRange(0, trivia);

                newNode = left.WithTrailingTrivia(trailingTrivia);
            }
            else
            {
                Debug.Fail(binaryExpression.ToString());
            }

            return document.ReplaceNodeAsync(binaryExpression, newNode.WithFormatterAnnotation(), cancellationToken);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            ForStatementSyntax forStatement,
            CancellationToken cancellationToken)
        {
            ForStatementSyntax newForStatement;

            if (forStatement
                .DescendantTrivia(TextSpan.FromBounds(forStatement.FirstSemicolonToken.Span.End, forStatement.SecondSemicolonToken.Span.Start))
                .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                newForStatement = forStatement.Update(
                    forStatement.ForKeyword,
                    forStatement.OpenParenToken,
                    forStatement.Declaration,
                    forStatement.Initializers,
                    forStatement.FirstSemicolonToken.WithTrailingTrivia(SyntaxFactory.Space),
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

            return document.ReplaceNodeAsync(forStatement, newForStatement, cancellationToken);
        }
    }
}
