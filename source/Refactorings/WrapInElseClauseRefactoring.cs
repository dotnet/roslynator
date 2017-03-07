// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

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

                                IfElseChain chain = IfElseChain.Create(ifStatement);

                                if (chain.EndsWithIf
                                    && chain
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

            IEnumerable<IfStatementSyntax> chain = IfElseChain.GetChain(ifStatement).Cast<IfStatementSyntax>();

            StatementSyntax statement = returnStatement;

            if (chain.Any(f => f.Statement?.IsKind(SyntaxKind.Block) == true))
                statement = Block(statement);

            ElseClauseSyntax elseClause = ElseClause(statement).WithFormatterAnnotation();

            IfStatementSyntax lastIfStatement = chain.Last();

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