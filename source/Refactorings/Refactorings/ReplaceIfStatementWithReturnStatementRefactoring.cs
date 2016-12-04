// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceIfStatementWithReturnStatementRefactoring
    {
        private const string Title = "Replace if with return statement";

        public static void ComputeRefactoring(RefactoringContext context, IfStatementSyntax ifStatement)
        {
            ElseClauseSyntax @else = ifStatement.Else;

            if (@else != null)
            {
                ExpressionSyntax expression1 = GetReturnExpression(ifStatement);

                if (expression1 != null)
                {
                    ExpressionSyntax expression2 = GetReturnExpression(@else.Statement);

                    if (expression2 != null)
                    {
                        if (expression1.IsBooleanLiteralExpression()
                            || expression2.IsBooleanLiteralExpression())
                        {
                            context.RegisterRefactoring(
                                Title,
                                cancellationToken => RefactorAsync(context.Document, ifStatement, cancellationToken));
                        }
                    }
                }
            }
        }

        public static void ComputeRefactoring(RefactoringContext context, SelectedStatementsInfo info)
        {
            if (info.SelectedCount == 2)
            {
                StatementSyntax[] statements = info.SelectedNodes().ToArray();

                if (statements[0].IsKind(SyntaxKind.IfStatement)
                    && statements[1].IsKind(SyntaxKind.ReturnStatement))
                {
                    var ifStatement = (IfStatementSyntax)statements[0];

                    if (!IfElseChain.IsPartOfChain(ifStatement))
                    {
                        ExpressionSyntax returnExpression = GetReturnExpression(ifStatement.Statement);

                        if (returnExpression != null)
                        {
                            var returnStatement = (ReturnStatementSyntax)statements[1];
                            ExpressionSyntax returnExpression2 = returnStatement.Expression;

                            if (returnExpression2 != null)
                            {
                                if (returnExpression.IsBooleanLiteralExpression()
                                    || returnExpression2.IsBooleanLiteralExpression())
                                {
                                    context.RegisterRefactoring(
                                        Title,
                                        cancellationToken => RefactorAsync(context.Document, ifStatement, returnStatement, cancellationToken));
                                }
                            }
                        }
                    }
                }
            }
        }

        private static ExpressionSyntax GetReturnExpression(IfStatementSyntax ifStatement)
        {
            return GetReturnExpression(ifStatement.Statement);
        }

        private static ExpressionSyntax GetReturnExpression(StatementSyntax statement)
        {
            return GetReturnStatement(statement)?.Expression;
        }

        private static ReturnStatementSyntax GetReturnStatement(StatementSyntax statement)
        {
            switch (statement?.Kind())
            {
                case SyntaxKind.Block:
                    {
                        var block = (BlockSyntax)statement;
                        SyntaxList<StatementSyntax> statements = block.Statements;

                        if (statements.Count == 1)
                        {
                            StatementSyntax firstStatement = statements[0];

                            if (firstStatement.IsKind(SyntaxKind.ReturnStatement))
                                return (ReturnStatementSyntax)firstStatement;
                        }

                        break;
                    }
                case SyntaxKind.ReturnStatement:
                    {
                        return (ReturnStatementSyntax)statement;
                    }
            }

            return null;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax expression = GetExpression(
                ifStatement.Condition,
                GetReturnExpression(ifStatement),
                GetReturnExpression(ifStatement.Else.Statement));

            ReturnStatementSyntax newReturnStatement = ReturnStatement(expression)
                .WithTriviaFrom(ifStatement)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(ifStatement, newReturnStatement, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            ReturnStatementSyntax returnStatement,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax expression = GetExpression(
                ifStatement.Condition,
                GetReturnExpression(ifStatement),
                returnStatement.Expression);

            ReturnStatementSyntax newReturnStatement = ReturnStatement(expression);

            var block = (BlockSyntax)ifStatement.Parent;

            SyntaxList<StatementSyntax> statements = block.Statements;

            int index = statements.IndexOf(ifStatement);

            newReturnStatement = newReturnStatement
                .WithLeadingTrivia(ifStatement.GetLeadingTrivia())
                .WithTrailingTrivia(returnStatement.GetTrailingTrivia())
                .WithFormatterAnnotation();

            SyntaxList<StatementSyntax> newStatements = statements
                .RemoveAt(index)
                .ReplaceAt(index, newReturnStatement);

            return await document.ReplaceNodeAsync(block, block.WithStatements(newStatements)).ConfigureAwait(false);
        }

        public static ExpressionSyntax GetExpression(ExpressionSyntax condition, ExpressionSyntax expression1, ExpressionSyntax expression2)
        {
            switch (expression1.Kind())
            {
                case SyntaxKind.TrueLiteralExpression:
                    {
                        switch (expression2.Kind())
                        {
                            case SyntaxKind.TrueLiteralExpression:
                                return expression2;
                            case SyntaxKind.FalseLiteralExpression:
                                return condition;
                            default:
                                return LogicalOrExpression(condition, expression2);
                        }
                    }
                case SyntaxKind.FalseLiteralExpression:
                    {
                        switch (expression2.Kind())
                        {
                            case SyntaxKind.TrueLiteralExpression:
                                return condition.Negate();
                            case SyntaxKind.FalseLiteralExpression:
                                return expression2;
                            default:
                                return LogicalOrExpression(condition.Negate(), expression2);
                        }
                    }
                default:
                    {
                        switch (expression2.Kind())
                        {
                            case SyntaxKind.TrueLiteralExpression:
                                return LogicalOrExpression(condition.Negate(), expression1);
                            case SyntaxKind.FalseLiteralExpression:
                                return LogicalAndExpression(condition, expression1, addParenthesesIfNecessary: true);
                            default:
                                {
                                    Debug.Assert(false);
                                    return null;
                                }
                        }
                    }
            }
        }
    }
}
