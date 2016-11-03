// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Refactorings
{
    internal static class FormatBinaryExpressionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, BinaryExpressionSyntax binaryExpression)
        {
            binaryExpression = GetBinaryExpression(binaryExpression, context.Span);

            if (binaryExpression != null
                && IsFormattableKind(binaryExpression.Kind()))
            {
                string title = "Format binary expression";

                if (binaryExpression.IsSingleLine())
                {
                    title += " on multiple lines";

                    context.RegisterRefactoring(
                        title,
                        cancellationToken => FormatOnMultipleLinesAsync(context.Document, binaryExpression, cancellationToken));
                }
                else
                {
                    title += " on a single line";

                    context.RegisterRefactoring(
                        title,
                        cancellationToken => FormatOnSingleLineAsync(context.Document, binaryExpression, cancellationToken));
                }
            }
        }

        private static BinaryExpressionSyntax GetBinaryExpression(BinaryExpressionSyntax binaryExpression, TextSpan span)
        {
            if (span.IsEmpty)
            {
                return GetTopmostBinaryExpression(binaryExpression);
            }
            else if (span.IsBetweenSpans(binaryExpression)
                && binaryExpression == GetTopmostBinaryExpression(binaryExpression))
            {
                return binaryExpression;
            }

            return null;
        }

        private static async Task<Document> FormatOnMultipleLinesAsync(
            Document document,
            BinaryExpressionSyntax condition,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            SyntaxTriviaList triviaList = SyntaxFactory.TriviaList(CSharpFactory.NewLineTrivia())
                .AddRange(SyntaxUtility.GetIndentTrivia(condition))
                .Add(CSharpFactory.IndentTrivia());

            var rewriter = new BinaryExpressioneSyntaxRewriter(triviaList);

            var newCondition = (ExpressionSyntax)rewriter.Visit(condition);

            root = root.ReplaceNode(condition, newCondition);

            return document.WithSyntaxRoot(root);
        }

        private static async Task<Document> FormatOnSingleLineAsync(
            Document document,
            BinaryExpressionSyntax condition,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            BinaryExpressionSyntax newCondition = SyntaxRemover.RemoveWhitespaceOrEndOfLine(condition);

            root = root.ReplaceNode(
                condition,
                newCondition.WithFormatterAnnotation());

            return document.WithSyntaxRoot(root);
        }

        private static bool IsFormattableKind(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.LogicalAndExpression:
                case SyntaxKind.LogicalOrExpression:
                case SyntaxKind.BitwiseAndExpression:
                case SyntaxKind.BitwiseOrExpression:
                    return true;
                default:
                    return false;
            }
        }

        private static BinaryExpressionSyntax GetTopmostBinaryExpression(BinaryExpressionSyntax binaryExpression)
        {
            bool success = true;

            while (success)
            {
                success = false;

                if (binaryExpression.Parent != null
                    && IsFormattableKind(binaryExpression.Parent.Kind()))
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

        private class BinaryExpressioneSyntaxRewriter : CSharpSyntaxRewriter
        {
            private readonly SyntaxTriviaList _triviaList;

            private BinaryExpressionSyntax _previous;

            public BinaryExpressioneSyntaxRewriter(SyntaxTriviaList triviaList)
            {
                _triviaList = triviaList;
            }

            public override SyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node)
            {
                if (_previous == null
                    || (_previous.Equals(node.Parent) && node.IsKind(_previous.Kind())))
                {
                    node = node
                        .WithLeft(node.Left.TrimTrivia())
                        .WithOperatorToken(node.OperatorToken.WithLeadingTrivia(_triviaList));

                    _previous = node;
                }

                return base.VisitBinaryExpression(node);
            }
        }
    }
}
