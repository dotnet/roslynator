// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.ReplaceStatementWithIf
{
    internal abstract class ReplaceStatementWithIfStatementRefactoring<TStatement> where TStatement : StatementSyntax
    {
        protected abstract ExpressionSyntax GetExpression(TStatement statement);

        protected abstract TStatement SetExpression(TStatement statement, ExpressionSyntax expression);

        protected abstract string GetTitle(TStatement statement);

        public async Task ComputeRefactoringAsync(RefactoringContext context, TStatement statement)
        {
            ExpressionSyntax expression = GetExpression(statement);

            if (expression != null)
            {
                switch (expression.Kind())
                {
                    case SyntaxKind.TrueLiteralExpression:
                    case SyntaxKind.FalseLiteralExpression:
                        {
                            break;
                        }
                    case SyntaxKind.ConditionalExpression:
                        {
                            context.RegisterRefactoring(
                                GetTitle(statement),
                                cancellationToken => RefactorAsync(context.Document, statement, (ConditionalExpressionSyntax)expression, cancellationToken));

                            break;
                        }
                    default:
                        {
                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            if (semanticModel
                                .GetTypeInfo(expression, context.CancellationToken)
                                .ConvertedType?
                                .IsBoolean() == true)
                            {
                                context.RegisterRefactoring(
                                    GetTitle(statement),
                                    cancellationToken => RefactorAsync(context.Document, statement, expression, cancellationToken));
                            }

                            break;
                        }
                }
            }
        }

        private Task<Document> RefactorAsync(
            Document document,
            TStatement statement,
            ExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            IfStatementSyntax ifStatement = CreateIfStatement(statement, expression)
                .WithTriviaFrom(statement)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(statement, ifStatement, cancellationToken);
        }

        private IfStatementSyntax CreateIfStatement(TStatement statement, ExpressionSyntax expression)
        {
            if (expression.IsKind(SyntaxKind.LogicalOrExpression))
            {
                var binaryExpression = (BinaryExpressionSyntax)expression;

                ExpressionSyntax left = binaryExpression.Left;

                if (left?.IsKind(SyntaxKind.LogicalOrExpression) == false)
                {
                    ExpressionSyntax right = binaryExpression.Right;

                    if (right != null)
                        return CreateIfStatement(statement, left, TrueLiteralExpression(), right.WithoutTrivia());
                }
            }

            return CreateIfStatement(statement, expression, TrueLiteralExpression(), FalseLiteralExpression());
        }

        private IfStatementSyntax CreateIfStatement(TStatement statement, ExpressionSyntax condition, ExpressionSyntax left, ExpressionSyntax right)
        {
            return IfStatement(
                condition,
                CreateBlock(statement, left),
                ElseClause(
                    CreateBlock(statement, right)));
        }

        private BlockSyntax CreateBlock(TStatement statement, ExpressionSyntax expression)
        {
            return Block(SetExpression(statement.WithoutLeadingTrivia(), expression));
        }

        private Task<Document> RefactorAsync(
            Document document,
            TStatement statement,
            ConditionalExpressionSyntax conditionalExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ExpressionSyntax condition = conditionalExpression.Condition.WithoutTrivia();

            if (condition.IsKind(SyntaxKind.ParenthesizedExpression))
                condition = ((ParenthesizedExpressionSyntax)condition).Expression;

            IfStatementSyntax ifStatement = IfStatement(
                condition,
                CreateBlock(statement.WithoutLeadingTrivia(), conditionalExpression.WhenTrue.WithoutTrivia()),
                ElseClause(
                    CreateBlock(statement.WithoutLeadingTrivia(), conditionalExpression.WhenFalse.WithoutTrivia())));

            ifStatement = ifStatement
                .WithTriviaFrom(statement)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(statement, ifStatement, cancellationToken);
        }
    }
}
