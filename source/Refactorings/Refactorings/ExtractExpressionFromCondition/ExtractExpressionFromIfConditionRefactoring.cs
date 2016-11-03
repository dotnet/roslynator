// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ExtractExpressionFromIfConditionRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, ExpressionSyntax expression)
        {
            if (context.Span.IsBetweenSpans(expression))
            {
                SyntaxNode parent = expression.Parent;

                if (parent?.IsKind(SyntaxKind.LogicalAndExpression, SyntaxKind.LogicalOrExpression) == true)
                {
                    BinaryExpressionSyntax binaryExpression = GetCondition((BinaryExpressionSyntax)parent);

                    if (binaryExpression != null)
                    {
                        context.RegisterRefactoring(
                            $"Extract '{expression}'",
                            cancellationToken => RefactorAsync(context.Document, binaryExpression, expression, cancellationToken));
                    }
                }
            }
        }

        private static BinaryExpressionSyntax GetCondition(BinaryExpressionSyntax parent)
        {
            BinaryExpressionSyntax binaryExpression = ExtractExpressionFromConditionRefactoring.GetCondition(parent, SyntaxKind.IfStatement);

            if (binaryExpression != null)
            {
                if (binaryExpression.IsKind(SyntaxKind.LogicalAndExpression)
                    || binaryExpression.Parent.Parent?.IsKind(SyntaxKind.Block) == true)
                {
                    return binaryExpression;
                }
            }

            return null;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax condition,
            ExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var ifStatement = (IfStatementSyntax)condition.Parent;

            IfStatementSyntax newIfStatement = ifStatement.ReplaceNode(
                expression.Parent,
                ExtractExpressionFromConditionRefactoring.GetNewCondition(condition, expression));

            newIfStatement = newIfStatement.WithFormatterAnnotation();

            if (condition.IsKind(SyntaxKind.LogicalAndExpression))
            {
                root = root.ReplaceNode(
                    ifStatement,
                    ExtractExpressionToNestedIf(expression, ifStatement, newIfStatement));
            }
            else if (condition.IsKind(SyntaxKind.LogicalOrExpression))
            {
                root = root.ReplaceNode(
                    ifStatement.Parent,
                    ExtractExpressionToIf(expression, ifStatement, newIfStatement));
            }

            return document.WithSyntaxRoot(root);
        }

        private static IfStatementSyntax ExtractExpressionToNestedIf(
            ExpressionSyntax expression,
            IfStatementSyntax ifStatement,
            IfStatementSyntax newIfStatement)
        {
            if (newIfStatement.Statement.IsKind(SyntaxKind.Block))
            {
                var block = (BlockSyntax)newIfStatement.Statement;

                IfStatementSyntax nestedIf = IfStatement(
                    expression.WithoutTrivia(),
                    Block(block.Statements));

                return newIfStatement.ReplaceNode(
                    block,
                    block.WithStatements(SingletonList<StatementSyntax>(nestedIf)));
            }
            else
            {
                IfStatementSyntax nestedIf = IfStatement(
                    expression.WithoutTrivia(),
                    ifStatement.Statement.WithoutTrivia());

                BlockSyntax block = Block(nestedIf).WithTriviaFrom(ifStatement.Statement);

                return newIfStatement.WithStatement(block);
            }
        }

        private static BlockSyntax ExtractExpressionToIf(
            ExpressionSyntax expression,
            IfStatementSyntax ifStatement,
            IfStatementSyntax newIfStatement)
        {
            IfStatementSyntax ifStatement2 = ifStatement
                .WithCondition(expression)
                .WithFormatterAnnotation();

            var parentBlock = (BlockSyntax)ifStatement.Parent;

            int index = parentBlock.Statements.IndexOf(ifStatement);

            return parentBlock
                .WithStatements(parentBlock.Statements
                    .Replace(ifStatement, newIfStatement)
                    .Insert(index + 1, ifStatement2));
        }
    }
}
