// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.If
{
    internal abstract class IfToReturnWithBooleanExpression : IfRefactoring
    {
        protected IfToReturnWithBooleanExpression(
            IfStatementSyntax ifStatement,
            ExpressionSyntax expression1,
            ExpressionSyntax expression2) : base(ifStatement)
        {
            Expression1 = expression1;
            Expression2 = expression2;
        }

        public ExpressionSyntax Expression1 { get; }

        public ExpressionSyntax Expression2 { get; }

        public abstract StatementSyntax CreateStatement(ExpressionSyntax expression);

        public static IfToReturnWithBooleanExpression Create(IfStatementSyntax ifStatement, ExpressionSyntax expression1, ExpressionSyntax expression2, bool isYield)
        {
            if (isYield)
            {
                return new IfElseToYieldReturnWithBooleanExpression(ifStatement, expression1, expression2);
            }
            else if (ifStatement.IsSimpleIf())
            {
                return new IfReturnToReturnWithBooleanExpression(ifStatement, expression1, expression2);
            }
            else
            {
                return new IfElseToReturnWithBooleanExpression(ifStatement, expression1, expression2);
            }
        }

        public override async Task<Document> RefactorAsync(Document document, CancellationToken cancellationToken = default(CancellationToken))
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ExpressionSyntax expression = IfRefactoringHelper.GetBooleanExpression(IfStatement.Condition, Expression1, Expression2, semanticModel, cancellationToken);

            StatementSyntax newNode = CreateStatement(expression)
                .WithTriviaFrom(IfStatement)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(IfStatement, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}