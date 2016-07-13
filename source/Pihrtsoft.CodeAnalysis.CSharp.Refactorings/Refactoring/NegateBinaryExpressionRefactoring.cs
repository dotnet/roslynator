// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class NegateBinaryExpressionRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            binaryExpression = GetTopmostExpression(binaryExpression);

            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            ExpressionSyntax newNode = binaryExpression.Negate()
                .WithFormatterAnnotation();

            root = root.ReplaceNode(binaryExpression, newNode);

            return document.WithSyntaxRoot(root);
        }

        private static BinaryExpressionSyntax GetTopmostExpression(BinaryExpressionSyntax binaryExpression)
        {
            bool success = true;

            while (success)
            {
                success = false;

                if (binaryExpression.Parent != null
                    && binaryExpression.Parent.IsAnyKind(SyntaxKind.LogicalAndExpression, SyntaxKind.LogicalOrExpression))
                {
                    var parent = (BinaryExpressionSyntax)binaryExpression.Parent;

                    if (parent.Left?.IsMissing == false
                        && parent.Right?.IsMissing == false)
                    {
                        binaryExpression = parent;
                        success = true;
                    }
                }
            }

            return binaryExpression;
        }
    }
}
