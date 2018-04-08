// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class InlineLazyInitializationRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            StatementListInfo statementsInfo = SyntaxInfo.StatementListInfo(ifStatement);

            SyntaxList<StatementSyntax> statements = statementsInfo.Statements;

            int index = statements.IndexOf(ifStatement);

            StatementSyntax expressionStatement = (ExpressionStatementSyntax)statements[index + 1];

            SimpleMemberInvocationStatementInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationStatementInfo((ExpressionStatementSyntax)expressionStatement);

            ExpressionSyntax expression = invocationInfo.Expression;

            SimpleAssignmentStatementInfo assignmentInfo = SyntaxInfo.SimpleAssignmentStatementInfo((ExpressionStatementSyntax)ifStatement.SingleNonBlockStatementOrDefault());

            BinaryExpressionSyntax coalesceExpression = CSharpFactory.CoalesceExpression(expression.WithoutTrivia(), ParenthesizedExpression(assignmentInfo.AssignmentExpression));

            ParenthesizedExpressionSyntax newExpression = ParenthesizedExpression(coalesceExpression)
                .WithTriviaFrom(expression);

            StatementSyntax newExpressionStatement = expressionStatement.ReplaceNode(expression, newExpression);

            IEnumerable<SyntaxTrivia> trivia = statementsInfo.Parent.DescendantTrivia(TextSpan.FromBounds(ifStatement.FullSpan.Start, expressionStatement.FullSpan.Start));

            if (trivia.Any(f => !f.IsWhitespaceOrEndOfLineTrivia()))
                newExpressionStatement = newExpressionStatement.PrependToLeadingTrivia(trivia);

            SyntaxList<StatementSyntax> newStatements = statements
                .Replace(expressionStatement, newExpressionStatement)
                .RemoveAt(index);

            return document.ReplaceStatementsAsync(statementsInfo, newStatements, cancellationToken);
        }
    }
}
