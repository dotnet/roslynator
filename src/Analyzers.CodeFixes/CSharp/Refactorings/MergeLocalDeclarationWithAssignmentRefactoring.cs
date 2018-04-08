// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MergeLocalDeclarationWithAssignmentRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            VariableDeclaratorSyntax declarator,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var declaration = (VariableDeclarationSyntax)declarator.Parent;

            var localDeclaration = (LocalDeclarationStatementSyntax)declaration.Parent;

            StatementListInfo statementsInfo = SyntaxInfo.StatementListInfo(localDeclaration);

            SyntaxList<StatementSyntax> statements = statementsInfo.Statements;

            int index = statements.IndexOf(localDeclaration);

            StatementSyntax nextStatement = statements[index + 1];

            var expressionStatement = (ExpressionStatementSyntax)nextStatement;

            var assignment = (AssignmentExpressionSyntax)expressionStatement.Expression;

            ExpressionSyntax right = assignment.Right;

            EqualsValueClauseSyntax initializer = declarator.Initializer;

            ExpressionSyntax value = initializer?.Value;

            VariableDeclaratorSyntax newDeclarator = (value != null)
                ? declarator.ReplaceNode(value, right)
                : declarator.WithInitializer(EqualsValueClause(right));

            LocalDeclarationStatementSyntax newLocalDeclaration = localDeclaration.ReplaceNode(declarator, newDeclarator);

            SyntaxTriviaList trailingTrivia = localDeclaration.GetTrailingTrivia();

            if (!trailingTrivia.IsEmptyOrWhitespace())
            {
                newLocalDeclaration = newLocalDeclaration.WithTrailingTrivia(trailingTrivia.Concat(nextStatement.GetTrailingTrivia()));
            }
            else
            {
                newLocalDeclaration = newLocalDeclaration.WithTrailingTrivia(nextStatement.GetTrailingTrivia());
            }

            SyntaxTriviaList leadingTrivia = nextStatement.GetLeadingTrivia();

            if (!leadingTrivia.IsEmptyOrWhitespace())
                newLocalDeclaration = newLocalDeclaration.WithLeadingTrivia(newLocalDeclaration.GetLeadingTrivia().Concat(leadingTrivia));

            SyntaxList<StatementSyntax> newStatements = statements
                .Replace(localDeclaration, newLocalDeclaration)
                .RemoveAt(index + 1);

            return document.ReplaceStatementsAsync(statementsInfo, newStatements, cancellationToken);
        }
    }
}