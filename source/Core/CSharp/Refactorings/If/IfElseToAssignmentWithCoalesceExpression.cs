// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings.If
{
    internal class IfElseToAssignmentWithCoalesceExpression : IfRefactoring
    {
        public IfElseToAssignmentWithCoalesceExpression(
            IfStatementSyntax ifStatement,
            ExpressionSyntax condition,
            ExpressionSyntax left,
            ExpressionSyntax right1,
            ExpressionSyntax right2) : base(ifStatement)
        {
            Condition = condition;
            Left = left;
            Right1 = right1;
            Right2 = right2;
        }

        public override RefactoringKind Kind
        {
            get { return RefactoringKind.IfElseToAssignmentWithCoalesceExpression; }
        }

        public override string Title
        {
            get { return "Use coalesce expression"; }
        }

        public ExpressionSyntax Condition { get; }

        public ExpressionSyntax Left { get; }

        public ExpressionSyntax Right1 { get; }

        public ExpressionSyntax Right2 { get; }

        public override async Task<Document> RefactorAsync(Document document, CancellationToken cancellationToken = default(CancellationToken))
        {
            ExpressionSyntax left = Right1.WithoutTrivia();
            ExpressionSyntax right = Right2.WithoutTrivia();

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            right = AddCastExpressionIfNecessary(right, semanticModel, IfStatement.SpanStart, cancellationToken);

            BinaryExpressionSyntax coalesceExpression = CoalesceExpression(
                left.Parenthesize().WithSimplifierAnnotation(),
                right.Parenthesize().WithSimplifierAnnotation());

            ExpressionStatementSyntax newNode = SimpleAssignmentStatement(Left.WithoutTrivia(), coalesceExpression)
                .WithTriviaFrom(IfStatement)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(IfStatement, newNode, cancellationToken).ConfigureAwait(false);
        }

        private ExpressionSyntax AddCastExpressionIfNecessary(ExpressionSyntax expression, SemanticModel semanticModel, int position, CancellationToken cancellationToken)
        {
            ITypeSymbol right1Symbol = semanticModel.GetTypeSymbol(Right1, cancellationToken);

            if (right1Symbol?.IsErrorType() == false)
            {
                ITypeSymbol right2Symbol = semanticModel.GetTypeSymbol(Right2, cancellationToken);

                if (right2Symbol?.IsErrorType() == false
                    && !right1Symbol.Equals(right2Symbol))
                {
                    ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(Left, cancellationToken);

                    if (typeSymbol?.IsErrorType() == false)
                    {
                        return SyntaxFactory.CastExpression(
                            typeSymbol.ToMinimalTypeSyntax(semanticModel, position),
                            expression);
                    }
                }
            }

            return expression;
        }
    }
}