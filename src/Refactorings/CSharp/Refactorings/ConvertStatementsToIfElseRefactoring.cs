// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ConvertStatementsToIfElseRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, StatementListSelection selectedStatements)
        {
            if (selectedStatements.Count < 2)
                return;

            int ifStatementCount = 0;

            for (int i = 0; i < selectedStatements.Count - 1; i++)
            {
                if (!(selectedStatements[i] is IfStatementSyntax ifStatement))
                    break;

                foreach (IfStatementOrElseClause ifOrElse in ifStatement.AsCascade())
                {
                    if (ifOrElse.IsElse)
                        return;

                    StatementSyntax statement = ifOrElse.Statement;

                    if (statement is BlockSyntax block)
                        statement = block.Statements.LastOrDefault();

                    if (statement == null)
                        return;

                    if (!statement.IsKind(
                        SyntaxKind.ReturnStatement,
                        SyntaxKind.ContinueStatement,
                        SyntaxKind.BreakStatement,
                        SyntaxKind.ThrowStatement))
                    {
                        return;
                    }
                }

                ifStatementCount++;
            }

            if (ifStatementCount == 0)
                return;

            Document document = context.Document;

            context.RegisterRefactoring(
                "Convert to if-else",
                ct => RefactorAsync(document, selectedStatements, ifStatementCount, ct),
                RefactoringIdentifiers.ConvertStatementsToIfElse);
        }

        private static Task<Document> RefactorAsync(
            Document document,
            StatementListSelection selectedStatements,
            int ifStatementCount,
            CancellationToken cancellationToken)
        {
            IfStatementSyntax newIfStatement = null;

            for (int i = ifStatementCount - 1; i >= 0; i--)
            {
                var ifStatement = (IfStatementSyntax)selectedStatements[i];

                IfStatementSyntax lastIf = ifStatement.GetCascadeInfo().Last.AsIf();

                StatementSyntax elseStatement = newIfStatement;

                if (elseStatement == null)
                {
                    if (selectedStatements.Count - ifStatementCount > 1)
                    {
                        elseStatement = Block(selectedStatements.Skip(ifStatementCount));
                    }
                    else
                    {
                        elseStatement = selectedStatements.Last();

                        if (!elseStatement.IsKind(SyntaxKind.IfStatement)
                            && lastIf.Statement.IsKind(SyntaxKind.Block))
                        {
                            elseStatement = Block(elseStatement);
                        }
                    }
                }

                StatementSyntax newStatement = lastIf.Statement;

                if (!newStatement.IsKind(SyntaxKind.Block))
                {
                    if (elseStatement.IsKind(SyntaxKind.Block))
                    {
                        newStatement = Block(newStatement);
                    }
                    else if (elseStatement.IsKind(SyntaxKind.IfStatement)
                        && ((IfStatementSyntax)elseStatement).AsCascade().All(f => f.Statement.IsKind(SyntaxKind.Block)))
                    {
                        newStatement = Block(newStatement);
                    }
                }

                IfStatementSyntax newLastIf = lastIf.Update(
                    lastIf.IfKeyword,
                    lastIf.OpenParenToken,
                    lastIf.Condition,
                    lastIf.CloseParenToken,
                    newStatement,
                    ElseClause(elseStatement));

                newIfStatement = ifStatement.ReplaceNode(lastIf, newLastIf);
            }

            SyntaxList<StatementSyntax> newStatements = selectedStatements.UnderlyingList
                .Replace(selectedStatements.First(), newIfStatement.WithFormatterAnnotation())
                .RemoveRange(selectedStatements.FirstIndex + 1, selectedStatements.Count - 1);

            return document.ReplaceStatementsAsync(SyntaxInfo.StatementListInfo(selectedStatements), newStatements, cancellationToken);
        }
    }
}
