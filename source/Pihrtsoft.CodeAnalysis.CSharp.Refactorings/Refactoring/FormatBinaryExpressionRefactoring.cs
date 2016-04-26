// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Pihrtsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class FormatBinaryExpressionRefactoring
    {
        public static void Refactor(CodeRefactoringContext context, IfStatementSyntax ifStatement)
        {
            Refactor(context, ifStatement, ifStatement.Condition);
        }

        public static void Refactor(CodeRefactoringContext context, WhileStatementSyntax whileStatement)
        {
            Refactor(context, whileStatement, whileStatement.Condition);
        }

        public static void Refactor(CodeRefactoringContext context, DoStatementSyntax doStatement)
        {
            Refactor(context, doStatement, doStatement.Condition);
        }

        public static void Refactor(
            CodeRefactoringContext context,
            StatementSyntax statement,
            ExpressionSyntax condition)
        {
            if (statement == null)
                throw new ArgumentNullException(nameof(statement));

            if (condition != null
                && condition.Span.Contains(context.Span)
                && condition.IsAnyKind(SyntaxKind.LogicalAndExpression, SyntaxKind.LogicalOrExpression)
                && ((BinaryExpressionSyntax)condition).Left?.IsKind(condition.Kind()) == true
                && condition.IsSingleline())
            {
                context.RegisterRefactoring(
                    "Format binary expressions on multiple lines",
                    cancellationToken =>
                    {
                        return RefactorAsync(
                            context.Document,
                            statement,
                            condition,
                            cancellationToken);
                    });
            }
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            StatementSyntax statement,
            ExpressionSyntax condition,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            SyntaxTriviaList triviaList = SyntaxFactory.TriviaList(SyntaxHelper.NewLine)
                .AddRange(statement.GetIndentTrivia())
                .Add(SyntaxHelper.DefaultIndent);

            var rewriter = new SyntaxRewriter(triviaList);

            var newCondition = (ExpressionSyntax)rewriter.Visit(condition);

            StatementSyntax newStatement = SetCondition(statement, newCondition)
                .WithAdditionalAnnotations(Formatter.Annotation);

            return document.WithSyntaxRoot(oldRoot.ReplaceNode(statement, newStatement));
        }

        private static StatementSyntax SetCondition(StatementSyntax statement, ExpressionSyntax condition)
        {
            switch (statement.Kind())
            {
                case SyntaxKind.IfStatement:
                    return ((IfStatementSyntax)statement).WithCondition(condition);
                case SyntaxKind.WhileStatement:
                    return ((WhileStatementSyntax)statement).WithCondition(condition);
                case SyntaxKind.DoStatement:
                    return ((DoStatementSyntax)statement).WithCondition(condition);
            }

            Debug.Assert(false, statement.Kind().ToString());

            return statement;
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
                    node = node.WithOperatorToken(node.OperatorToken.WithLeadingTrivia(_triviaList));

                    _previous = node;
                }

                return base.VisitBinaryExpression(node);
            }
        }
    }
}
