// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings.ExtractCondition
{
    internal abstract class ExtractConditionRefactoring<TStatement> where TStatement : StatementSyntax
    {
        public abstract SyntaxKind StatementKind { get; }

        public abstract string Title { get; }

        public abstract StatementSyntax GetStatement(TStatement statement);

        public abstract TStatement SetStatement(TStatement statement, StatementSyntax newStatement);

        protected static TStatement RemoveExpressionFromCondition(
            TStatement statement,
            BinaryExpressionSyntax condition,
            ExpressionSyntax expression)
        {
            return statement.ReplaceNode(
                expression.Parent,
                GetNewCondition(condition, expression));
        }

        private static ExpressionSyntax GetNewCondition(
            ExpressionSyntax condition,
            ExpressionSyntax expression)
        {
            var binaryExpression = (BinaryExpressionSyntax)expression.Parent;

            ExpressionSyntax left = binaryExpression.Left;

            if (expression == left)
            {
                return binaryExpression.Right;
            }
            else
            {
                return (binaryExpression == condition)
                    ? left.TrimTrailingTrivia()
                    : left;
            }
        }

        protected static TStatement RemoveExpressionsFromCondition(
            TStatement statement,
            BinaryExpressionSyntax condition,
            BinaryExpressionSelection binaryExpressionSelection)
        {
            var binaryExpression = (BinaryExpressionSyntax)binaryExpressionSelection.Expressions[0].Parent;

            return statement.ReplaceNode(
                condition,
                binaryExpression.Left.TrimTrailingTrivia());
        }

        protected TStatement AddNestedIf(
            TStatement statement,
            BinaryExpressionSelection binaryExpressionSelection)
        {
            ExpressionSyntax expression = ParseExpression(binaryExpressionSelection.ToString());

            return AddNestedIf(statement, expression);
        }

        protected TStatement AddNestedIf(
            TStatement statement,
            ExpressionSyntax expression)
        {
            StatementSyntax childStatement = GetStatement(statement);

            if (childStatement.IsKind(SyntaxKind.Block))
            {
                var block = (BlockSyntax)childStatement;

                IfStatementSyntax nestedIf = IfStatement(
                    expression.WithoutTrivia(),
                    Block(block.Statements));

                return statement.ReplaceNode(
                    block,
                    block.WithStatements(SingletonList<StatementSyntax>(nestedIf)));
            }
            else
            {
                IfStatementSyntax nestedIf = IfStatement(
                    expression.WithoutTrivia(),
                    childStatement.WithoutTrivia());

                BlockSyntax block = Block(nestedIf).WithTriviaFrom(childStatement);

                return SetStatement(statement, block);
            }
        }
    }
}
