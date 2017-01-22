// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseCoalesceExpressionRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken)
        {
            StatementContainer container;

            if (StatementContainer.TryCreate(statement, out container))
            {
                SyntaxList<StatementSyntax> statements = container.Statements;

                int index = statements.IndexOf(statement);

                StatementSyntax previousStatement = statements[index - 1];

                switch (previousStatement.Kind())
                {
                    case SyntaxKind.ExpressionStatement:
                        {
                            var expressionStatement = (ExpressionStatementSyntax)previousStatement;

                            var assignment = (AssignmentExpressionSyntax)expressionStatement.Expression;

                            return await RefactorAsync(document, expressionStatement, (IfStatementSyntax)statement, index - 1, container, assignment.Right, cancellationToken).ConfigureAwait(false);
                        }
                    case SyntaxKind.LocalDeclarationStatement:
                        {
                            var localDeclaration = (LocalDeclarationStatementSyntax)previousStatement;

                            ExpressionSyntax value = localDeclaration
                                .Declaration
                                .Variables
                                .First()
                                .Initializer
                                .Value;

                            return await RefactorAsync(document, localDeclaration, (IfStatementSyntax)statement, index - 1, container, value, cancellationToken).ConfigureAwait(false);
                        }
                    default:
                        {
                            Debug.Assert(false, previousStatement.Kind().ToString());
                            return document;
                        }
                }
            }

            Debug.Assert(false, "");

            return document;
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            StatementSyntax statement,
            IfStatementSyntax ifStatement,
            int ifStatementIndex,
            StatementContainer container,
            ExpressionSyntax expression,
            CancellationToken cancellationToken)
        {
            var expressionStatement = (ExpressionStatementSyntax)GetSingleStatementOrDefault(ifStatement.Statement);

            var assignment = (AssignmentExpressionSyntax)expressionStatement.Expression;

            ExpressionSyntax right = assignment.Right;

            BinaryExpressionSyntax newNode = CoalesceExpression(
                expression.WithoutTrailingTrivia(),
                right.WithTrailingTrivia(expression.GetTrailingTrivia()));

            StatementSyntax newStatement = statement.ReplaceNode(expression, newNode);

            IEnumerable<SyntaxTrivia> trivia = container.Node.DescendantTrivia(TextSpan.FromBounds(statement.Span.End, ifStatement.Span.End));

            if (!trivia.All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                newStatement = newStatement.WithTrailingTrivia(trivia);
                newStatement = newStatement.AppendToTrailingTrivia(ifStatement.GetTrailingTrivia());
            }
            else
            {
                newStatement = newStatement.WithTrailingTrivia(ifStatement.GetTrailingTrivia());
            }

            SyntaxList<StatementSyntax> newStatements = container.Statements
                .Remove(ifStatement)
                .ReplaceAt(ifStatementIndex, newStatement);

            return await document.ReplaceNodeAsync(container.Node, container.NodeWithStatements(newStatements), cancellationToken).ConfigureAwait(false);
        }

        private static StatementSyntax GetSingleStatementOrDefault(StatementSyntax statement)
        {
            if (statement != null)
            {
                if (statement.IsKind(SyntaxKind.Block))
                {
                    var block = (BlockSyntax)statement;

                    SyntaxList<StatementSyntax> statements = block.Statements;

                    if (statements.Count == 1)
                        return statements[0];
                }
                else
                {
                    return statement;
                }
            }

            return null;
        }
    }
}
