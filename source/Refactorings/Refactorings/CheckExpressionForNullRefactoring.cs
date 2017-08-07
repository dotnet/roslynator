// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static Roslynator.CSharp.CSharpFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CheckExpressionForNullRefactoring
    {
        public static async Task ComputeRefactoringAsync(RefactoringContext context, ExpressionSyntax expression)
        {
            SyntaxNode parent = expression.Parent;

            if (parent != null)
            {
                var assignment = parent as AssignmentExpressionSyntax;

                if (assignment?.Left == expression)
                {
                    ExpressionSyntax right = assignment.Right;

                    if (right?.IsKind(SyntaxKind.NullLiteralExpression) == false
                        && CanBeEqualToNull(right))
                    {
                        parent = parent.Parent;

                        if (parent?.IsKind(SyntaxKind.ExpressionStatement) == true)
                        {
                            var statement = (ExpressionStatementSyntax)parent;

                            if (!NullCheckExists(expression, statement))
                            {
                                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                if (semanticModel
                                    .GetTypeSymbol(expression, context.CancellationToken)?
                                    .IsReferenceType == true)
                                {
                                    RegisterRefactoring(context, expression, statement);
                                }
                            }
                        }
                    }
                }
            }
        }

        internal static async Task ComputeRefactoringAsync(RefactoringContext context, VariableDeclarationSyntax variableDeclaration)
        {
            SyntaxNode parent = variableDeclaration.Parent;

            if (parent?.IsKind(SyntaxKind.LocalDeclarationStatement) == true)
            {
                TypeSyntax type = variableDeclaration.Type;

                if (type != null)
                {
                    VariableDeclaratorSyntax variableDeclarator = variableDeclaration.SingleVariableOrDefault();

                    ExpressionSyntax value = variableDeclarator?.Initializer?.Value;

                    if (value?.IsKind(SyntaxKind.NullLiteralExpression) == false
                        && CanBeEqualToNull(value))
                    {
                        SyntaxToken identifier = variableDeclarator.Identifier;

                        if (context.Span.IsContainedInSpanOrBetweenSpans(identifier))
                        {
                            IdentifierNameSyntax identifierName = IdentifierName(identifier);

                            var localDeclaration = (StatementSyntax)parent;

                            if (!NullCheckExists(identifierName, localDeclaration))
                            {
                                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                if (semanticModel
                                    .GetTypeSymbol(type, context.CancellationToken)?
                                    .IsReferenceType == true)
                                {
                                    RegisterRefactoring(context, identifierName, localDeclaration);
                                }
                            }
                        }
                    }
                }
            }
        }

        internal static async Task ComputeRefactoringAsync(RefactoringContext context, StatementContainerSelection selectedStatements)
        {
            if (selectedStatements.Count > 1)
            {
                StatementSyntax statement = selectedStatements.First();

                SyntaxKind kind = statement.Kind();

                if (kind == SyntaxKind.LocalDeclarationStatement)
                {
                    await ComputeRefactoringAsync(context, (LocalDeclarationStatementSyntax)statement, selectedStatements).ConfigureAwait(false);
                }
                else if (kind == SyntaxKind.ExpressionStatement)
                {
                    await ComputeRefactoringAsync(context, (ExpressionStatementSyntax)statement, selectedStatements).ConfigureAwait(false);
                }
            }
        }

        private static async Task ComputeRefactoringAsync(
            RefactoringContext context,
            LocalDeclarationStatementSyntax localDeclaration,
            StatementContainerSelection selectedStatements)
        {
            VariableDeclarationSyntax variableDeclaration = localDeclaration.Declaration;

            if (variableDeclaration != null)
            {
                TypeSyntax type = variableDeclaration.Type;

                if (type != null)
                {
                    VariableDeclaratorSyntax variableDeclarator = variableDeclaration.SingleVariableOrDefault();

                    ExpressionSyntax value = variableDeclarator?.Initializer?.Value;

                    if (value?.IsKind(SyntaxKind.NullLiteralExpression) == false
                        && CanBeEqualToNull(value))
                    {
                        SyntaxToken identifier = variableDeclarator.Identifier;

                        IdentifierNameSyntax identifierName = IdentifierName(identifier);

                        if (!NullCheckExists(identifierName, localDeclaration))
                        {
                            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            if (semanticModel
                                .GetTypeSymbol(type, context.CancellationToken)?
                                .IsReferenceType == true)
                            {
                                RegisterRefactoring(context, identifierName, localDeclaration, selectedStatements.Count - 1);
                            }
                        }
                    }
                }
            }
        }

        private static async Task ComputeRefactoringAsync(
            RefactoringContext context,
            ExpressionStatementSyntax expressionStatement,
            StatementContainerSelection selectedStatements)
        {
            ExpressionSyntax expression = expressionStatement.Expression;

            if (expression?.IsKind(SyntaxKind.SimpleAssignmentExpression) == true)
            {
                var assignment = (AssignmentExpressionSyntax)expression;
                ExpressionSyntax left = assignment.Left;

                if (left != null)
                {
                    ExpressionSyntax right = assignment.Right;

                    if (right?.IsKind(SyntaxKind.NullLiteralExpression) == false
                        && CanBeEqualToNull(right)
                        && !NullCheckExists(left, expressionStatement))
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        if (semanticModel
                            .GetTypeSymbol(left, context.CancellationToken)?
                            .IsReferenceType == true)
                        {
                            RegisterRefactoring(context, left, expressionStatement, selectedStatements.Count - 1);
                        }
                    }
                }
            }
        }

        private static bool CanBeEqualToNull(ExpressionSyntax expression)
        {
            switch (expression?.Kind())
            {
                case SyntaxKind.AnonymousObjectCreationExpression:
                case SyntaxKind.ArrayCreationExpression:
                case SyntaxKind.ImplicitArrayCreationExpression:
                case SyntaxKind.ObjectCreationExpression:
                case SyntaxKind.ThisExpression:
                case SyntaxKind.CharacterLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                case SyntaxKind.NumericLiteralExpression:
                case SyntaxKind.StringLiteralExpression:
                case SyntaxKind.TrueLiteralExpression:
                    return false;
                default:
                    return true;
            }
        }

        private static void RegisterRefactoring(RefactoringContext context, ExpressionSyntax expression, StatementSyntax statement)
        {
            context.RegisterRefactoring(
                GetTitle(expression),
                cancellationToken => RefactorAsync(context.Document, expression, statement, cancellationToken));
        }

        private static void RegisterRefactoring(RefactoringContext context, ExpressionSyntax expression, StatementSyntax statement, int statementCount)
        {
            context.RegisterRefactoring(
                GetTitle(expression),
                cancellationToken => RefactorAsync(context.Document, expression, statement, statementCount, cancellationToken));
        }

        private static string GetTitle(ExpressionSyntax expression)
        {
            return $"Check '{expression}' for null";
        }

        private static bool NullCheckExists(ExpressionSyntax expression, StatementSyntax statement)
        {
            if (!EmbeddedStatementHelper.IsEmbeddedStatement(statement))
            {
                StatementContainer container;
                if (StatementContainer.TryCreate(statement, out container))
                {
                    SyntaxList<StatementSyntax> statements = container.Statements;

                    int index = statements.IndexOf(statement);

                    if (index < statements.Count - 1)
                    {
                        StatementSyntax nextStatement = statements[index + 1];

                        if (nextStatement.IsKind(SyntaxKind.IfStatement))
                        {
                            var ifStatement = (IfStatementSyntax)nextStatement;

                            ExpressionSyntax condition = ifStatement.Condition;

                            if (condition?.IsKind(SyntaxKind.NotEqualsExpression) == true)
                            {
                                var notEqualsExpression = (BinaryExpressionSyntax)condition;

                                ExpressionSyntax left = notEqualsExpression.Left;

                                if (left?.IsEquivalentTo(expression, topLevel: false) == true)
                                {
                                    ExpressionSyntax right = notEqualsExpression.Right;

                                    if (right?.IsKind(SyntaxKind.NullLiteralExpression) == true)
                                        return true;
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            StatementSyntax statement,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            if (EmbeddedStatementHelper.IsEmbeddedStatement(statement))
            {
                return await document.ReplaceNodeAsync(statement, Block(statement, CreateNullCheck(expression)), cancellationToken).ConfigureAwait(false);
            }
            else
            {
                StatementContainer container;
                if (StatementContainer.TryCreate(statement, out container))
                {
                    SyntaxList<StatementSyntax> statements = container.Statements;

                    int statementIndex = statements.IndexOf(statement);

                    ISymbol symbol = (statement.IsKind(SyntaxKind.LocalDeclarationStatement))
                        ? semanticModel.GetDeclaredSymbol(((LocalDeclarationStatementSyntax)statement).Declaration.Variables.First(), cancellationToken)
                        : semanticModel.GetSymbol(expression, cancellationToken);

                    int lastStatementIndex = IncludeAllReferencesOfSymbol(symbol, expression.Kind(), statements, statementIndex + 1, semanticModel, cancellationToken);

                    if (lastStatementIndex != -1)
                    {
                        if (lastStatementIndex < statements.Count - 1)
                            lastStatementIndex = IncludeAllReferencesOfVariablesDeclared(statements, statementIndex + 1, lastStatementIndex, semanticModel, cancellationToken);

                        return await RefactorAsync(
                            document,
                            expression,
                            statements,
                            container,
                            statementIndex,
                            lastStatementIndex,
                            cancellationToken).ConfigureAwait(false);
                    }
                }
            }

            return await document.InsertNodeAfterAsync(statement, CreateNullCheck(expression), cancellationToken).ConfigureAwait(false);
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            StatementSyntax statement,
            int statementCount,
            CancellationToken cancellationToken)
        {
            StatementContainer container;
            if (StatementContainer.TryCreate(statement, out container))
            {
                SyntaxList<StatementSyntax> statements = container.Statements;

                int statementIndex = statements.IndexOf(statement);

                return await RefactorAsync(
                    document,
                    expression,
                    statements,
                    container,
                    statementIndex,
                    statementIndex + statementCount,
                    cancellationToken).ConfigureAwait(false);
            }

            return document;
        }

        private static Task<Document> RefactorAsync(
            Document document,
            ExpressionSyntax expression,
            SyntaxList<StatementSyntax> statements,
            StatementContainer container,
            int statementIndex,
            int lastStatementIndex,
            CancellationToken cancellationToken)
        {
            IEnumerable<StatementSyntax> blockStatements = statements
                .Skip(statementIndex + 1)
                .Take(lastStatementIndex - statementIndex);

            IfStatementSyntax ifStatement = CreateNullCheck(expression, List(blockStatements));

            if (lastStatementIndex < statements.Count - 1)
                ifStatement = ifStatement.AppendToTrailingTrivia(NewLine());

            IEnumerable<StatementSyntax> newStatements = statements.Take(statementIndex + 1)
                .Concat(new IfStatementSyntax[] { ifStatement })
                .Concat(statements.Skip(lastStatementIndex + 1));

            return document.ReplaceNodeAsync(container.Node, container.NodeWithStatements(newStatements), cancellationToken);
        }

        private static IfStatementSyntax CreateNullCheck(ExpressionSyntax expression, SyntaxList<StatementSyntax> statements = default(SyntaxList<StatementSyntax>))
        {
            SyntaxToken openBrace = (statements.Any())
                ? OpenBraceToken()
                : Token(default(SyntaxTriviaList), SyntaxKind.OpenBraceToken, TriviaList(NewLine()));

            SyntaxToken closeBrace = (statements.Any())
                ? CloseBraceToken()
                : Token(TriviaList(NewLine()), SyntaxKind.CloseBraceToken, default(SyntaxTriviaList));

            IfStatementSyntax ifStatement = IfStatement(
                NotEqualsExpression(expression.WithoutTrivia(), NullLiteralExpression()),
                Block(openBrace, statements, closeBrace));

            return ifStatement.WithFormatterAnnotation();
        }

        private static int IncludeAllReferencesOfSymbol(
            ISymbol symbol,
            SyntaxKind kind,
            SyntaxList<StatementSyntax> statements,
            int lastStatementIndex,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            for (int i = statements.Count - 1; i >= lastStatementIndex; i--)
            {
                foreach (SyntaxNode node in statements[i].DescendantNodes())
                {
                    if (node.IsKind(kind))
                    {
                        ISymbol symbol2 = semanticModel.GetSymbol(node, cancellationToken);

                        if (symbol.Equals(symbol2))
                            return i;
                    }
                }
            }

            return -1;
        }

        private static int IncludeAllReferencesOfVariablesDeclared(
            SyntaxList<StatementSyntax> statements,
            int statementIndex,
            int lastStatementIndex,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            for (int i = statementIndex; i <= lastStatementIndex; i++)
            {
                if (statements[i].IsKind(SyntaxKind.LocalDeclarationStatement))
                {
                    var localDeclaration = (LocalDeclarationStatementSyntax)statements[i];

                    VariableDeclarationSyntax declaration = localDeclaration.Declaration;

                    if (declaration != null)
                    {
                        foreach (VariableDeclaratorSyntax variable in declaration.Variables)
                        {
                            ISymbol symbol = semanticModel.GetDeclaredSymbol(variable, cancellationToken);

                            if (symbol != null)
                            {
                                int index = IncludeAllReferencesOfSymbol(symbol, SyntaxKind.IdentifierName, statements, i + 1, semanticModel, cancellationToken);

                                if (index > lastStatementIndex)
                                    lastStatementIndex = index;
                            }
                        }
                    }
                }
            }

            return lastStatementIndex;
        }
    }
}