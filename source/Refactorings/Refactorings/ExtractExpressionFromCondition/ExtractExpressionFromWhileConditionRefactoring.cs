// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ExtractExpressionFromWhileConditionRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, ExpressionSyntax expression)
        {
            if (context.Span.IsBetweenSpans(expression))
            {
                SyntaxNode parent = expression.Parent;

                if (parent?.IsKind(SyntaxKind.LogicalAndExpression) == true)
                {
                    BinaryExpressionSyntax binaryExpression = ExtractExpressionFromConditionRefactoring.GetCondition((BinaryExpressionSyntax)parent, SyntaxKind.WhileStatement);

                    if (binaryExpression?.IsKind(SyntaxKind.LogicalAndExpression) == true)
                    {
                        context.RegisterRefactoring(
                            $"Extract '{expression}'",
                            cancellationToken => RefactorAsync(context.Document, binaryExpression, expression, cancellationToken));
                    }
                }
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax condition,
            ExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var whileStatement = (WhileStatementSyntax)condition.Parent;

            WhileStatementSyntax newWhileStatement = whileStatement.ReplaceNode(
                expression.Parent,
                ExtractExpressionFromConditionRefactoring.GetNewCondition(condition, expression));

            newWhileStatement = newWhileStatement.WithFormatterAnnotation();

            root = root.ReplaceNode(
                whileStatement,
                ExtractExpressionToNestedIf(expression, whileStatement, newWhileStatement));

            return document.WithSyntaxRoot(root);
        }

        private static WhileStatementSyntax ExtractExpressionToNestedIf(
            ExpressionSyntax expression,
            WhileStatementSyntax whileStatement,
            WhileStatementSyntax newWhileStatement)
        {
            if (newWhileStatement.Statement.IsKind(SyntaxKind.Block))
            {
                var block = (BlockSyntax)newWhileStatement.Statement;

                IfStatementSyntax nestedIf = IfStatement(
                    expression.WithoutTrivia(),
                    Block(block.Statements));

                return newWhileStatement.ReplaceNode(
                    block,
                    block.WithStatements(SingletonList<StatementSyntax>(nestedIf)));
            }
            else
            {
                IfStatementSyntax nestedIf = IfStatement(
                    expression.WithoutTrivia(),
                    whileStatement.Statement.WithoutTrivia());

                BlockSyntax block = Block(nestedIf).WithTriviaFrom(whileStatement.Statement);

                return newWhileStatement.WithStatement(block);
            }
        }
    }
}
