// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class SimplifyLambdaExpressionRefactoring
    {
        public static LambdaExpressionSyntax SimplifyLambdaExpression(LambdaExpressionSyntax lambda)
        {
            if (lambda == null)
                throw new ArgumentNullException(nameof(lambda));

            if (lambda.Body.IsKind(SyntaxKind.Block))
            {
                var block = (BlockSyntax)lambda.Body;
                if (block.Statements.Count == 1 && block.IsSingleline())
                {
                    StatementSyntax statement = block.Statements[0];
                    ExpressionSyntax expression = GetBody(statement);
                    if (expression != null)
                    {
                        expression = expression
                            .WithLeadingTrivia(GetLeadingTrivia(block, statement))
                            .WithTrailingTrivia(GetTrailingTrivia(block, statement));

                        switch (lambda.Kind())
                        {
                            case SyntaxKind.SimpleLambdaExpression:
                                {
                                    return ((SimpleLambdaExpressionSyntax)lambda)
                                        .WithArrowToken(lambda.ArrowToken.TrimTrailingWhitespace())
                                        .WithBody(expression);
                                }
                            case SyntaxKind.ParenthesizedLambdaExpression:
                                {
                                    return ((ParenthesizedLambdaExpressionSyntax)lambda)
                                        .WithArrowToken(lambda.ArrowToken.TrimTrailingWhitespace())
                                        .WithBody(expression);
                                }
                        }

                        Debug.Assert(false, lambda.Kind().ToString());
                    }
                }
            }

            return lambda;
        }

        private static ExpressionSyntax GetBody(StatementSyntax statement)
        {
            switch (statement.Kind())
            {
                case SyntaxKind.ReturnStatement:
                    return ((ReturnStatementSyntax)statement).Expression;
                case SyntaxKind.ExpressionStatement:
                    return ((ExpressionStatementSyntax)statement).Expression;
            }

            Debug.Assert(false, statement.Kind().ToString());

            return null;
        }

        private static SyntaxTriviaList GetLeadingTrivia(BlockSyntax block, StatementSyntax statement)
        {
            switch (statement.Kind())
            {
                case SyntaxKind.ReturnStatement:
                    {
                        var returnStatement = (ReturnStatementSyntax)statement;

                        return block.GetLeadingTrivia()
                            .AddRange(block.OpenBraceToken.TrailingTrivia)
                            .AddRange(returnStatement.ReturnKeyword.GetLeadingAndTrailingTrivia())
                            .AddRange(returnStatement.Expression.GetLeadingTrivia())
                            .TrimLeadingWhitespace();
                    }
                case SyntaxKind.ExpressionStatement:
                    {
                        var expressionStatement = (ExpressionStatementSyntax)statement;

                        return block.GetLeadingTrivia()
                            .AddRange(block.OpenBraceToken.TrailingTrivia)
                            .AddRange(expressionStatement.GetLeadingTrivia())
                            .AddRange(expressionStatement.Expression.GetLeadingTrivia())
                            .TrimLeadingWhitespace();
                    }
            }

            return SyntaxTriviaList.Empty;
        }

        private static SyntaxTriviaList GetTrailingTrivia(BlockSyntax block, StatementSyntax statement)
        {
            switch (statement.Kind())
            {
                case SyntaxKind.ReturnStatement:
                    {
                        var returnStatement = (ReturnStatementSyntax)statement;

                        return returnStatement.Expression.GetTrailingTrivia()
                            .AddRange(returnStatement.SemicolonToken.GetLeadingAndTrailingTrivia())
                            .AddRange(block.CloseBraceToken.GetLeadingAndTrailingTrivia())
                            .TrimTrailingWhitespace();
                    }
                case SyntaxKind.ExpressionStatement:
                    {
                        var expressionStatement = (ExpressionStatementSyntax)statement;

                        return expressionStatement.Expression.GetTrailingTrivia()
                            .AddRange(expressionStatement.SemicolonToken.GetLeadingAndTrailingTrivia())
                            .AddRange(block.CloseBraceToken.GetLeadingAndTrailingTrivia())
                            .TrimTrailingWhitespace();
                    }
            }

            return SyntaxTriviaList.Empty;
        }
    }
}
