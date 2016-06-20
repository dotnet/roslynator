// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class ExpandInitializerRefactoring
    {
        private const string Title = "Expand object initializer";

        public static void Register(RefactoringContext context, InitializerExpressionSyntax initializer)
        {
            if (initializer == null)
                throw new ArgumentNullException(nameof(initializer));

            if (initializer.IsAnyKind(SyntaxKind.ObjectInitializerExpression, SyntaxKind.CollectionInitializerExpression)
                && initializer.Expressions.Count > 0
                && initializer.Parent?.IsKind(SyntaxKind.ObjectCreationExpression) == true)
            {
                switch (initializer.Parent.Parent?.Kind())
                {
                    case SyntaxKind.SimpleAssignmentExpression:
                        {
                            var assignmentExpression = (AssignmentExpressionSyntax)initializer.Parent.Parent;

                            if (assignmentExpression.Left != null
                                && assignmentExpression.Parent?.IsKind(SyntaxKind.ExpressionStatement) == true
                                && assignmentExpression.Parent.Parent?.IsKind(SyntaxKind.Block) == true)
                            {
                                context.RegisterRefactoring(
                                    Title,
                                    cancellationToken => ExpandObjectInitializerAsync(
                                        context.Document,
                                        initializer,
                                        (ExpressionStatementSyntax)assignmentExpression.Parent,
                                        assignmentExpression.Left.WithoutTrivia(),
                                        cancellationToken));
                            }

                            break;
                        }
                    case SyntaxKind.EqualsValueClause:
                        {
                            var equalsValueClause = (EqualsValueClauseSyntax)initializer.Parent.Parent;

                            if (equalsValueClause.Parent?.IsKind(SyntaxKind.VariableDeclarator) == true
                                && equalsValueClause.Parent.Parent?.IsKind(SyntaxKind.VariableDeclaration) == true
                                && equalsValueClause.Parent.Parent.Parent?.IsKind(SyntaxKind.LocalDeclarationStatement) == true
                                && equalsValueClause.Parent.Parent.Parent.Parent?.IsKind(SyntaxKind.Block) == true)
                            {
                                context.RegisterRefactoring(
                                    Title,
                                    cancellationToken => ExpandObjectInitializerAsync(
                                        context.Document,
                                        initializer,
                                        (LocalDeclarationStatementSyntax)equalsValueClause.Parent.Parent.Parent,
                                        IdentifierName(((VariableDeclaratorSyntax)equalsValueClause.Parent).Identifier.ToString()),
                                        cancellationToken));
                            }

                            break;
                        }
                }
            }
        }

        private static async Task<Document> ExpandObjectInitializerAsync(
            Document document,
            InitializerExpressionSyntax initializer,
            StatementSyntax statement,
            ExpressionSyntax expression,
            CancellationToken cancellationToken)
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken);

            ExpressionStatementSyntax[] expressions = ExpandObjectInitializer(initializer, expression).ToArray();

            expressions[expressions.Length - 1] = expressions[expressions.Length - 1]
                .WithTrailingTrivia(statement.GetTrailingTrivia());

            var block = (BlockSyntax)statement.Parent;

            int index = block.Statements.IndexOf(statement);

            StatementSyntax newStatement = statement.RemoveNode(initializer, SyntaxRemoveOptions.KeepNoTrivia);

            if (statement.IsKind(SyntaxKind.ExpressionStatement))
            {
                var expressionStatement = (ExpressionStatementSyntax)newStatement;

                newStatement = expressionStatement
                    .WithExpression(expressionStatement.Expression.WithoutTrailingTrivia());
            }
            else if (statement.IsKind(SyntaxKind.LocalDeclarationStatement))
            {
                var localDeclaration = (LocalDeclarationStatementSyntax)newStatement;

                newStatement = localDeclaration
                    .WithDeclaration(localDeclaration.Declaration.WithoutTrailingTrivia());
            }

            SyntaxList<StatementSyntax> newStatements = block.Statements.Replace(statement, newStatement);

            BlockSyntax newBlock = block
                .WithStatements(newStatements.InsertRange(index + 1, expressions))
                .WithAdditionalAnnotations(Formatter.Annotation);

            SyntaxNode newRoot = oldRoot.ReplaceNode(block, newBlock);

            return document.WithSyntaxRoot(newRoot);
        }

        private static IEnumerable<ExpressionStatementSyntax> ExpandObjectInitializer(
            InitializerExpressionSyntax initializer,
            ExpressionSyntax initializedExpression)
        {
            foreach (ExpressionSyntax expression in initializer.Expressions)
            {
                if (expression.IsKind(SyntaxKind.SimpleAssignmentExpression))
                {
                    var assignment = (AssignmentExpressionSyntax)expression;

                    if (assignment.Left.IsKind(SyntaxKind.ImplicitElementAccess))
                    {
                        yield return ExpressionStatement(
                            AssignmentExpression(
                                SyntaxKind.SimpleAssignmentExpression,
                                ElementAccessExpression(
                                    initializedExpression,
                                    ((ImplicitElementAccessSyntax)assignment.Left).ArgumentList),
                                assignment.Right));
                    }
                    else
                    {
                        yield return ExpressionStatement(
                            AssignmentExpression(
                                SyntaxKind.SimpleAssignmentExpression,
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    initializedExpression,
                                    (IdentifierNameSyntax)assignment.Left),
                                assignment.Right));
                    }
                }
                else if (expression.IsKind(SyntaxKind.ComplexElementInitializerExpression))
                {
                    var elementInitializer = (InitializerExpressionSyntax)expression;

                    yield return ExpressionStatement(
                        AssignmentExpression(
                            SyntaxKind.SimpleAssignmentExpression,
                            ElementAccessExpression(
                                initializedExpression,
                                BracketedArgumentList(SingletonSeparatedList(Argument(elementInitializer.Expressions[0])))),
                            elementInitializer.Expressions[1]));
                }
            }
        }
    }
}
