// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

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

            ExpressionSyntax expression = GetExpression(block.Statements[0]).WithoutTrivia();

            LambdaExpressionSyntax newLambda = GetNewLambda()
                .WithTriviaFrom(lambda)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(lambda, newLambda, cancellationToken);

            ExpressionSyntax GetExpression(StatementSyntax statement)
            {
                switch (statement.Kind())
                {
                    case SyntaxKind.ReturnStatement:
                        {
                            return ((ReturnStatementSyntax)statement).Expression;
                        }
                    case SyntaxKind.ExpressionStatement:
                        {
                            return ((ExpressionStatementSyntax)statement).Expression;
                        }
                    case SyntaxKind.ThrowStatement:
                        {
                            return ThrowExpression(
                               Token(SyntaxTriviaList.Empty, SyntaxKind.ThrowKeyword, TriviaList(Space)),
                               ((ThrowStatementSyntax)statement).Expression);
                        }
                }

                return null;
            }

            LambdaExpressionSyntax GetNewLambda()
            {
                switch (lambda.Kind())
                {
                    case SyntaxKind.SimpleLambdaExpression:
                        {
                            return ((SimpleLambdaExpressionSyntax)lambda)
                                .WithArrowToken(lambda.ArrowToken.WithTrailingTrivia(TriviaList(Space)))
                                .WithBody(expression);
                        }
                    case SyntaxKind.ParenthesizedLambdaExpression:
                        {
                            return ((ParenthesizedLambdaExpressionSyntax)lambda)
                                .WithArrowToken(lambda.ArrowToken.WithTrailingTrivia(TriviaList(Space)))
                                .WithBody(expression);
                        }
                    default:
                        {
                            throw new InvalidOperationException();
                        }
                }
            }
        }
    }
}
