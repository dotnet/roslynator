// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.ExtractCondition
{
    internal class ExtractConditionFromWhileToNestedIfRefactoring
        : ExtractConditionRefactoring<WhileStatementSyntax>
    {
        private ExtractConditionFromWhileToNestedIfRefactoring()
        {
        }

        public static ExtractConditionFromWhileToNestedIfRefactoring Instance { get; } = new ExtractConditionFromWhileToNestedIfRefactoring();

        public override SyntaxKind StatementKind
        {
            get { return SyntaxKind.WhileStatement; }
        }

        public override string Title
        {
            get { return "Extract condition to nested if"; }
        }

        public override StatementSyntax GetStatement(WhileStatementSyntax statement)
        {
            return statement.Statement;
        }

        public override WhileStatementSyntax SetStatement(WhileStatementSyntax statement, StatementSyntax newStatement)
        {
            return statement.WithStatement(newStatement);
        }

        public Task<Document> RefactorAsync(
            Document document,
            WhileStatementSyntax whileStatement,
            BinaryExpressionSyntax condition,
            ExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            WhileStatementSyntax newNode = RemoveExpressionFromCondition(whileStatement, condition, expression);

            newNode = AddNestedIf(newNode, expression)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(whileStatement, newNode, cancellationToken);
        }

        public Task<Document> RefactorAsync(
            Document document,
            WhileStatementSyntax whileStatement,
            BinaryExpressionSyntax condition,
            BinaryExpressionSelection binaryExpressionSelection,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            WhileStatementSyntax newNode = RemoveExpressionsFromCondition(whileStatement, condition, binaryExpressionSelection);

            newNode = AddNestedIf(newNode, binaryExpressionSelection)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(whileStatement, newNode, cancellationToken);
        }
    }
}
