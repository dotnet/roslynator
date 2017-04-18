// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MergeAssignmentExpressionWithReturnStatementRefactoring
    {
        public static void ComputeRefactorings(RefactoringContext context, StatementContainerSelection selectedStatements)
        {
            using (IEnumerator<StatementSyntax> en = selectedStatements.GetEnumerator())
            {
                if (en.MoveNext()
                    && en.Current.IsKind(SyntaxKind.ExpressionStatement))
                {
                    var statement = (ExpressionStatementSyntax)en.Current;

                    if (statement.Expression?.IsKind(SyntaxKind.SimpleAssignmentExpression) == true
                        && en.MoveNext()
                        && en.Current.IsKind(SyntaxKind.ReturnStatement))
                    {
                        var returnStatement = (ReturnStatementSyntax)en.Current;

                        if (returnStatement.Expression != null
                            && !en.MoveNext())
                        {
                            var assignment = (AssignmentExpressionSyntax)statement.Expression;

                            if (assignment.Left?.IsMissing == false
                                && assignment.Right?.IsMissing == false
                                && assignment.Left.IsEquivalentTo(returnStatement.Expression, topLevel: false))
                            {
                                context.RegisterRefactoring(
                                    "Merge statements",
                                    cancellationToken =>
                                    {
                                        return RefactorAsync(
                                            context.Document,
                                            statement,
                                            returnStatement,
                                            cancellationToken);
                                    });
                            }
                        }
                    }
                }
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            ExpressionStatementSyntax statement,
            ReturnStatementSyntax returnStatement,
            CancellationToken cancellationToken = default(CancellationToken))
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
