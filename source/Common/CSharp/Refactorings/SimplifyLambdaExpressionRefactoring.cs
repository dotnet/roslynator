// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SimplifyLambdaExpressionRefactoring
    {
        public static bool CanRefactor(LambdaExpressionSyntax lambda)
        {
            if (lambda == null)
                throw new ArgumentNullException(nameof(lambda));

            CSharpSyntaxNode body = lambda.Body;

            if (body?.IsKind(SyntaxKind.Block) == true)
            {
                var block = (BlockSyntax)body;

                StatementSyntax statement = block.SingleStatementOrDefault();

                if (statement?.IsKind(SyntaxKind.ReturnStatement, SyntaxKind.ExpressionStatement) == true)
                {
                    ExpressionSyntax expression = GetExpression(statement);

                    if (expression?.IsSingleLine() == true)
                    {
                        TextSpan span = TextSpan.FromBounds(lambda.ArrowToken.Span.End, expression.Span.Start);

                        if (lambda
                            .DescendantTrivia(span)
                            .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                        {
                            span = TextSpan.FromBounds(expression.Span.End, block.Span.End);

                            return lambda
                                .DescendantTrivia(span)
                                .All(f => f.IsWhitespaceOrEndOfLineTrivia());
                        }
                    }
                }
            }

            return false;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            LambdaExpressionSyntax lambdaExpressionSyntax,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            if (lambdaExpressionSyntax == null)
                throw new ArgumentNullException(nameof(lambdaExpressionSyntax));

            LambdaExpressionSyntax newLambda = Refactor(lambdaExpressionSyntax)
                .WithTriviaFrom(lambdaExpressionSyntax)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(lambdaExpressionSyntax, newLambda, cancellationToken);
        }

        private static LambdaExpressionSyntax Refactor(LambdaExpressionSyntax lambda)
        {
            var block = (BlockSyntax)lambda.Body;
            StatementSyntax statement = block.Statements[0];
            ExpressionSyntax expression = GetExpression(statement);

            expression = expression
                .WithoutTrivia();

            switch (lambda.Kind())
            {
                case SyntaxKind.SimpleLambdaExpression:
                    {
                        return ((SimpleLambdaExpressionSyntax)lambda)
                            .WithArrowToken(lambda.ArrowToken.WithoutTrailingTrivia())
                            .WithBody(expression);
                    }
                case SyntaxKind.ParenthesizedLambdaExpression:
                    {
                        return ((ParenthesizedLambdaExpressionSyntax)lambda)
                            .WithArrowToken(lambda.ArrowToken.WithoutTrailingTrivia())
                            .WithBody(expression);
                    }
            }

            return lambda;
        }

        private static ExpressionSyntax GetExpression(StatementSyntax statement)
        {
            switch (statement.Kind())
            {
                case SyntaxKind.ReturnStatement:
                    return ((ReturnStatementSyntax)statement).Expression;
                case SyntaxKind.ExpressionStatement:
                    return ((ExpressionStatementSyntax)statement).Expression;
            }

            return null;
        }
    }
}
