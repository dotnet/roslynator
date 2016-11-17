// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SimplifyIfElseStatementRefactoring
    {
        public static bool CanRefactor(
            IfStatementSyntax ifStatement,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (IfElseAnalysis.IsTopmostIf(ifStatement))
            {
                ElseClauseSyntax elseClause = ifStatement.Else;

                if (elseClause != null)
                {
                    ExpressionSyntax condition = ifStatement.Condition;

                    if (condition != null)
                    {
                        ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(condition, cancellationToken);

                        if (typeSymbol?.IsBoolean() == true)
                        {
                            AssignmentExpressionSyntax trueExpression = GetSimpleAssignmentExpression(ifStatement.Statement);

                            ExpressionSyntax trueRight = trueExpression?.Right;

                            if (trueRight?.IsBooleanLiteralExpression() == true)
                            {
                                AssignmentExpressionSyntax falseExpression = GetSimpleAssignmentExpression(elseClause.Statement);

                                ExpressionSyntax falseRight = falseExpression?.Right;

                                if (falseRight?.IsBooleanLiteralExpression() == true)
                                {
                                    var trueBooleanLiteral = (LiteralExpressionSyntax)trueRight;
                                    var falseBooleanLiteral = (LiteralExpressionSyntax)falseRight;

                                    if (trueBooleanLiteral.IsKind(SyntaxKind.TrueLiteralExpression) != falseBooleanLiteral.IsKind(SyntaxKind.TrueLiteralExpression)
                                        && trueExpression.Left?.IsEquivalentTo(falseExpression.Left, topLevel: false) == true)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        private static AssignmentExpressionSyntax GetSimpleAssignmentExpression(StatementSyntax statement)
        {
            statement = GetStatement(statement);

            if (statement?.IsKind(SyntaxKind.ExpressionStatement) == true)
            {
                var expressionStatement = (ExpressionStatementSyntax)statement;

                ExpressionSyntax expression = expressionStatement.Expression;

                if (expression?.IsKind(SyntaxKind.SimpleAssignmentExpression) == true)
                    return (AssignmentExpressionSyntax)expression;
            }

            return null;
        }

        private static StatementSyntax GetStatement(StatementSyntax statement)
        {
            if (statement != null)
            {
                if (statement.IsKind(SyntaxKind.Block))
                {
                    var block = (BlockSyntax)statement;

                    SyntaxList<StatementSyntax> statements = block.Statements;

                    if (statements.Count == 1)
                        return statements.First();
                }
                else
                {
                    return statement;
                }
            }

            return null;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            ExpressionSyntax condition = ifStatement.Condition;

            AssignmentExpressionSyntax assignment = GetSimpleAssignmentExpression(ifStatement.Statement);

            if (assignment.Right.IsKind(SyntaxKind.FalseLiteralExpression))
                condition = LogicalNotExpression(condition.WithoutTrivia()).WithTriviaFrom(condition);

            ExpressionStatementSyntax newNode = SimpleAssignmentExpressionStatement(assignment.Left, condition)
                .WithTriviaFrom(ifStatement)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = root.ReplaceNode(ifStatement, newNode);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
