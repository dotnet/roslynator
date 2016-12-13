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
            StatementSyntax statement = GetContainingStatement(expression);

            if (statement != null
                && !NullCheckExists(expression, statement))
            {
                SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(expression, context.CancellationToken);

                if (typeSymbol?.IsReferenceType == true)
                    RegisterRefactoring(context, expression.WithoutTrivia(), statement);
            }
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, VariableDeclaratorSyntax variableDeclarator)
        {
            SyntaxToken identifier = variableDeclarator.Identifier;

            StatementSyntax statement = GetContainingStatement(variableDeclarator);

            if (statement != null)
            {
                IdentifierNameSyntax identifierName = IdentifierName(identifier.WithoutTrivia());

                if (!NullCheckExists(identifierName, statement))
                {
                    var variableDeclaration = (VariableDeclarationSyntax)variableDeclarator.Parent;

                    TypeSyntax type = variableDeclaration.Type;

                    if (type != null)
                    {
                        SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(type, context.CancellationToken).Type;

                        if (typeSymbol?.IsReferenceType == true)
                            RegisterRefactoring(context, identifierName, statement);
                    }
                }
            }
        }

        private static void RegisterRefactoring(RefactoringContext context, ExpressionSyntax expression, StatementSyntax statement)
        {
            context.RegisterRefactoring(
                $"Check '{expression}' for null",
                cancellationToken => RefactorAsync(context.Document, expression, statement, cancellationToken));
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

        private static StatementSyntax GetContainingStatement(ExpressionSyntax expression)
        {
            SyntaxNode parent = expression.Parent;

            if (parent != null)
            {
                var assignment = parent as AssignmentExpressionSyntax;

                if (assignment?.Left?.Equals(expression) == true)
                {
                    parent = parent.Parent;

                    if (parent?.IsKind(SyntaxKind.ExpressionStatement) == true)
                        return (StatementSyntax)parent;
                }
            }

            return null;
        }

        private static StatementSyntax GetContainingStatement(VariableDeclaratorSyntax variableDeclarator)
        {
            SyntaxNode parent = variableDeclarator.Parent;

            if (parent?.IsKind(SyntaxKind.VariableDeclaration) == true)
            {
                parent = parent.Parent;

                if (parent?.IsKind(SyntaxKind.LocalDeclarationStatement) == true)
                    return (StatementSyntax)parent;
            }

            return null;
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
                    List<StatementSyntax>(),
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