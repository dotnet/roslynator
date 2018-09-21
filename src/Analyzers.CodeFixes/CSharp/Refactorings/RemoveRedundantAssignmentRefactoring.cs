// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveRedundantAssignmentRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            AssignmentExpressionSyntax assignmentExpression,
            CancellationToken cancellationToken)
        {
            var statement = (StatementSyntax)assignmentExpression.Parent;

            StatementListInfo statementsInfo = SyntaxInfo.StatementListInfo(statement);

            SyntaxList<StatementSyntax> statements = statementsInfo.Statements;

            int index = statements.IndexOf(statement);

            statements = statements.RemoveAt(index);

            var returnStatement = (ReturnStatementSyntax)statement.NextStatement();

            SyntaxTriviaList trivia = statementsInfo
                .Parent
                .DescendantTrivia(TextSpan.FromBounds(statement.SpanStart, returnStatement.SpanStart))
                .ToSyntaxTriviaList()
                .EmptyIfWhitespace();

            trivia = statement
                .GetLeadingTrivia()
                .AddRange(trivia);

            returnStatement = returnStatement
                .WithExpression(assignmentExpression.Right.WithTriviaFrom(returnStatement.Expression))
                .WithLeadingTrivia(trivia)
                .WithFormatterAnnotation();

            statements = statements.ReplaceAt(index, returnStatement);

            return document.ReplaceStatementsAsync(statementsInfo, statements, cancellationToken);
        }
    }
}
