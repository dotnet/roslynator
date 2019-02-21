// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class InvertBinaryExpressionRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, BinaryExpressionSyntax binaryExpression)
        {
            if (!CanRefactor(binaryExpression))
                return;

            binaryExpression = GetBinaryExpression(binaryExpression, context.Span);

            if (binaryExpression == null)
                return;

            context.RegisterRefactoring(
                "Invert binary expression",
                cancellationToken => RefactorAsync(context.Document, binaryExpression, cancellationToken),
                RefactoringIdentifiers.InvertBinaryExpression);
        }

        private static bool CanRefactor(SyntaxNode node)
        {
            return node.IsKind(SyntaxKind.LogicalAndExpression, SyntaxKind.LogicalOrExpression)
                && SyntaxInfo.BinaryExpressionInfo((BinaryExpressionSyntax)node).Success;
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

        public static async Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax binaryExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ExpressionSyntax newNode = SyntaxInverter.LogicallyInvert(binaryExpression, semanticModel, cancellationToken);

            newNode = newNode.WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(binaryExpression, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}
