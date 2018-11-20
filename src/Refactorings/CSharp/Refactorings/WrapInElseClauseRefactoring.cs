// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class WrapInElseClauseRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, StatementListSelection selectedStatements)
        {
            if (selectedStatements.FirstIndex == 0)
                return;

            if (!CSharpFacts.IsJumpStatement(selectedStatements.Last().Kind()))
                return;

            StatementSyntax prevStatement = selectedStatements.UnderlyingList[selectedStatements.FirstIndex - 1];

            if (!(prevStatement is IfStatementSyntax ifStatement))
                return;

            foreach (IfStatementOrElseClause ifOrElse in ifStatement.AsCascade())
            {
                if (ifOrElse.IsElse)
                    return;

                StatementSyntax statement = ifOrElse.AsIf().Statement;

                if (statement is BlockSyntax block)
                    statement = block.Statements.LastOrDefault();

                if (statement == null)
                    return;

                if (!CSharpFacts.IsJumpStatement(statement.Kind()))
                    return;
            }

            context.RegisterRefactoring(
                "Wrap in else clause",
                cancellationToken => RefactorAsync(context.Document, ifStatement, selectedStatements, cancellationToken),
                RefactoringIdentifiers.WrapInElseClause);
        }

        private static Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            StatementListSelection selectedStatements,
            CancellationToken cancellationToken)
        {
            int count = selectedStatements.Count;

            StatementSyntax newStatement;

            if (count == 1
                && !ifStatement.AsCascade().Any(f => f.Statement?.Kind() == SyntaxKind.Block))
            {
                newStatement = selectedStatements.First();
            }
            else
            {
                newStatement = SyntaxFactory.Block(selectedStatements);
            }

            ElseClauseSyntax elseClause = SyntaxFactory.ElseClause(newStatement).WithFormatterAnnotation();

            IfStatementSyntax lastIfStatement = ifStatement.GetCascadeInfo().Last;

            IfStatementSyntax newIfStatement = ifStatement.ReplaceNode(
                lastIfStatement,
                lastIfStatement.WithElse(elseClause));

            SyntaxList<StatementSyntax> newStatements = selectedStatements
                .UnderlyingList
                .Replace(ifStatement, newIfStatement)
                .RemoveRange(selectedStatements.FirstIndex, count);

            return document.ReplaceStatementsAsync(SyntaxInfo.StatementListInfo(selectedStatements), newStatements, cancellationToken);
        }
    }
}