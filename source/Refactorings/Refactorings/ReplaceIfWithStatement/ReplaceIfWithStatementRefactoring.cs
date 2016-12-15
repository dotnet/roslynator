// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.ReplaceIfWithStatement
{
    internal static class ReplaceIfWithStatementRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, IfStatementSyntax ifStatement)
        {
            StatementSyntax statement = GetStatement(ifStatement);

            SyntaxKind kind = statement.Kind();

            if (kind == SyntaxKind.ReturnStatement)
            {
                var refactoring = new ReplaceIfWithReturnRefactoring();
                refactoring.ComputeRefactoring(context, ifStatement);
            }
            else if (kind == SyntaxKind.YieldReturnStatement)
            {
                var refactoring = new ReplaceIfWithYieldReturnRefactoring();
                refactoring.ComputeRefactoring(context, ifStatement);
            }
        }

        public static StatementSyntax GetStatement(IfStatementSyntax ifStatement)
        {
            return GetStatement(ifStatement.Statement);
        }

        public static StatementSyntax GetStatement(ElseClauseSyntax elseClause)
        {
            return GetStatement(elseClause.Statement);
        }

        private static StatementSyntax GetStatement(StatementSyntax statement)
        {
            if (statement?.IsKind(SyntaxKind.Block) == true)
            {
                var block = (BlockSyntax)statement;

                SyntaxList<StatementSyntax> statements = block.Statements;

                if (statements.Count == 1)
                {
                    return statements[0];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return statement;
            }
        }

        public static ExpressionSyntax GetReturnExpression(IfStatementSyntax ifStatement)
        {
            StatementSyntax statement = GetStatement(ifStatement);

            if (statement.IsKind(SyntaxKind.ReturnStatement))
                return ((ReturnStatementSyntax)statement).Expression;

            return null;
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
                                return condition.LogicallyNegate();
                            case SyntaxKind.FalseLiteralExpression:
                                return expression2;
                            default:
                                return LogicalOrExpression(condition.LogicallyNegate(), expression2);
                        }
                    }
                default:
                    {
                        switch (expression2.Kind())
                        {
                            case SyntaxKind.TrueLiteralExpression:
                                return LogicalOrExpression(condition.LogicallyNegate(), expression1);
                            case SyntaxKind.FalseLiteralExpression:
                                return LogicalAndExpression(condition, expression1, addParenthesesIfNecessary: true);
                            default:
                                return LogicalOrExpression(ParenthesizedExpression(LogicalAndExpression(condition, expression1)), expression2);
                        }
                    }
            }
        }

        public static ConditionalExpressionSyntax CreateConditionalExpression(ExpressionSyntax condition, ExpressionSyntax whenTrue, ExpressionSyntax whenFalse)
        {
            if (!condition.IsKind(SyntaxKind.ParenthesizedExpression))
            {
                condition = ParenthesizedExpression(condition.WithoutTrivia())
                    .WithTriviaFrom(condition);
            }

            return ConditionalExpression(condition, whenTrue, whenFalse);
        }
    }
}
