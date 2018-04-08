// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceConditionalExpressionWithIfElseRefactoring
    {
        private const string Title = "Replace ?: with if-else";

        public static async Task ComputeRefactoringAsync(RefactoringContext context, ConditionalExpressionSyntax conditionalExpression)
        {
            ExpressionSyntax expression = conditionalExpression.WalkUpParentheses();

            SyntaxNode parent = expression.Parent;

            if (parent.IsKind(SyntaxKind.ReturnStatement, SyntaxKind.YieldReturnStatement))
            {
                context.RegisterRefactoring(
                    Title,
                    cancellationToken => RefactorAsync(context.Document, (StatementSyntax)parent, conditionalExpression, cancellationToken));
            }
            else if (parent is AssignmentExpressionSyntax assignment)
            {
                if (assignment.Parent is ExpressionStatementSyntax expressionStatement)
                {
                    context.RegisterRefactoring(
                        Title,
                        cancellationToken => RefactorAsync(context.Document, expressionStatement, conditionalExpression, cancellationToken));
                }
            }
            else
            {
                SingleLocalDeclarationStatementInfo localDeclarationInfo = SyntaxInfo.SingleLocalDeclarationStatementInfo(expression);

                if (localDeclarationInfo.Success)
                {
                    TypeSyntax type = localDeclarationInfo.Type;

                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    if (!type.IsVar
                        || semanticModel.GetTypeSymbol(type, context.CancellationToken)?.SupportsExplicitDeclaration() == true)
                    {
                        context.RegisterRefactoring(
                            Title,
                            cancellationToken => RefactorAsync(context.Document, localDeclarationInfo.Statement, conditionalExpression, semanticModel, cancellationToken));
                    }
                }
            }
        }

        private static Task<Document> RefactorAsync(
            Document document,
            LocalDeclarationStatementSyntax localDeclaration,
            ConditionalExpressionSyntax conditionalExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            VariableDeclaratorSyntax variableDeclarator = localDeclaration.Declaration.Variables[0];

            LocalDeclarationStatementSyntax newLocalDeclaration = localDeclaration.RemoveNode(variableDeclarator.Initializer, SyntaxRemoveOptions.KeepExteriorTrivia);

            TypeSyntax type = newLocalDeclaration.Declaration.Type;

            if (type.IsVar)
            {
                newLocalDeclaration = newLocalDeclaration.ReplaceNode(
                    type,
                    semanticModel.GetTypeSymbol(conditionalExpression)
                        .ToMinimalTypeSyntax(semanticModel, type.SpanStart)
                        .WithTriviaFrom(type));
            }

            IdentifierNameSyntax left = IdentifierName(variableDeclarator.Identifier.ValueText);

            IfStatementSyntax ifStatement = IfElseStatement(
                conditionalExpression.Condition.WalkDownParentheses().WithoutTrivia(),
                SimpleAssignmentStatement(left, conditionalExpression.WhenTrue.WithoutTrivia()),
                SimpleAssignmentStatement(left, conditionalExpression.WhenFalse.WithoutTrivia()));

            StatementListInfo statementsInfo = SyntaxInfo.StatementListInfo(localDeclaration);

            SyntaxList<StatementSyntax> statements = statementsInfo.Statements;

            SyntaxList<StatementSyntax> newStatements = statements
                .Replace(localDeclaration, newLocalDeclaration.WithFormatterAnnotation())
                .Insert(statements.IndexOf(localDeclaration) + 1, ifStatement.WithFormatterAnnotation());

            return document.ReplaceStatementsAsync(statementsInfo, newStatements, cancellationToken);
        }

        private static Task<Document> RefactorAsync(
            Document document,
            StatementSyntax statement,
            ConditionalExpressionSyntax conditionalExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            StatementSyntax newStatement = statement.TrimTrivia();

            IfStatementSyntax ifStatement = IfElseStatement(
                conditionalExpression.Condition.WalkDownParentheses().WithoutTrivia(),
                SetExpression(newStatement, conditionalExpression.WhenTrue),
                SetExpression(newStatement, conditionalExpression.WhenFalse));

            ifStatement = ifStatement
                .WithTriviaFrom(statement)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(statement, ifStatement, cancellationToken);
        }

        private static StatementSyntax SetExpression(StatementSyntax statement, ExpressionSyntax expression)
        {
            switch (statement.Kind())
            {
                case SyntaxKind.ReturnStatement:
                    {
                        return ((ReturnStatementSyntax)statement).WithExpression(expression);
                    }
                case SyntaxKind.YieldReturnStatement:
                    {
                        return ((YieldStatementSyntax)statement).WithExpression(expression);
                    }
                case SyntaxKind.ExpressionStatement:
                    {
                        var expressionStatement = (ExpressionStatementSyntax)statement;

                        var assignment = (AssignmentExpressionSyntax)expressionStatement.Expression;

                        return expressionStatement.WithExpression(assignment.WithRight(expression));
                    }
            }

            Debug.Fail(statement.Kind().ToString());

            return statement;
        }

        private static IfStatementSyntax IfElseStatement(
            ExpressionSyntax condition,
            StatementSyntax whenTrueStatement,
            StatementSyntax whenFalseStatement)
        {
            return IfStatement(
                condition,
                Block(whenTrueStatement),
                ElseClause(Block(whenFalseStatement)));
        }
    }
}
