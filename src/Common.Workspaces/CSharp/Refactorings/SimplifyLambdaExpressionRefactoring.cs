// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SimplifyLambdaExpressionRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            LambdaExpressionSyntax lambda,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var block = (BlockSyntax)lambda.Body;
            StatementSyntax statement = block.Statements[0];
            ExpressionSyntax expression = GetNewExpression(statement).WithoutTrivia();

            LambdaExpressionSyntax newLambda = GetNewLambda()
                .WithTriviaFrom(lambda)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(lambda, newLambda, cancellationToken);

            LambdaExpressionSyntax GetNewLambda()
            {
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
                    default:
                        {
                            throw new InvalidOperationException();
                        }
                }
            }
        }

        private static ExpressionSyntax GetNewExpression(StatementSyntax statement)
        {
            switch (statement.Kind())
            {
                case SyntaxKind.ReturnStatement:
                    return ((ReturnStatementSyntax)statement).Expression;
                case SyntaxKind.ExpressionStatement:
                    return ((ExpressionStatementSyntax)statement).Expression;
                case SyntaxKind.ThrowStatement:
                    return SyntaxFactory.ThrowExpression(((ThrowStatementSyntax)statement).Expression);
            }

            return null;
        }
    }
}
