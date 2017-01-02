// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings.ReplaceIfWithStatement
{
    internal static class ReplaceIfAndReturnWithReturnRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, SelectedStatementCollection selectedStatements)
        {
            if (selectedStatements.Count == 2)
            {
                StatementSyntax[] statements = selectedStatements.ToArray();

                if (statements[0].IsKind(SyntaxKind.IfStatement)
                    && statements[1].IsKind(SyntaxKind.ReturnStatement))
                {
                    var ifStatement = (IfStatementSyntax)statements[0];

                    if (!IfElseChain.IsPartOfChain(ifStatement))
                    {
                        ExpressionSyntax returnExpression = ReplaceIfWithStatementRefactoring.GetReturnExpression(ifStatement);

                        if (returnExpression != null)
                        {
                            var returnStatement = (ReturnStatementSyntax)statements[1];
                            ExpressionSyntax returnExpression2 = returnStatement.Expression;

                            if (returnExpression2 != null)
                            {
                                const string title = "Replace if-return with return";

                                if (returnExpression.IsBooleanLiteralExpression()
                                    || returnExpression2.IsBooleanLiteralExpression())
                                {
                                    context.RegisterRefactoring(
                                        title,
                                        cancellationToken => RefactorAsync(context.Document, selectedStatements.Container, ifStatement, returnStatement, cancellationToken));
                                }
                                else
                                {
                                    context.RegisterRefactoring(
                                        title,
                                        cancellationToken =>
                                        {
                                            return RefactorAsync(
                                                context.Document,
                                                selectedStatements.Container,
                                                ifStatement,
                                                returnStatement,
                                                returnExpression,
                                                returnExpression2,
                                                cancellationToken);
                                        });
                                }
                            }
                        }
                    }
                }
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            StatementContainer container,
            IfStatementSyntax ifStatement,
            ReturnStatementSyntax returnStatement,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax expression = ReplaceIfWithStatementRefactoring.GetExpression(
                ifStatement.Condition,
                ReplaceIfWithStatementRefactoring.GetReturnExpression(ifStatement),
                returnStatement.Expression);

            ReturnStatementSyntax newReturnStatement = ReturnStatement(expression);

            SyntaxList<StatementSyntax> statements = container.Statements;

            int index = statements.IndexOf(ifStatement);

            newReturnStatement = newReturnStatement
                .WithLeadingTrivia(ifStatement.GetLeadingTrivia())
                .WithTrailingTrivia(returnStatement.GetTrailingTrivia())
                .WithFormatterAnnotation();

            SyntaxList<StatementSyntax> newStatements = statements
                .RemoveAt(index)
                .ReplaceAt(index, newReturnStatement);

            return await document.ReplaceNodeAsync(container.Node, container.NodeWithStatements(newStatements), cancellationToken).ConfigureAwait(false);
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            StatementContainer container,
            IfStatementSyntax ifStatement,
            ReturnStatementSyntax returnStatement,
            ExpressionSyntax whenTrue,
            ExpressionSyntax whenFalse,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxList<StatementSyntax> statements = container.Statements;

            int index = statements.IndexOf(ifStatement);

            ConditionalExpressionSyntax conditionalExpression = ReplaceIfWithStatementRefactoring.CreateConditionalExpression(ifStatement.Condition, whenTrue, whenFalse);

            ReturnStatementSyntax newReturnStatement = ReturnStatement(conditionalExpression)
                .WithLeadingTrivia(ifStatement.GetLeadingTrivia())
                .WithTrailingTrivia(returnStatement.GetTrailingTrivia())
                .WithFormatterAnnotation();

            SyntaxList<StatementSyntax> newStatements = statements
                .Remove(returnStatement)
                .ReplaceAt(index, newReturnStatement);

            return await document.ReplaceNodeAsync(container.Node, container.NodeWithStatements(newStatements), cancellationToken).ConfigureAwait(false);
        }
    }
}
