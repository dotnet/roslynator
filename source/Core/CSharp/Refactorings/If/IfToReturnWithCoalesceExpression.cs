// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        public abstract bool IsYield { get; }

        protected abstract StatementSyntax CreateStatement(ExpressionSyntax expression);

        public static IfToReturnWithCoalesceExpression Create(IfStatementSyntax ifStatement, ExpressionSyntax expression1, ExpressionSyntax expression2, bool isYield)
        {
            if (isYield)
            {
                return new IfElseToYieldReturnWithCoalesceExpression(ifStatement, expression1, expression2);
            }
            else
            {
                if (ifStatement.IsSimpleIf())
                {
                    return new IfReturnToReturnWithCoalesceExpression(ifStatement, expression1, expression2);
                }
                else
                {
                    return new IfElseToReturnWithCoalesceExpression(ifStatement, expression1, expression2);
                }
            }
        }

        public override async Task<Document> RefactorAsync(Document document, CancellationToken cancellationToken = default(CancellationToken))
        {
            ExpressionSyntax left = Left.WithoutTrivia();
            ExpressionSyntax right = Right.WithoutTrivia();

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            right = AddCastExpressionIfNecessary(right, semanticModel, IfStatement.SpanStart, cancellationToken);

            BinaryExpressionSyntax coalesceExpression = CoalesceExpression(
                left.Parenthesize().WithSimplifierAnnotation(),
                right.Parenthesize().WithSimplifierAnnotation());

            StatementSyntax newNode = CreateStatement(coalesceExpression)
                .WithTriviaFrom(IfStatement)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(IfStatement, newNode, cancellationToken).ConfigureAwait(false);
        }

        protected ExpressionSyntax AddCastExpressionIfNecessary(ExpressionSyntax expression, SemanticModel semanticModel, int position, CancellationToken cancellationToken)
        {
            ITypeSymbol leftSymbol = semanticModel.GetTypeSymbol(Left, cancellationToken);

            if (leftSymbol?.IsErrorType() == false)
            {
                ITypeSymbol rightSymbol = semanticModel.GetTypeSymbol(Right, cancellationToken);

                if (rightSymbol?.IsErrorType() == false
                    && !leftSymbol.Equals(rightSymbol))
                {
                    IMethodSymbol methodSymbol = semanticModel.GetEnclosingSymbol<IMethodSymbol>(IfStatement.SpanStart, cancellationToken);

                    Debug.Assert(methodSymbol != null, "");

                    if (methodSymbol?.IsErrorType() == false)
                    {
                        ITypeSymbol returnType = methodSymbol.ReturnType;

                        if (!returnType.IsErrorType())
                        {
                            ITypeSymbol castType = GetCastType(returnType, semanticModel);

                            if (castType?.IsErrorType() == false)
                            {
                                return SyntaxFactory.CastExpression(
                                    castType.ToMinimalTypeSyntax(semanticModel, position),
                                    expression);
                            }

                        }
                    }
                }
            }

            return expression;
        }

        private ITypeSymbol GetCastType(ITypeSymbol returnType, SemanticModel semanticModel)
        {
            if (!IsYield)
            {
                return returnType;
            }
            else if (returnType.IsIEnumerable())
            {
                return semanticModel.Compilation.ObjectType;
            }
            else if (returnType.IsConstructedFromIEnumerableOfT())
            {
                return ((INamedTypeSymbol)returnType).TypeArguments[0];
            }
            else
            {
                return null;
            }
        }
    }
}