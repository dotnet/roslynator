// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.ExtractCondition
{
    internal class ExtractConditionFromIfToNestedIfRefactoring
        : ExtractConditionFromIfRefactoring
    {
        public override string Title
        {
            get { return "Extract condition to nested if"; }
        }

        public async Task<Document> RefactorAsync(
            Document document,
            BinaryExpressionSyntax condition,
            ExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var ifStatement = (IfStatementSyntax)condition.Parent;

            IfStatementSyntax newIfStatement = RemoveExpressionFromCondition(ifStatement, condition, expression)
                .WithFormatterAnnotation();

            IfStatementSyntax newNode = AddNestedIf(newIfStatement, expression);

            return await document.ReplaceNodeAsync(ifStatement, newNode, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            BinaryExpressionSyntax condition,
            SelectedExpressions selectedExpressions,
            CancellationToken cancellationToken)
        {
            IfStatementSyntax newNode = RemoveExpressionsFromCondition(ifStatement, condition, selectedExpressions)
                .WithFormatterAnnotation();

            ExpressionSyntax expression = SyntaxFactory.ParseExpression(selectedExpressions.ExpressionsText);

            newNode = AddNestedIf(newNode, expression);

            return await document.ReplaceNodeAsync(ifStatement, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}
