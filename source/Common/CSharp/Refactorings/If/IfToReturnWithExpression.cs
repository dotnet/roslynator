// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.If
{
    internal abstract class IfToReturnWithExpression : IfRefactoring
    {
        protected IfToReturnWithExpression(
            IfStatementSyntax ifStatement,
            ExpressionSyntax expression) : base(ifStatement)
        {
            Expression = expression;
        }

        public ExpressionSyntax Expression { get; }

        public abstract bool IsYield { get; }

        protected abstract StatementSyntax CreateStatement(ExpressionSyntax expression);

        public static IfToReturnWithExpression Create(IfStatementSyntax ifStatement, ExpressionSyntax expression, bool isYield)
        {
            if (isYield)
            {
                return new IfElseToYieldReturnWithExpression(ifStatement, expression);
            }
            else
            {
                if (ifStatement.IsSimpleIf())
                {
                    return new IfReturnToReturnWithExpression(ifStatement, expression);
                }
                else
                {
                    return new IfElseToReturnWithExpression(ifStatement, expression);
                }
            }
        }

        public override Task<Document> RefactorAsync(Document document, CancellationToken cancellationToken = default(CancellationToken))
        {
            StatementSyntax newNode = CreateStatement(Expression)
                .WithTriviaFrom(IfStatement)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(IfStatement, newNode, cancellationToken);
        }
    }
}