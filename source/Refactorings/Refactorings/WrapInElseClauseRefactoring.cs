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
        public static void ComputeRefactoring(RefactoringContext context, StatementContainerSelection selectedStatements)
        {
            StatementSyntax lastStatement = selectedStatements.Last();

            if (lastStatement.IsKind(SyntaxKind.ReturnStatement)
                && selectedStatements.EndIndex == selectedStatements.Statements.IndexOf(lastStatement)
                && selectedStatements.StartIndex > 0)
            {
                var returnStatement = (ReturnStatementSyntax)lastStatement;

                ExpressionSyntax expression = returnStatement.Expression;

                if (expression != null)
                {
                    StatementSyntax prevStatement = selectedStatements.Statements[selectedStatements.StartIndex - 1];

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
                                "Wrap in else clause",
                                cancellationToken => RefactorAsync(context.Document, ifStatement, selectedStatements, cancellationToken));
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
            StatementContainerSelection selectedStatements,
            CancellationToken cancellationToken)
        {
            IfStatement ifElse = IfStatement.Create(ifStatement);

            StatementSyntax newStatement = null;

            if (selectedStatements.Count == 1
                && !ifElse.Nodes.Any(f => f.Statement?.IsKind(SyntaxKind.Block) == true))
            {
                newStatement = selectedStatements.First();
            }
            else
            {
                newStatement = SyntaxFactory.Block(selectedStatements);
            }

            ElseClauseSyntax elseClause = SyntaxFactory.ElseClause(newStatement).WithFormatterAnnotation();

            IfStatementSyntax lastIfStatement = ifElse.Nodes.Last();

            IfStatementSyntax newIfStatement = ifStatement.ReplaceNode(
                lastIfStatement,
                lastIfStatement.WithElse(elseClause));

            SyntaxList<StatementSyntax> newStatements = selectedStatements.Statements.Replace(ifStatement, newIfStatement);

            for (int i = newStatements.Count - 1; i >= selectedStatements.StartIndex; i--)
                newStatements = newStatements.RemoveAt(i);

            return document.ReplaceNodeAsync(selectedStatements.Container.Node, selectedStatements.Container.NodeWithStatements(newStatements), cancellationToken);
        }
    }
}