// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.If
{
    internal abstract class IfToReturnWithConditionalExpression : IfRefactoring
    {
        protected IfToReturnWithConditionalExpression(
            IfStatementSyntax ifStatement,
            ExpressionSyntax expression1,
            ExpressionSyntax expression2) : base(ifStatement)
        {
            Expression1 = expression1;
            Expression2 = expression2;
        }

        public ExpressionSyntax Expression1 { get; }

        public ExpressionSyntax Expression2 { get; }

        protected abstract StatementSyntax CreateStatement(ExpressionSyntax expression);

        public static IfToReturnWithConditionalExpression Create(IfStatementSyntax ifStatement, ExpressionSyntax expression1, ExpressionSyntax expression2, bool isYield)
        {
            if (isYield)
            {
                return new IfElseToYieldReturnWithConditionalExpression(ifStatement, expression1, expression2);
            }
            else if (ifStatement.IsSimpleIf())
            {
                return new IfReturnToReturnWithConditionalExpression(ifStatement, expression1, expression2);
            }
            else
            {
                return new IfElseToReturnWithConditionalExpression(ifStatement, expression1, expression2);
            }
        }

        public override Task<Document> RefactorAsync(Document document, CancellationToken cancellationToken = default(CancellationToken))
        {
            ConditionalExpressionSyntax conditionalExpression = IfRefactoringHelper.CreateConditionalExpression(IfStatement.Condition, Expression1, Expression2);

            StatementSyntax newNode = CreateStatement(conditionalExpression)
                .WithTriviaFrom(IfStatement)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(IfStatement, newNode, cancellationToken);
        }
    }
}