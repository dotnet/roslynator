// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Analysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceIfElseWithConditionalExpressionRefactoring
    {
        private const string Title = "Replace if-else with ?:";

        public static void ComputeRefactoring(RefactoringContext context, IfStatementSyntax ifStatement)
        {
            if (IfElseAnalysis.IsTopmostIf(ifStatement))
            {
                ElseClauseSyntax elseClause = ifStatement.Else;

                if (elseClause != null)
                {
                    StatementSyntax statement2 = elseClause.Statement;

                    if (statement2?.IsKind(SyntaxKind.IfStatement) == false)
                    {
                        StatementSyntax statement1 = GetStatementOrDefault(ifStatement.Statement);

                        if (statement1 != null)
                        {
                            statement2 = GetStatementOrDefault(statement2);

                            if (statement2 != null)
                            {
                                SyntaxKind kind1 = statement1.Kind();
                                SyntaxKind kind2 = statement2.Kind();

                                if (kind1 == SyntaxKind.ReturnStatement)
                                {
                                    if (kind2 == SyntaxKind.ReturnStatement)
                                    {
                                        ComputeRefactoring(
                                           context,
                                           ifStatement,
                                           (ReturnStatementSyntax)statement1,
                                           (ReturnStatementSyntax)statement2);
                                    }
                                }
                                else if (kind1 == SyntaxKind.YieldReturnStatement)
                                {
                                    if (kind2 == SyntaxKind.YieldReturnStatement)
                                    {
                                        ComputeRefactoring(
                                           context,
                                           ifStatement,
                                           (YieldStatementSyntax)statement1,
                                           (YieldStatementSyntax)statement2);
                                    }
                                }
                                else if (kind1 == SyntaxKind.ExpressionStatement)
                                {
                                    if (kind2 == SyntaxKind.ExpressionStatement)
                                    {
                                        ComputeRefactoring(
                                           context,
                                           ifStatement,
                                           (ExpressionStatementSyntax)statement2,
                                           (ExpressionStatementSyntax)statement1);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void ComputeRefactoring(
            RefactoringContext context,
            IfStatementSyntax ifStatement,
            ReturnStatementSyntax returnStatement1,
            ReturnStatementSyntax returnStatement2)
        {
            ExpressionSyntax expression1 = returnStatement1.Expression;
            ExpressionSyntax expression2 = returnStatement2.Expression;

            if (expression1?.IsMissing == false
                && expression2?.IsMissing == false)
            {
                context.RegisterRefactoring(
                    Title,
                    cancellationToken =>
                    {
                        return ReplaceIfElseWithReturnStatementAsync(
                            context.Document,
                            ifStatement,
                            expression1,
                            expression2,
                            cancellationToken);
                    });
            }
        }

        private static void ComputeRefactoring(
            RefactoringContext context,
            IfStatementSyntax ifStatement,
            YieldStatementSyntax yieldStatement1,
            YieldStatementSyntax yieldStatement2)
        {
            ExpressionSyntax expression1 = yieldStatement1.Expression;
            ExpressionSyntax expression2 = yieldStatement2.Expression;

            if (expression1?.IsMissing == false
                && expression2?.IsMissing == false)
            {
                context.RegisterRefactoring(
                    Title,
                    cancellationToken =>
                    {
                        return ReplaceIfElseWithYieldReturnStatementAsync(
                            context.Document,
                            ifStatement,
                            expression1,
                            expression2,
                            cancellationToken);
                    });
            }
        }

        private static void ComputeRefactoring(
            RefactoringContext context,
            IfStatementSyntax ifStatement,
            ExpressionStatementSyntax expressionStatement1,
            ExpressionStatementSyntax expressionStatement2)
        {
            ExpressionSyntax expression1 = expressionStatement1.Expression;

            if (expression1?.IsKind(SyntaxKind.SimpleAssignmentExpression) == true)
            {
                ExpressionSyntax expression2 = expressionStatement2.Expression;

                if (expression2?.IsKind(SyntaxKind.SimpleAssignmentExpression) == true)
                {
                    var assignment1 = (AssignmentExpressionSyntax)expression1;
                    var assignment2 = (AssignmentExpressionSyntax)expression2;

                    ExpressionSyntax left1 = assignment1.Left;
                    ExpressionSyntax right1 = assignment1.Right;

                    if (left1?.IsMissing == false
                        && right1?.IsMissing == false)
                    {
                        ExpressionSyntax left2 = assignment2.Left;
                        ExpressionSyntax right2 = assignment2.Right;

                        if (left2?.IsMissing == false
                            && right2?.IsMissing == false
                            && left1.IsEquivalentTo(left2, topLevel: false))
                        {
                            context.RegisterRefactoring(
                                Title,
                                cancellationToken =>
                                {
                                    return ReplaceIfElseWithAssignmentAsync(
                                        context.Document,
                                        ifStatement,
                                        left1,
                                        right1,
                                        right2,
                                        cancellationToken);
                                });
                        }
                    }
                }
            }
        }

        private static async Task<Document> ReplaceIfElseWithReturnStatementAsync(
            Document document,
            IfStatementSyntax ifStatement,
            ExpressionSyntax whenTrue,
            ExpressionSyntax whenFalse,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            ReturnStatementSyntax newNode = ReturnStatement(
                CreateConditionalExpression(ifStatement, whenTrue, whenFalse));

            newNode = newNode
                .WithTriviaFrom(ifStatement)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = root.ReplaceNode(ifStatement, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> ReplaceIfElseWithYieldReturnStatementAsync(
            Document document,
            IfStatementSyntax ifStatement,
            ExpressionSyntax whenTrue,
            ExpressionSyntax whenFalse,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            YieldStatementSyntax newNode = YieldReturnStatement(
                CreateConditionalExpression(ifStatement, whenTrue, whenFalse));

            newNode = newNode
                .WithTriviaFrom(ifStatement)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = root.ReplaceNode(ifStatement, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static async Task<Document> ReplaceIfElseWithAssignmentAsync(
            Document document,
            IfStatementSyntax ifStatement,
            ExpressionSyntax left,
            ExpressionSyntax whenTrue,
            ExpressionSyntax whenFalse,
            CancellationToken cancellationToken)
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            ExpressionStatementSyntax newNode = ExpressionStatement(
                SimpleAssignmentExpression(
                    left,
                    CreateConditionalExpression(ifStatement, whenTrue, whenFalse)));

            newNode = newNode
                .WithTriviaFrom(ifStatement)
                .WithFormatterAnnotation();

            SyntaxNode newRoot = root.ReplaceNode(ifStatement, newNode);

            return document.WithSyntaxRoot(newRoot);
        }

        private static ConditionalExpressionSyntax CreateConditionalExpression(
            IfStatementSyntax ifStatement,
            ExpressionSyntax whenTrue,
            ExpressionSyntax whenFalse)
        {
            ExpressionSyntax condition = ifStatement.Condition;

            if (!condition.IsKind(SyntaxKind.ParenthesizedExpression))
            {
                condition = ParenthesizedExpression(condition.WithoutTrivia())
                    .WithTriviaFrom(condition);
            }

            return ConditionalExpression(condition, whenTrue, whenFalse);
        }

        private static StatementSyntax GetStatementOrDefault(StatementSyntax statement)
        {
            if (statement.IsKind(SyntaxKind.Block))
            {
                var block = (BlockSyntax)statement;

                SyntaxList<StatementSyntax> statements = block.Statements;

                if (statements.Count == 1)
                    return statements[0];
            }

            return statement;
        }
    }
}
