// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class WrapInElseClauseRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, ReturnStatementSyntax returnStatement)
        {
            ExpressionSyntax expression = returnStatement.Expression;

            if (expression != null)
            {
                StatementContainer container;
                if (StatementContainer.TryCreate(returnStatement, out container))
                {
                    SyntaxList<StatementSyntax> statements = container.Statements;

                    if (statements.Count > 1)
                    {
                        int index = statements.IndexOf(returnStatement);

                        if (index == statements.Count - 1)
                        {
                            StatementSyntax prevStatement = statements[index - 1];

                            if (prevStatement.IsKind(SyntaxKind.IfStatement))
                            {
                                var ifStatement = (IfStatementSyntax)prevStatement;

                                IfStatement ifElse = IfStatement.Create(ifStatement);

                                if (ifElse.EndsWithIf
                                    && ifElse
                                        .Nodes
                                        .Where(f => f.IsIf)
                                        .All(f => IsLastStatementReturnStatement(f)))
                                {
                                    context.RegisterRefactoring(
                                        "Wrap in else",
                                        cancellationToken => RefactorAsync(context.Document, ifStatement, returnStatement, container, cancellationToken));
                                }
                            }
                        }
                    }
                }
            }
        }

        private static bool IsLastStatementReturnStatement(IfStatementSyntax ifStatement)
        {
            StatementSyntax statement = ifStatement.Statement;

            if (statement.IsKind(SyntaxKind.Block))
            {
                var block = (BlockSyntax)statement;

                return IsReturnStatementWithExpression(block.Statements.LastOrDefault());
            }
            else
            {
                return IsReturnStatementWithExpression(statement);
            }
        }

        private static bool IsReturnStatementWithExpression(StatementSyntax statement)
        {
            if (statement?.IsKind(SyntaxKind.ReturnStatement) == true)
            {
                var returnStatement = (ReturnStatementSyntax)statement;

                return returnStatement.Expression != null;
            }

            return false;
        }

        private static Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            ReturnStatementSyntax returnStatement,
            StatementContainer container,
            CancellationToken cancellationToken)
        {
            SyntaxList<StatementSyntax> statements = container.Statements;

            IfStatement ifElse = IfStatement.Create(ifStatement);

            StatementSyntax statement = returnStatement;

            if (ifElse.Nodes.Any(f => f.Statement?.IsKind(SyntaxKind.Block) == true))
                statement = SyntaxFactory.Block(statement);

            ElseClauseSyntax elseClause = SyntaxFactory.ElseClause(statement).WithFormatterAnnotation();

            IfStatementSyntax lastIfStatement = ifElse.Nodes.Last();

            IfStatementSyntax newIfStatement = ifStatement.ReplaceNode(
                lastIfStatement,
                lastIfStatement.WithElse(elseClause));

            SyntaxList<StatementSyntax> newStatements = statements
                .Replace(ifStatement, newIfStatement)
                .RemoveAt(statements.IndexOf(returnStatement));

            return document.ReplaceNodeAsync(container.Node, container.NodeWithStatements(newStatements), cancellationToken);
        }
    }
}