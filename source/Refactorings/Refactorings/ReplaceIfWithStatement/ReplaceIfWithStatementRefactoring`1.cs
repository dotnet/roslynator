// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings.ReplaceIfWithStatement
{
    internal abstract class ReplaceIfWithStatementRefactoring<TStatement> where TStatement : StatementSyntax
    {
        public abstract SyntaxKind StatementKind { get; }

        public abstract string StatementTitle { get; }

        public abstract TStatement CreateStatement(ExpressionSyntax expression);

        public abstract ExpressionSyntax GetExpression(TStatement statement);

        public async Task ComputeRefactoringAsync(RefactoringContext context, IfStatementSyntax ifStatement)
        {
            ElseClauseSyntax elseClause = ifStatement.Else;

            if (elseClause != null)
            {
                TStatement statement1 = GetStatement(ifStatement);

                if (statement1 != null)
                {
                    TStatement statement2 = GetStatement(elseClause);

                    if (statement2 != null)
                    {
                        SyntaxKind kind1 = statement1.Kind();
                        SyntaxKind kind2 = statement2.Kind();

                        if (statement1.IsKind(StatementKind)
                            && statement2.IsKind(StatementKind))
                        {
                            ExpressionSyntax expression1 = GetExpression(statement1);

                            if (expression1?.IsMissing == false)
                            {
                                ExpressionSyntax expression2 = GetExpression(statement2);

                                if (expression2?.IsMissing == false)
                                {
                                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                    if (semanticModel.GetTypeSymbol(expression1, context.CancellationToken)?.IsBoolean() == true
                                        && semanticModel.GetTypeSymbol(expression2, context.CancellationToken)?.IsBoolean() == true)
                                    {
                                        context.RegisterRefactoring(
                                            $"Replace if-else with {StatementTitle}",
                                            cancellationToken => RefactorAsync(context.Document, ifStatement, expression1, expression2, cancellationToken));
                                    }

                                    if (!expression1.IsBooleanLiteralExpression()
                                        || !expression2.IsBooleanLiteralExpression())
                                    {
                                        context.RegisterRefactoring(
                                            $"Replace if-else with {StatementTitle} ?:",
                                            cancellationToken => RefactorWithConditionalExpressionAsync(context.Document, ifStatement, expression1, expression2, cancellationToken));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private TStatement GetStatement(IfStatementSyntax ifStatement)
        {
            StatementSyntax statement = ReplaceIfWithStatementRefactoring.GetStatement(ifStatement);

            if (statement?.IsKind(StatementKind) == true)
                return (TStatement)statement;

            return null;
        }

        private TStatement GetStatement(ElseClauseSyntax elseClause)
        {
            StatementSyntax statement = ReplaceIfWithStatementRefactoring.GetStatement(elseClause);

            if (statement?.IsKind(StatementKind) == true)
                return (TStatement)statement;

            return null;
        }

        private async Task<Document> RefactorAsync(
            Document document,
            IfStatementSyntax ifStatement,
            ExpressionSyntax expression1,
            ExpressionSyntax expression2,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax expression = ReplaceIfWithStatementRefactoring.GetExpression(ifStatement.Condition, expression1, expression2);

            TStatement newNode = CreateStatement(expression)
                .WithTriviaFrom(ifStatement)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(ifStatement, newNode, cancellationToken).ConfigureAwait(false);
        }

        private async Task<Document> RefactorWithConditionalExpressionAsync(
            Document document,
            IfStatementSyntax ifStatement,
            ExpressionSyntax whenTrue,
            ExpressionSyntax whenFalse,
            CancellationToken cancellationToken)
        {
            ConditionalExpressionSyntax conditionalExpression = ReplaceIfWithStatementRefactoring.CreateConditionalExpression(ifStatement.Condition, whenTrue, whenFalse);

            TStatement newNode = CreateStatement(conditionalExpression);

            newNode = newNode
                .WithTriviaFrom(ifStatement)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(ifStatement, newNode, cancellationToken).ConfigureAwait(false);
        }
    }
}
