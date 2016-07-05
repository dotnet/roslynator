// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Pihrtsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class FormatBinaryExpressionRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, BinaryExpressionSyntax binaryExpression)
        {
            if (!context.Settings.IsRefactoringEnabled(RefactoringIdentifiers.FormatBinaryExpression))
                return;

            binaryExpression = BinaryExpressionRefactoring.GetTopmostExpression(binaryExpression);

            switch (binaryExpression.Parent?.Kind())
            {
                case SyntaxKind.IfStatement:
                case SyntaxKind.DoStatement:
                case SyntaxKind.WhileStatement:
                case SyntaxKind.ReturnStatement:
                case SyntaxKind.YieldReturnStatement:
                    {
                        if (binaryExpression.Span.Contains(context.Span)
                            && binaryExpression.IsAnyKind(SyntaxKind.LogicalAndExpression, SyntaxKind.LogicalOrExpression)
                            && binaryExpression.IsSingleline())
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

                        break;
                    }
            }
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax condition,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken);

            var statement = (StatementSyntax)condition.Parent;

            SyntaxTriviaList triviaList = SyntaxFactory.TriviaList(CSharpFactory.NewLine)
                .AddRange(statement.GetIndentTrivia())
                .Add(CSharpFactory.IndentTrivia);

            var rewriter = new SyntaxRewriter(triviaList);

            var newCondition = (ExpressionSyntax)rewriter.Visit(condition);

             root = root.ReplaceNode(condition, newCondition);

            return document.WithSyntaxRoot(root);
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
                        .WithLeft(node.Left.TrimWhitespace())
                        .WithOperatorToken(node.OperatorToken.WithLeadingTrivia(_triviaList));

                    _previous = node;
                }

                return base.VisitBinaryExpression(node);
            }
        }
    }
}
