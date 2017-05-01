// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            BinaryExpressionSyntax coalesceExpression = CreateCoalesceExpression(semanticModel, cancellationToken);

            StatementSyntax newNode = CreateStatement(coalesceExpression)
                .WithTriviaFrom(IfStatement)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(IfStatement, newNode, cancellationToken).ConfigureAwait(false);
        }

        protected BinaryExpressionSyntax CreateCoalesceExpression(SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            int position = IfStatement.SpanStart;

            return RefactoringHelper.CreateCoalesceExpression(
                GetTargetType(position, semanticModel, cancellationToken),
                Left.WithoutTrivia(),
                Right.WithoutTrivia(),
                position,
                semanticModel);
        }

        protected ITypeSymbol GetTargetType(int position, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            IMethodSymbol methodSymbol = semanticModel.GetEnclosingSymbol<IMethodSymbol>(position, cancellationToken);

            Debug.Assert(methodSymbol != null, "");

            if (methodSymbol?.IsErrorType() == false)
            {
                ITypeSymbol returnType = methodSymbol.ReturnType;

                if (!returnType.IsErrorType())
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
                }
            }

            return null;
        }
    }
}