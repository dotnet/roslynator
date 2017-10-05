// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceIfElseWithIfReturnRefactoring
    {
        public static void ComputeRefactoring(RefactoringContext context, IfStatementSyntax ifStatement)
        {
            if (!ifStatement.IsTopmostIf())
                return;

            IfStatementSyntax topmostIf = ifStatement;

            while (true)
            {
                ElseClauseSyntax elseClause = ifStatement.Else;

                if (elseClause == null)
                    return;

                if (!IsLastStatementJumpStatement(ifStatement.Statement))
                    return;

                StatementSyntax elseStatement = elseClause.Statement;

                if (elseStatement == null)
                    return;

                if (elseStatement.Kind() != SyntaxKind.IfStatement)
                {
                    if (!IsLastStatementJumpStatement(elseStatement))
                        return;

                    context.RegisterRefactoring(
                        "Replace if-else with if-return",
                        cancellationToken => RefactorAsync(context.Document, topmostIf, cancellationToken));

                    return;
                }

                ifStatement = (IfStatementSyntax)elseStatement;
            }
        }

        private static bool IsLastStatementJumpStatement(StatementSyntax statement)
        {
            if (statement == null)
                return false;

            SyntaxKind kind = statement.Kind();

            if (kind == SyntaxKind.Block)
            {
                statement = ((BlockSyntax)statement).Statements.LastOrDefault();

                if (statement == null)
                    return false;

                kind = statement.Kind();
            }

            return kind.Is(
                SyntaxKind.ReturnStatement,
                SyntaxKind.ContinueStatement,
                SyntaxKind.BreakStatement,
                SyntaxKind.ThrowStatement);
        }

        private static Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            CancellationToken cancellationToken)
        {
            var statements = new List<StatementSyntax>();

            IfStatementSyntax topIfStatement = ifStatement;

            ElseClauseSyntax elseClause = null;

            while (true)
            {
                IfStatementSyntax newIfStatement = ifStatement.WithElse(null);

                if (elseClause != null)
                    newIfStatement = newIfStatement.PrependToLeadingTrivia(elseClause.GetLeadingTrivia());

                newIfStatement = newIfStatement
                    .AppendToTrailingTrivia(CSharpFactory.NewLine())
                    .WithFormatterAnnotation();

                statements.Add(newIfStatement);

                elseClause = ifStatement.Else;

                StatementSyntax statement = elseClause.Statement;

                SyntaxKind kind = statement.Kind();

                if (kind != SyntaxKind.IfStatement)
                {
                    if (kind == SyntaxKind.Block)
                    {
                        statements.AddRange(((BlockSyntax)statement).Statements.Select(f => f.WithFormatterAnnotation()));
                    }
                    else
                    {
                        statements.Add(statement);
                    }

                    break;
                }

                ifStatement = ((IfStatementSyntax)statement);
            }

            return document.ReplaceNodeAsync(topIfStatement, statements, cancellationToken);
        }
    }
}
