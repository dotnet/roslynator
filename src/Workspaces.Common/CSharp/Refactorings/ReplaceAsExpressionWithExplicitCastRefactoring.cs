// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceAsExpressionWithExplicitCastRefactoring
    {
        public const string Title = "Replace 'as' with explicit cast";

        public static Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken)
        {
            return RefactorAsync(
                document,
                binaryExpression,
                binaryExpression.Left,
                (TypeSyntax)binaryExpression.Right,
                cancellationToken);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            ExpressionSyntax left,
            TypeSyntax right,
            CancellationToken cancellationToken)
        {
            ParenthesizedExpressionSyntax newNode = CastExpression(right, left)
                .WithTriviaFrom(binaryExpression)
                .Parenthesize()
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(binaryExpression, newNode, cancellationToken);
        }
    }
}
