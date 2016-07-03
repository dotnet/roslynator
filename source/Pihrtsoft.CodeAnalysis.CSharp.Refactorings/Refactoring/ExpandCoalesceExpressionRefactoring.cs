// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ExpandCoalesceExpressionRefactoring
    {
        public static bool CanRefactor(BinaryExpressionSyntax binaryExpression)
        {
            return binaryExpression.IsKind(SyntaxKind.CoalesceExpression)

                && binaryExpression.Left?.IsMissing == false
                && binaryExpression.Right?.IsMissing == false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            ExpressionSyntax left = binaryExpression.Left.WithoutTrivia();
            ExpressionSyntax right = binaryExpression.Right.WithoutTrivia();

            SyntaxNode newNode = ConditionalExpression(
                ParenthesizedExpression(
                    BinaryExpression(
                        SyntaxKind.NotEqualsExpression,
                        left,
                        LiteralExpression(SyntaxKind.NullLiteralExpression))),
                left,
                right);

            newNode = newNode
                .WithTriviaFrom(binaryExpression)
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(binaryExpression, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
