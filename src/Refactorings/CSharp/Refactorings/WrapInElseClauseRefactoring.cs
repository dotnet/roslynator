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
        public static void ComputeRefactoring(RefactoringContext context, StatementListSelection selectedStatements)
        {
            StatementSyntax lastStatement = selectedStatements.Last();

            if (!lastStatement.IsKind(SyntaxKind.ReturnStatement))
                return;

            if (selectedStatements.FirstIndex == 0)
                return;

            var returnStatement = (ReturnStatementSyntax)lastStatement;

            ExpressionSyntax expression = returnStatement.Expression;

            if (expression == null)
                return;

            StatementSyntax prevStatement = selectedStatements.UnderlyingList[selectedStatements.FirstIndex - 1];

            if (!prevStatement.IsKind(SyntaxKind.IfStatement))
                return;

            var ifStatement = (IfStatementSyntax)prevStatement;

            foreach (IfStatementOrElseClause ifOrElse in ifStatement.AsCascade())
            {
                if (ifOrElse.IsElse)
                    return;

                if (!IsLastStatementReturnStatement(ifOrElse))
                    return;
            }

            context.RegisterRefactoring(
                "Wrap in else clause",
                cancellationToken => RefactorAsync(context.Document, ifStatement, selectedStatements, cancellationToken));
        }

        private static bool IsLastStatementReturnStatement(IfStatementSyntax ifStatement)
        {
            StatementSyntax statement = ifStatement.Statement;

            if (statement is BlockSyntax block)
            {
                return IsReturnStatementWithExpression(block.Statements.LastOrDefault());
            }
            else
            {
                return IsReturnStatementWithExpression(statement);
            }
        }

        private static bool IsReturnStatementWithExpression(StatementSyntax statement)
        {
            return statement is ReturnStatementSyntax returnStatement
                && returnStatement.Expression != null;
        }

        private static Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            StatementListSelection selectedStatements,
            CancellationToken cancellationToken)
        {
            StatementSyntax newStatement = null;

            if (selectedStatements.Count == 1
                && !ifStatement.AsCascade().Any(f => f.Statement?.Kind() == SyntaxKind.Block))
            {
                newStatement = selectedStatements.First();
            }
            else
            {
                newStatement = SyntaxFactory.Block(selectedStatements);
            }

            ElseClauseSyntax elseClause = SyntaxFactory.ElseClause(newStatement).WithFormatterAnnotation();

            IfStatementSyntax lastIfStatement = ifStatement.AsCascade().Last();

            IfStatementSyntax newIfStatement = ifStatement.ReplaceNode(
                lastIfStatement,
                lastIfStatement.WithElse(elseClause));

            SyntaxList<StatementSyntax> newStatements = selectedStatements.UnderlyingList.Replace(ifStatement, newIfStatement);

            for (int i = newStatements.Count - 1; i >= selectedStatements.FirstIndex; i--)
                newStatements = newStatements.RemoveAt(i);

            return document.ReplaceStatementsAsync(SyntaxInfo.StatementListInfo(selectedStatements), newStatements, cancellationToken);
        }
    }
}