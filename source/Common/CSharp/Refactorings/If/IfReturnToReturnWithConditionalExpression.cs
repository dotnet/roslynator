// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings.If
{
    internal class IfReturnToReturnWithConditionalExpression : IfToReturnWithConditionalExpression
    {
        public IfReturnToReturnWithConditionalExpression(
            IfStatementSyntax ifStatement,
            ExpressionSyntax expression1,
            ExpressionSyntax expression2) : base(ifStatement, expression1, expression2)
        {
        }

        public override RefactoringKind Kind
        {
            get { return RefactoringKind.IfReturnToReturnWithConditionalExpression; }
        }

        public override string Title
        {
            get { return "Use conditional expression"; }
        }

        protected override StatementSyntax CreateStatement(ExpressionSyntax expression)
        {
            return ReturnStatement(expression);
        }

        public override Task<Document> RefactorAsync(Document document, CancellationToken cancellationToken = default(CancellationToken))
        {
            StatementContainer container = StatementContainer.Create(IfStatement);

            SyntaxList<StatementSyntax> statements = container.Statements;

            int index = statements.IndexOf(IfStatement);

            ConditionalExpressionSyntax conditionalExpression = IfRefactoringHelper.CreateConditionalExpression(IfStatement.Condition, Expression1, Expression2);

            StatementSyntax newStatement = CreateStatement(conditionalExpression)
                .WithLeadingTrivia(IfStatement.GetLeadingTrivia())
                .WithTrailingTrivia(statements[index + 1].GetTrailingTrivia())
                .WithFormatterAnnotation();

            SyntaxList<StatementSyntax> newStatements = statements
                .RemoveAt(index)
                .ReplaceAt(index, newStatement);

            return document.ReplaceNodeAsync(container.Node, container.NodeWithStatements(newStatements), cancellationToken);
        }
    }
}