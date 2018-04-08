// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class FormatBinaryOperatorOnNextLineRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken)
        {
            BinaryExpressionSyntax newBinaryExpression = BinaryExpression(
                binaryExpression.Kind(),
                binaryExpression.Left.WithTrailingTrivia(binaryExpression.OperatorToken.TrailingTrivia),
                Token(
                    binaryExpression.Right.GetLeadingTrivia(),
                    binaryExpression.OperatorToken.Kind(),
                    TriviaList(Space)),
                binaryExpression.Right.WithoutLeadingTrivia());

            newBinaryExpression = newBinaryExpression
                .WithTriviaFrom(binaryExpression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(binaryExpression, newBinaryExpression, cancellationToken);
        }
    }
}
