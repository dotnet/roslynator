// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class FormatBinaryExpressionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, BinaryExpressionSyntax binaryExpression)
        {
            if (context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.FormatBinaryExpression))
            {
                binaryExpression = GetTopmostBinaryExpression(binaryExpression);

                if (binaryExpression.Span.Contains(context.Span)
                    && IsAllowedBinaryExpression(binaryExpression)
                    && binaryExpression.IsSingleLine())
                {
                    string title = binaryExpression.Left?.IsKind(binaryExpression.Kind()) == true
                        ? "Format binary expressions on multiple lines"
                        : "Format binary expression on multiple lines";

                    context.RegisterRefactoring(
                        title,
                        cancellationToken =>
                        {
                            return RefactorAsync(
                                context.Document,
                                binaryExpression,
                                cancellationToken);
                        });
                }
            }
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax condition,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            SyntaxTriviaList triviaList = SyntaxFactory.TriviaList(CSharpFactory.NewLine)
                .AddRange(SyntaxUtility.GetIndentTrivia(condition))
                .Add(CSharpFactory.IndentTrivia);

            var rewriter = new SyntaxRewriter(triviaList);

            var newCondition = (ExpressionSyntax)rewriter.Visit(condition);

             root = root.ReplaceNode(condition, newCondition);

            return document.WithSyntaxRoot(root);
        }

        private static bool IsAllowedBinaryExpression(SyntaxNode node)
        {
            switch (node.Kind())
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
                    && IsAllowedBinaryExpression(binaryExpression.Parent))
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

        private class SyntaxRewriter : CSharpSyntaxRewriter
        {
            private readonly SyntaxTriviaList _triviaList;

            private BinaryExpressionSyntax _previous;

            public SyntaxRewriter(SyntaxTriviaList triviaList)
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
