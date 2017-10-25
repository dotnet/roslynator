// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ExtractDeclarationFromUsingStatementRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            UsingStatementSyntax usingStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            StatementsInfo statementsInfo = SyntaxInfo.StatementsInfo(usingStatement);

            int index = statementsInfo.Statements.IndexOf(usingStatement);

            StatementsInfo newStatementsInfo = statementsInfo.RemoveAt(index);

            var statements = new List<StatementSyntax>() { SyntaxFactory.LocalDeclarationStatement(usingStatement.Declaration) };

            statements.AddRange(GetStatements(usingStatement));

            if (statements.Count > 0)
            {
                statements[0] = statements[0]
                    .WithLeadingTrivia(usingStatement.GetLeadingTrivia());

                statements[statements.Count - 1] = statements[statements.Count - 1]
                    .WithTrailingTrivia(usingStatement.GetTrailingTrivia());
            }

            newStatementsInfo = newStatementsInfo.WithStatements(newStatementsInfo.Statements.InsertRange(index, statements));

            return document.ReplaceNodeAsync(statementsInfo.Node, newStatementsInfo.Node.WithFormatterAnnotation(), cancellationToken);
        }

        private static IEnumerable<StatementSyntax> GetStatements(UsingStatementSyntax usingStatement)
        {
            StatementSyntax statement = usingStatement.Statement;

            if (statement != null)
            {
                if (statement.IsKind(SyntaxKind.Block))
                {
                    foreach (StatementSyntax statement2 in ((BlockSyntax)statement).Statements)
                        yield return statement2;
                }
                else
                {
                    yield return statement;
                }
            }
        }
    }
}
