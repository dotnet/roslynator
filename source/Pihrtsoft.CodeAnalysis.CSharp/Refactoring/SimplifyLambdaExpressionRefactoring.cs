// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class SimplifyLambdaExpressionRefactoring
    {
        public static bool CanRefactor(LambdaExpressionSyntax lambda)
        {
            if (lambda.Body?.IsKind(SyntaxKind.Block) == true)
            {
                var block = (BlockSyntax)lambda.Body;

                if (block.Statements.Count == 1)
                {
                    StatementSyntax statement = block.Statements[0];

                    if (statement.IsAnyKind(SyntaxKind.ReturnStatement, SyntaxKind.ExpressionStatement))
                    {
                        ExpressionSyntax expression = GetExpression(statement);

                        TextSpan span = TextSpan.FromBounds(lambda.ArrowToken.Span.End, expression.Span.Start);

                        if (lambda
                            .DescendantTrivia(span)
                            .All(f => f.IsWhitespaceOrEndOfLine()))
                        {
                            span = TextSpan.FromBounds(expression.Span.End, block.Span.End);

                            return lambda
                                .DescendantTrivia(span)
                                .All(f => f.IsWhitespaceOrEndOfLine());
                        }
                    }
                }
            }

            return false;
        }

        public static LambdaExpressionSyntax Refactor(LambdaExpressionSyntax lambda)
        {
            if (lambda == null)
                throw new ArgumentNullException(nameof(lambda));

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
