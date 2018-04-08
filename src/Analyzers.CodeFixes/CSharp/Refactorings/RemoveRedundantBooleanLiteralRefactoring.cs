// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Roslynator.CSharp.CSharpFacts;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveRedundantBooleanLiteralRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax left = binaryExpression.Left;
            ExpressionSyntax right = binaryExpression.Right;

            ExpressionSyntax newNode = binaryExpression;

            TextSpan span = TextSpan.FromBounds(left.Span.End, right.SpanStart);

            IEnumerable<SyntaxTrivia> trivia = binaryExpression.DescendantTrivia(span);

            bool isWhiteSpaceOrEndOfLine = trivia.All(f => f.IsWhitespaceOrEndOfLineTrivia());

            if (IsBooleanLiteralExpression(left.Kind()))
            {
                SyntaxTriviaList leadingTrivia = binaryExpression.GetLeadingTrivia();

                if (!isWhiteSpaceOrEndOfLine)
                    leadingTrivia = leadingTrivia.AddRange(trivia);

                newNode = right.WithLeadingTrivia(leadingTrivia);
            }
            else if (IsBooleanLiteralExpression(right.Kind()))
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
                .DescendantTrivia(TextSpan.FromBounds(forStatement.FirstSemicolonToken.Span.End, forStatement.SecondSemicolonToken.SpanStart))
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
