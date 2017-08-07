// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class NegateBinaryExpressionRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, BinaryExpressionSyntax binaryExpression)
        {
            if (CanRefactor(binaryExpression))
            {
                binaryExpression = GetBinaryExpression(binaryExpression, context.Span);

                if (binaryExpression != null)
                {
                    context.RegisterRefactoring(
                        "Negate binary expression",
                        cancellationToken => RefactorAsync(context.Document, binaryExpression, cancellationToken));
                }
            }
        }

        private static bool CanRefactor(SyntaxNode node)
        {
            if (node?.IsKind(
                    SyntaxKind.LogicalAndExpression,
                    SyntaxKind.LogicalOrExpression,
                    SyntaxKind.BitwiseAndExpression,
                    SyntaxKind.BitwiseOrExpression) == true)
            {
                var binaryExpression = (BinaryExpressionSyntax)node;

                return binaryExpression.Left?.IsMissing == false
                    && binaryExpression.Right?.IsMissing == false;
            }

            return false;
        }

        private static BinaryExpressionSyntax GetBinaryExpression(BinaryExpressionSyntax binaryExpression, TextSpan span)
        {
            if (span.IsEmpty)
                return GetTopmostExpression(binaryExpression);

            if (span.IsBetweenSpans(binaryExpression))
                return binaryExpression;

            return null;
        }

        internal static BinaryExpressionSyntax GetTopmostExpression(BinaryExpressionSyntax binaryExpression)
        {
            SyntaxNode parent = binaryExpression.Parent;

            while (CanRefactor(parent))
            {
                binaryExpression = (BinaryExpressionSyntax)parent;
                parent = parent.Parent;
            }

            return binaryExpression;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ExpressionSyntax newNode = Negator.LogicallyNegate(binaryExpression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(binaryExpression, newNode, cancellationToken);
        }
    }
}
