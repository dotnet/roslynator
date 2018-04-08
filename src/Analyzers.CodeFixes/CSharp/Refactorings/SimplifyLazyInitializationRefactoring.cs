// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class SimplifyLazyInitializationRefactoring
    {
        public static Task<Document> RefactorAsync(
            Document document,
            BlockSyntax block,
            CancellationToken cancellationToken)
        {
            SyntaxList<StatementSyntax> statements = block.Statements;

            var ifStatement = (IfStatementSyntax)statements[0];

            var returnStatement = (ReturnStatementSyntax)statements[1];

            var expressionStatement = (ExpressionStatementSyntax)ifStatement.SingleNonBlockStatementOrDefault();

            var assignment = (AssignmentExpressionSyntax)expressionStatement.Expression;

            ExpressionSyntax expression = returnStatement.Expression;

            IdentifierNameSyntax valueName = null;

            if (expression.Kind() == SyntaxKind.SimpleMemberAccessExpression)
            {
                var memberAccess = (MemberAccessExpressionSyntax)expression;

                if ((memberAccess.Name is IdentifierNameSyntax identifierName)
                    && string.Equals(identifierName.Identifier.ValueText, "Value", StringComparison.Ordinal))
                {
                    expression = memberAccess.Expression;
                    valueName = identifierName;
                }
            }

            expression = expression.WithoutTrivia();

            ExpressionSyntax right = SimpleAssignmentExpression(expression, assignment.Right.WithoutTrivia()).Parenthesize();

            if (valueName != null)
                right = SimpleMemberAccessExpression(right.Parenthesize(), valueName);

            BinaryExpressionSyntax coalesceExpression = CoalesceExpression(expression, right);

            ReturnStatementSyntax newReturnStatement = returnStatement
                .WithExpression(coalesceExpression)
                .WithLeadingTrivia(ifStatement.GetLeadingTrivia());

            SyntaxList<StatementSyntax> newStatements = statements
                .Replace(returnStatement, newReturnStatement)
                .RemoveAt(0);

            BlockSyntax newBlock = block.WithStatements(newStatements);

            return document.ReplaceNodeAsync(block, newBlock, cancellationToken);
        }
    }
}
