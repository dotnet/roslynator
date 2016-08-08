// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class MergeAssignmentExpressionWithReturnStatementRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            ExpressionStatementSyntax statement,
            ReturnStatementSyntax returnStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var block = (BlockSyntax)statement.Parent;

            SyntaxList<StatementSyntax> statements = block.Statements;

            int index = statements.IndexOf(statement);

            SyntaxList<StatementSyntax> newStatements = statements.RemoveAt(index);

            var assignmentExpression = (AssignmentExpressionSyntax)statement.Expression;

            ReturnStatementSyntax newReturnStatement = returnStatement
                .WithExpression(assignmentExpression.Right)
                .WithLeadingTrivia(assignmentExpression.GetLeadingTrivia())
                .WithTrailingTrivia(returnStatement.GetTrailingTrivia())
                .WithFormatterAnnotation();

            newStatements = newStatements.Replace(newStatements[index], newReturnStatement);

            root = root.ReplaceNode(block, block.WithStatements(newStatements));

            return document.WithSyntaxRoot(root);
        }
    }
}
