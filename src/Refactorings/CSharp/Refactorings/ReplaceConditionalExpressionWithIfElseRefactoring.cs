// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
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

        private const string RecursiveTitle = Title + " (recursively)";

        internal const string EquivalenceKey = RefactoringIdentifiers.ReplaceConditionalExpressionWithIfElse;

        internal static readonly string RecursiveEquivalenceKey = Roslynator.EquivalenceKey.Join(RefactoringIdentifiers.ReplaceConditionalExpressionWithIfElse, "Recursive");

        public static async Task ComputeRefactoringAsync(RefactoringContext context, ConditionalExpressionSyntax conditionalExpression)
        {
            ExpressionSyntax expression = conditionalExpression.WalkUpParentheses();

            SyntaxNode parent = expression.Parent;

            if (parent.IsKind(SyntaxKind.ReturnStatement, SyntaxKind.YieldReturnStatement))
            {
                var statement = (StatementSyntax)parent;

                RegisterRefactoring(context, conditionalExpression, statement);

                if (IsRecursive())
                    RegisterRefactoring(context, conditionalExpression, statement, recursive: true);
            }
            else if (parent is AssignmentExpressionSyntax assignment)
            {
                if (assignment.Parent is ExpressionStatementSyntax expressionStatement)
                {
                    RegisterRefactoring(context, conditionalExpression, expressionStatement);

                    if (IsRecursive())
                        RegisterRefactoring(context, conditionalExpression, expressionStatement, recursive: true);
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
                        LocalDeclarationStatementSyntax statement = localDeclarationInfo.Statement;

                        RegisterRefactoring(context, conditionalExpression, statement, semanticModel);

                        if (IsRecursive())
                            RegisterRefactoring(context, conditionalExpression, statement, semanticModel, recursive: true);
                    }
                }
            }

            bool IsRecursive()
            {
                return conditionalExpression
                    .WhenFalse
                    .WalkDownParentheses()
                    .IsKind(SyntaxKind.ConditionalExpression);
            }
        }

        private static void RegisterRefactoring(
            RefactoringContext context,
            ConditionalExpressionSyntax conditionalExpression,
            StatementSyntax statement,
            bool recursive = false)
        {
            Document document = context.Document;

            context.RegisterRefactoring(
                (recursive) ? RecursiveTitle : Title,
                ct => RefactorAsync(document, conditionalExpression, statement, recursive: recursive, cancellationToken: ct),
                (recursive) ? RecursiveEquivalenceKey : EquivalenceKey);
        }

        private static void RegisterRefactoring(
            RefactoringContext context,
            ConditionalExpressionSyntax conditionalExpression,
            LocalDeclarationStatementSyntax localDeclaration,
            SemanticModel semanticModel,
            bool recursive = false)
        {
            Document document = context.Document;

            context.RegisterRefactoring(
                (recursive) ? RecursiveTitle : Title,
                ct => RefactorAsync(document, conditionalExpression, localDeclaration, semanticModel, recursive: recursive, cancellationToken: ct),
                (recursive) ? RecursiveEquivalenceKey : EquivalenceKey);
        }

        private static Task<Document> RefactorAsync(
            Document document,
            ConditionalExpressionSyntax conditionalExpression,
            StatementSyntax statement,
            bool recursive,
            CancellationToken cancellationToken)
        {
            StatementSyntax newStatement = statement.TrimTrivia();

            IfStatementSyntax ifElseStatement = ConvertConditionalExpressionToIfElse(
                conditionalExpression,
                expression => CreateNewStatement(newStatement, expression),
                recursive: recursive);

            ifElseStatement = ifElseStatement
                .WithTriviaFrom(statement)
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(statement, ifElseStatement, cancellationToken);
        }

        private static StatementSyntax CreateNewStatement(StatementSyntax statement, ExpressionSyntax expression)
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
                default:
                    {
                        throw new InvalidOperationException();
                    }
            }
        }

        private static Task<Document> RefactorAsync(
            Document document,
            ConditionalExpressionSyntax conditionalExpression,
            LocalDeclarationStatementSyntax localDeclaration,
            SemanticModel semanticModel,
            bool recursive,
            CancellationToken cancellationToken)
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

            IfStatementSyntax ifElseStatement = ConvertConditionalExpressionToIfElse(conditionalExpression, expression => SimpleAssignmentStatement(left, expression), recursive: recursive);

            StatementListInfo statementsInfo = SyntaxInfo.StatementListInfo(localDeclaration);

            SyntaxList<StatementSyntax> statements = statementsInfo.Statements;

            SyntaxList<StatementSyntax> newStatements = statements
                .Replace(localDeclaration, newLocalDeclaration.WithFormatterAnnotation())
                .Insert(statements.IndexOf(localDeclaration) + 1, ifElseStatement.WithFormatterAnnotation());

            return document.ReplaceStatementsAsync(statementsInfo, newStatements, cancellationToken);
        }

        private static IfStatementSyntax ConvertConditionalExpressionToIfElse(
            ConditionalExpressionSyntax conditionalExpression,
            Func<ExpressionSyntax, StatementSyntax> createStatement,
            bool recursive = false)
        {
            StatementSyntax whenTrue = createStatement(conditionalExpression.WhenTrue.WithoutTrivia());

            ElseClauseSyntax elseClause;

            if (recursive
                && conditionalExpression.WhenFalse.WalkDownParentheses() is ConditionalExpressionSyntax nestedConditionalExpression)
            {
                IfStatementSyntax ifElseStatement = ConvertConditionalExpressionToIfElse(nestedConditionalExpression, createStatement, recursive: true);

                elseClause = ElseClause(ifElseStatement);
            }
            else
            {
                StatementSyntax whenFalse = createStatement(conditionalExpression.WhenFalse.WithoutTrivia());

                elseClause = ElseClause(Block(whenFalse));
            }

            return IfStatement(
                conditionalExpression.Condition.WalkDownParentheses().WithoutTrivia(),
                Block(whenTrue),
                elseClause);
        }
    }
}
