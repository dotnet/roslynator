// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

            if (condition == null)
                return false;

            var namedTypeSymbol = semanticModel
                .GetTypeInfo(condition, cancellationToken)
                .ConvertedType as INamedTypeSymbol;

            return namedTypeSymbol?.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T
                && namedTypeSymbol.TypeArguments[0].SpecialType == SpecialType.System_Boolean;
        }

        public static void Refactor(
            StatementSyntax statement,
            CodeRefactoringContext context,
            SemanticModel semanticModel)
        {
            if (CanRefactor(statement, semanticModel, context.CancellationToken))
            {
                context.RegisterRefactoring(
                    "Add boolean comparison",
                    cancellationToken => RefactorAsync(context.Document, statement, cancellationToken));
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            StatementSyntax newNode = SetCondition(statement);

            return document.WithSyntaxRoot(oldRoot.ReplaceNode(statement, newNode));
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

        private static StatementSyntax SetCondition(StatementSyntax statement)
        {
            switch (statement.Kind())
            {
                case SyntaxKind.IfStatement:
                    {
                        var ifStatement = (IfStatementSyntax)statement;
                        return ifStatement.WithCondition(CreateNewCondition(ifStatement.Condition));
                    }
                case SyntaxKind.WhileStatement:
                    {
                        var whileStatement = (WhileStatementSyntax)statement;
                        return whileStatement.WithCondition(CreateNewCondition(whileStatement.Condition));
                    }
                case SyntaxKind.DoStatement:
                    {
                        var doStatement = (DoStatementSyntax)statement;
                        return doStatement.WithCondition(CreateNewCondition(doStatement.Condition));
                    }
            }

            Debug.Assert(false, statement.Kind().ToString());
            return statement;
        }

        private static BinaryExpressionSyntax CreateNewCondition(ExpressionSyntax expression)
        {
            return SyntaxFactory.BinaryExpression(
                SyntaxKind.EqualsExpression,
                expression,
                SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression));
        }
    }
}
