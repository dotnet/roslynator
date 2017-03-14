// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.If
{
    internal abstract class IfToReturnWithCoalesceExpression : IfRefactoring
    {
        protected IfToReturnWithCoalesceExpression(
            IfStatementSyntax ifStatement,
            ExpressionSyntax left,
            ExpressionSyntax right) : base(ifStatement)
        {
            Left = left;
            Right = right;
        }

        public ExpressionSyntax Left { get; }

        public ExpressionSyntax Right { get; }

        protected abstract StatementSyntax CreateStatement(ExpressionSyntax expression);

        public static IfToReturnWithCoalesceExpression Create(IfStatementSyntax ifStatement, ExpressionSyntax expression1, ExpressionSyntax expression2, bool isYield)
        {
            if (isYield)
            {
                return new IfElseToYieldReturnWithCoalesceExpression(ifStatement, expression1, expression2);
            }
            else
            {
                if (IfElseChain.IsPartOfChain(ifStatement))
                {
                    return new IfElseToReturnWithCoalesceExpression(ifStatement, expression1, expression2);
                }
                else
                {
                    return new IfReturnToReturnWithCoalesceExpression(ifStatement, expression1, expression2);
                }
            }
        }

        public override Task<Document> RefactorAsync(Document document, CancellationToken cancellationToken = default(CancellationToken))
        {
            BinaryExpressionSyntax coalesceExpression = CoalesceExpression(
                Left.WithoutTrivia().Parenthesize().WithSimplifierAnnotation(),
                Right.WithoutTrivia().Parenthesize().WithSimplifierAnnotation());

            StatementSyntax newNode = CreateStatement(coalesceExpression)
                .WithTriviaFrom(IfStatement)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(IfStatement, newNode, cancellationToken);
        }
    }
}