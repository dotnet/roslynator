// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceAsWithCastRefactoring
    {
        public static bool CanRefactor(
            BinaryExpressionSyntax binaryExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (binaryExpression.Kind() != SyntaxKind.AsExpression)
                return false;

            ExpressionSyntax left = binaryExpression.Left;

            if (left?.IsMissing != false)
                return false;

            var type = binaryExpression.Right as TypeSyntax;

            if (type?.IsMissing != false)
                return false;

            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, cancellationToken);

            if (typeSymbol == null)
                return false;

            if (!semanticModel.IsExplicitConversion(left, typeSymbol))
                return false;

            return true;
        }

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