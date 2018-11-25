// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseNameOfOperatorRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            LiteralExpressionSyntax literalExpression,
            string identifier,
            CancellationToken cancellationToken)
        {
            InvocationExpressionSyntax newNode = NameOfExpression(identifier)
                .WithTriviaFrom(literalExpression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(literalExpression, newNode, cancellationToken);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            InvocationExpressionSyntax invocationExpression,
            CancellationToken cancellationToken)
        {
            var memberAccessExpression = (MemberAccessExpressionSyntax)invocationExpression.Expression;

            InvocationExpressionSyntax newNode = NameOfExpression(memberAccessExpression.Expression)
                .WithTriviaFrom(invocationExpression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(invocationExpression, newNode, cancellationToken);
        }
    }
}
