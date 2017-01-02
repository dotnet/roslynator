// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.ReplaceIfWithStatement
{
    internal static class ReplaceIfElseWithAssignmentRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, IfStatementSyntax ifStatement)
        {
            if (IfElseChain.IsTopmostIf(ifStatement))
            {
                ElseClauseSyntax elseClause = ifStatement.Else;

                if (elseClause != null)
                {
                    StatementSyntax statement1 = ReplaceIfWithStatementRefactoring.GetStatement(ifStatement);

                    if (statement1?.IsKind(SyntaxKind.ExpressionStatement) == true)
                    {
                        StatementSyntax statement2 = ReplaceIfWithStatementRefactoring.GetStatement(elseClause);

                        if (statement2?.IsKind(SyntaxKind.ExpressionStatement) == true)
                        {
                            var expressionStatement1 = (ExpressionStatementSyntax)statement1;
                            var expressionStatement2 = (ExpressionStatementSyntax)statement2;

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
                                                "Replace if-else with assignment",
                                                cancellationToken =>
                                                {
                                                    return RefactorAsync(
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
                    }
                }
            }
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            ExpressionSyntax left,
            ExpressionSyntax whenTrue,
            ExpressionSyntax whenFalse,
            CancellationToken cancellationToken)
        {
            ConditionalExpressionSyntax conditionalExpression = ReplaceIfWithStatementRefactoring.CreateConditionalExpression(ifStatement.Condition, whenTrue, whenFalse);

            ExpressionStatementSyntax newNode = ExpressionStatement(
                SimpleAssignmentExpression(left, conditionalExpression));

            newNode = newNode
                .WithTriviaFrom(ifStatement)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(ifStatement, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}
