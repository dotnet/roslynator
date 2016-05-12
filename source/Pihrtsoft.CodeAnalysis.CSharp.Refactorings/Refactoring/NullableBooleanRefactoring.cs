// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class NullableBooleanRefactoring
    {
        private static bool CanRefactor(
            StatementSyntax statement,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ExpressionSyntax condition = GetCondition(statement);

            if (condition != null)
                return CanRefactor(condition, semanticModel, cancellationToken);

            return false;
        }

        private static ExpressionSyntax GetCondition(StatementSyntax statement)
        {
            switch (statement.Kind())
            {
                case SyntaxKind.IfStatement:
                    return ((IfStatementSyntax)statement).Condition;
                case SyntaxKind.WhileStatement:
                    return ((WhileStatementSyntax)statement).Condition;
                case SyntaxKind.DoStatement:
                    return ((DoStatementSyntax)statement).Condition;
            }

            Debug.Assert(false, statement.Kind().ToString());

            return null;
        }

        private static bool CanRefactor(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (expression.IsKind(SyntaxKind.LogicalNotExpression))
            {
                var logicalNot = (PrefixUnaryExpressionSyntax)expression;

                if (logicalNot.Operand != null)
                    return IsNullableBoolean(logicalNot.Operand, semanticModel, cancellationToken);
            }
            else
            {
                return IsNullableBoolean(expression, semanticModel, cancellationToken);
            }

            return false;
        }

        private static bool IsNullableBoolean(
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            var namedTypeSymbol = semanticModel
                .GetTypeInfo(expression, cancellationToken)
                .ConvertedType as INamedTypeSymbol;

            return namedTypeSymbol?.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T
                && namedTypeSymbol.TypeArguments[0].SpecialType == SpecialType.System_Boolean;
        }

        public static void Refactor(
            StatementSyntax statement,
            CodeRefactoringContext context,
            SemanticModel semanticModel)
        {
            if (statement == null)
                throw new System.ArgumentNullException(nameof(statement));

            if (semanticModel == null)
                throw new System.ArgumentNullException(nameof(semanticModel));

            if (CanRefactor(statement, semanticModel, context.CancellationToken))
            {
                context.RegisterRefactoring(
                    "Add boolean comparison",
                    cancellationToken => RefactorAsync(context.Document, statement, cancellationToken));
            }
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            StatementSyntax newNode = SetCondition(statement);

            return document.WithSyntaxRoot(oldRoot.ReplaceNode(statement, newNode));
        }

        public static void Refactor(
            ExpressionSyntax expression,
            CodeRefactoringContext context,
            SemanticModel semanticModel)
        {
            if (expression == null)
                throw new System.ArgumentNullException(nameof(expression));

            if (semanticModel == null)
                throw new System.ArgumentNullException(nameof(semanticModel));

            if (CanRefactor(expression, semanticModel, context.CancellationToken))
            {
                context.RegisterRefactoring(
                    "Add boolean comparison",
                    cancellationToken => RefactorAsync(context.Document, expression, cancellationToken));
            }
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            SyntaxNode newNode = CreateNewExpression(expression)
                .WithTriviaFrom(expression)
                .WithAdditionalAnnotations(Formatter.Annotation);

            return document.WithSyntaxRoot(oldRoot.ReplaceNode(expression, newNode));
        }

        private static StatementSyntax SetCondition(StatementSyntax statement)
        {
            switch (statement.Kind())
            {
                case SyntaxKind.IfStatement:
                    {
                        var ifStatement = (IfStatementSyntax)statement;
                        return ifStatement.WithCondition(CreateNewExpression(ifStatement.Condition));
                    }
                case SyntaxKind.WhileStatement:
                    {
                        var whileStatement = (WhileStatementSyntax)statement;
                        return whileStatement.WithCondition(CreateNewExpression(whileStatement.Condition));
                    }
                case SyntaxKind.DoStatement:
                    {
                        var doStatement = (DoStatementSyntax)statement;
                        return doStatement.WithCondition(CreateNewExpression(doStatement.Condition));
                    }
            }

            Debug.Assert(false, statement.Kind().ToString());

            return statement;
        }

        private static BinaryExpressionSyntax CreateNewExpression(ExpressionSyntax expression)
        {
            if (expression.IsKind(SyntaxKind.LogicalNotExpression))
            {
                var logicalNot = (PrefixUnaryExpressionSyntax)expression;

                return SyntaxFactory.BinaryExpression(
                    SyntaxKind.EqualsExpression,
                    logicalNot.Operand.WithoutTrivia(),
                    SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression));
            }
            else
            {
                return SyntaxFactory.BinaryExpression(
                    SyntaxKind.EqualsExpression,
                    expression.WithoutTrivia(),
                    SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression));
            }
        }
    }
}
