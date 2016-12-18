// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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

                if (assignment?.Left == expression
                    && !RulesOutNullCheck(assignment.Right))
                {
                    parent = parent.Parent;

                    if (parent?.IsKind(SyntaxKind.ExpressionStatement) == true)
                    {
                        var statement = (ExpressionStatementSyntax)parent;

                        if (statement != null
                            && !NullCheckExists(expression, statement))
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

        internal static async Task ComputeRefactoringAsync(RefactoringContext context, VariableDeclarationSyntax variableDeclaration)
        {
            SyntaxNode parent = variableDeclaration.Parent;

            if (parent?.IsKind(SyntaxKind.LocalDeclarationStatement) == true)
            {
                TypeSyntax type = variableDeclaration.Type;

                if (type != null)
                {
                    SeparatedSyntaxList<VariableDeclaratorSyntax> variables = variableDeclaration.Variables;

                    if (variables.Count == 1)
                    {
                        VariableDeclaratorSyntax variableDeclarator = variables[0];

                        if (!RulesOutNullCheck(variableDeclarator?.Initializer?.Value))
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
        }

        private static bool RulesOutNullCheck(ExpressionSyntax expression)
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
                case SyntaxKind.NullLiteralExpression:
                case SyntaxKind.NumericLiteralExpression:
                case SyntaxKind.StringLiteralExpression:
                case SyntaxKind.TrueLiteralExpression:
                    return true;
                default:
                    return false;
            }
        }

        private static void RegisterRefactoring(RefactoringContext context, ExpressionSyntax expression, StatementSyntax statement)
        {
            context.RegisterRefactoring(
                $"Check '{expression}' for null",
                cancellationToken => RefactorAsync(context.Document, expression.WithoutTrivia(), statement, cancellationToken));
        }

        private static bool NullCheckExists(ExpressionSyntax expression, StatementSyntax statement)
        {
            if (!EmbeddedStatement.IsEmbeddedStatement(statement))
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
            IfStatementSyntax ifStatement = IfStatement(
                NotEqualsExpression(expression, NullLiteralExpression()),
                Block(
                    Token(TriviaList(), SyntaxKind.OpenBraceToken, TriviaList(NewLineTrivia())),
                    default(SyntaxList<StatementSyntax>),
                    Token(TriviaList(NewLineTrivia()), SyntaxKind.CloseBraceToken, TriviaList())));

            ifStatement = ifStatement
                .WithLeadingTrivia(NewLineTrivia())
                .WithFormatterAnnotation();

            if (EmbeddedStatement.IsEmbeddedStatement(statement))
            {
                return await document.ReplaceNodeAsync(statement, Block(statement, ifStatement), cancellationToken).ConfigureAwait(false);
            }
            else
            {
                return await document.InsertNodeAfterAsync(statement, ifStatement, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}