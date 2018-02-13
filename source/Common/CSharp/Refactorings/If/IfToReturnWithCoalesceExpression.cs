// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.If
{
    internal class IfToReturnWithCoalesceExpression : IfRefactoring
    {
        public IfToReturnWithCoalesceExpression(
            IfStatementSyntax ifStatement,
            ExpressionSyntax left,
            ExpressionSyntax right,
            bool isYield) : base(ifStatement)
        {
            Left = left;
            Right = right;
            IsYield = isYield;
        }

        public ExpressionSyntax Left { get; }

        public ExpressionSyntax Right { get; }

        public bool IsYield { get; }

        public override RefactoringKind Kind
        {
            get
            {
                if (IsYield)
                    return RefactoringKind.IfElseToYieldReturnWithCoalesceExpression;

                return (IfStatement.IsSimpleIf())
                    ? RefactoringKind.IfReturnToReturnWithCoalesceExpression
                    : RefactoringKind.IfElseToReturnWithCoalesceExpression;
            }
        }

        public override string Title
        {
            get { return "Use coalesce expression"; }
        }

        protected StatementSyntax CreateStatement(ExpressionSyntax expression)
        {
            if (IsYield)
            {
                return CSharpFactory.YieldReturnStatement(expression);
            }
            else
            {
                return SyntaxFactory.ReturnStatement(expression);
            }
        }

        public override async Task<Document> RefactorAsync(Document document, CancellationToken cancellationToken = default(CancellationToken))
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            int position = IfStatement.SpanStart;

            ITypeSymbol targetType = GetTargetType(position, semanticModel, cancellationToken);

            BinaryExpressionSyntax coalesceExpression = RefactoringHelper.CreateCoalesceExpression(
                targetType,
                Left.WithoutTrivia(),
                Right.WithoutTrivia(),
                position,
                semanticModel);

            StatementSyntax statement = CreateStatement(coalesceExpression);

            if (IfStatement.IsSimpleIf())
            {
                    StatementsInfo statementsInfo = SyntaxInfo.StatementsInfo(IfStatement);

                    SyntaxList<StatementSyntax> statements = statementsInfo.Statements;

                    int index = statements.IndexOf(IfStatement);

                    StatementSyntax newNode = statement
                        .WithLeadingTrivia(IfStatement.GetLeadingTrivia())
                        .WithTrailingTrivia(statements[index + 1].GetTrailingTrivia())
                        .WithFormatterAnnotation();

                    SyntaxList<StatementSyntax> newStatements = statements
                        .RemoveAt(index)
                        .ReplaceAt(index, newNode);

                    return await document.ReplaceStatementsAsync(statementsInfo, newStatements, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                StatementSyntax newNode = statement
                    .WithTriviaFrom(IfStatement)
                    .WithFormatterAnnotation();

                return await document.ReplaceNodeAsync(IfStatement, newNode, cancellationToken).ConfigureAwait(false);
            }
        }

        private ITypeSymbol GetTargetType(int position, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            IMethodSymbol methodSymbol = semanticModel.GetEnclosingSymbol<IMethodSymbol>(position, cancellationToken);

            Debug.Assert(methodSymbol != null, "");

            if (methodSymbol?.IsErrorType() == false)
            {
                ITypeSymbol returnType = methodSymbol.ReturnType;

                if (!returnType.IsErrorType())
                {
                    if (methodSymbol.IsAsync)
                    {
                        if (returnType is INamedTypeSymbol namedTypeSymbol
                            && namedTypeSymbol.ConstructedFrom.EqualsOrInheritsFrom(semanticModel.GetTypeByMetadataName(MetadataNames.System_Threading_Tasks_Task_T)))
                        {
                            return namedTypeSymbol.TypeArguments[0];
                        }
                    }
                    else if (!IsYield)
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