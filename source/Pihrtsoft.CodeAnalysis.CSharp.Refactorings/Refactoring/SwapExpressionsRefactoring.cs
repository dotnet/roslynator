// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class SwapExpressionsRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            ExpressionSyntax newNode = binaryExpression
                .WithLeft(binaryExpression.Right.WithTriviaFrom(binaryExpression.Left))
                .WithRight(binaryExpression.Left.WithTriviaFrom(binaryExpression.Right));

            root = root.ReplaceNode(binaryExpression, newNode);

            return document.WithSyntaxRoot(root);
        }
    }
}
