// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class BinaryExpressionRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, BinaryExpressionSyntax binaryExpression)
        {
            if (binaryExpression.IsAnyKind(SyntaxKind.LogicalAndExpression, SyntaxKind.LogicalOrExpression)
                && binaryExpression.Left?.IsMissing == false
                && binaryExpression.Right?.IsMissing == false)
            {
                if (binaryExpression.Left.Span.Contains(context.Span))
                {
                    await AddBooleanComparisonRefactoring.RefactorAsync(context, binaryExpression.Left);
                }
                else if (binaryExpression.Right.Span.Contains(context.Span))
                {
                    await AddBooleanComparisonRefactoring.RefactorAsync(context, binaryExpression.Right);
                }

                context.RegisterRefactoring(
                    "Negate binary expression",
                    cancellationToken =>
                    {
                        return NegateBinaryExpressionAsync(
                            context.Document,
                            GetTopmostExpression(binaryExpression),
                            cancellationToken);
                    });
            }

            if (binaryExpression.OperatorToken.Span.Contains(context.Span)
                && ExpandCoalesceExpressionRefactoring.CanRefactor(binaryExpression))
            {
                context.RegisterRefactoring(
                    "Expand coalesce expression",
                    cancellationToken =>
                    {
                        return ExpandCoalesceExpressionRefactoring.RefactorAsync(
                            context.Document,
                            binaryExpression,
                            cancellationToken);
                    });
            }

            if (binaryExpression.IsKind(SyntaxKind.AddExpression)
                && binaryExpression.Span.Equals(context.Span)
                && StringLiteralChain.IsStringLiteralChain(binaryExpression))
            {
                context.RegisterRefactoring(
                    "Merge string literals",
                    cancellationToken => MergeStringLiteralsAsync(context.Document, binaryExpression, cancellationToken));

                if (binaryExpression
                    .DescendantTrivia(binaryExpression.Span)
                    .Any(f => f.IsKind(SyntaxKind.EndOfLineTrivia)))
                {
                    context.RegisterRefactoring(
                        "Merge string literals into multiline string literal",
                        cancellationToken => MergeStringLiteralsAsync(context.Document, binaryExpression, cancellationToken, multiline: true));
                }
            }

            SwapExpressionsRefactoring.Refactor(context, binaryExpression);
        }

        private static async Task<Document> NegateBinaryExpressionAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            ExpressionSyntax newNode = binaryExpression.Negate()
                .WithAdditionalAnnotations(Formatter.Annotation);

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

        private static async Task<Document> MergeStringLiteralsAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken = default(CancellationToken),
            bool multiline = false)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            var chain = new StringLiteralChain(binaryExpression);

            LiteralExpressionSyntax newNode = (multiline)
                ? chain.MergeMultiline()
                : chain.Merge();

            root = root.ReplaceNode(
                binaryExpression,
                newNode.WithAdditionalAnnotations(Formatter.Annotation));

            return document.WithSyntaxRoot(root);
        }
    }
}
