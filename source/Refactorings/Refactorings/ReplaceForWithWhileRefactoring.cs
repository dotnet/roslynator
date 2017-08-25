// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceForWithWhileRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            ForStatementSyntax forStatement,
            CancellationToken cancellationToken)
        {
            var statements = new List<StatementSyntax>();

            VariableDeclarationSyntax declaration = forStatement.Declaration;

            if (declaration != null)
            {
                statements.Add(LocalDeclarationStatement(declaration));
            }
            else
            {
                foreach (ExpressionSyntax initializer in forStatement.Initializers)
                    statements.Add(ExpressionStatement(initializer));
            }

            StatementSyntax statement = forStatement.Statement ?? Block();

            SeparatedSyntaxList<ExpressionSyntax> incrementors = forStatement.Incrementors;

            if (incrementors.Any())
            {
                if (!statement.IsKind(SyntaxKind.Block))
                    statement = Block(statement);

                ExpressionStatementSyntax[] items = incrementors
                    .Select(f => ExpressionStatement(f).WithFormatterAnnotation())
                    .ToArray();

                statement = ((BlockSyntax)statement).AddStatements(items);
            }

            statements.Add(WhileStatement(forStatement.Condition ?? TrueLiteralExpression(), statement));

            statements[0] = statements[0].WithLeadingTrivia(forStatement.GetLeadingTrivia());

            if (forStatement.IsEmbedded())
            {
                return document.ReplaceNodeAsync(forStatement, Block(statements), cancellationToken);
            }
            else
            {
                return document.ReplaceNodeAsync(forStatement, statements, cancellationToken);
            }
        }
    }
}