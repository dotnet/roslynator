// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveUnnecessaryAssignmentRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, StatementListSelection selectedStatements)
        {
            if (selectedStatements.Count != 2)
                return;

            SimpleAssignmentStatementInfo simpleAssignment = SyntaxInfo.SimpleAssignmentStatementInfo(selectedStatements.First());

            if (!simpleAssignment.Success)
                return;

            if (selectedStatements.Last() is not ReturnStatementSyntax returnStatement)
                return;

            if (returnStatement.Expression == null)
                return;

            if (!CSharpFactory.AreEquivalent(simpleAssignment.Left, returnStatement.Expression))
                return;

            context.RegisterRefactoring(
                "Remove unnecessary assignment",
                ct => RefactorAsync(context.Document, simpleAssignment.Statement, returnStatement, ct),
                RefactoringDescriptors.RemoveUnnecessaryAssignment);
        }

        public static Task<Document> RefactorAsync(
            Document document,
            ExpressionStatementSyntax statement,
            ReturnStatementSyntax returnStatement,
            CancellationToken cancellationToken = default)
        {
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

            return document.ReplaceNodeAsync(block, block.WithStatements(newStatements), cancellationToken);
        }
    }
}
