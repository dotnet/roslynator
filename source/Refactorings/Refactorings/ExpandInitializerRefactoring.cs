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
    internal static class ExpandInitializerRefactoring
    {
        private const string Title = "Expand initializer";

        public static async Task ComputeRefactoringsAsync(RefactoringContext context, InitializerExpressionSyntax initializer)
        {
            if (initializer.IsKind(SyntaxKind.ObjectInitializerExpression, SyntaxKind.CollectionInitializerExpression)
                && initializer.Expressions.Any()
                && initializer.Parent?.IsKind(SyntaxKind.ObjectCreationExpression) == true
                && await CanExpandAsync(context, initializer).ConfigureAwait(false))
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
                                    cancellationToken => RefactorAsync(
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
                                    cancellationToken => RefactorAsync(
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

        private static async Task<bool> CanExpandAsync(RefactoringContext context, InitializerExpressionSyntax initializer)
        {
            var objectCreationExpression = (ObjectCreationExpressionSyntax)initializer.Parent;

            if (objectCreationExpression.Type != null)
            {
                ExpressionSyntax expression = initializer.Expressions[0];

                if (expression.IsKind(SyntaxKind.SimpleAssignmentExpression))
                {
                    var assignment = (AssignmentExpressionSyntax)expression;

                    if (assignment.Left.IsKind(SyntaxKind.ImplicitElementAccess))
                    {
                        var implicitElementAccess = (ImplicitElementAccessSyntax)assignment.Left;

                        if (implicitElementAccess.ArgumentList?.Arguments.Count > 0
                            && context.SupportsSemanticModel)
                        {
                            return await HasPublicWritableIndexerAsync(
                                context,
                                implicitElementAccess.ArgumentList.Arguments[0].Expression,
                                objectCreationExpression).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                else if (expression.IsKind(SyntaxKind.ComplexElementInitializerExpression))
                {
                    var initializerExpression = (InitializerExpressionSyntax)expression;

                    if (initializerExpression.Expressions.Count > 0
                        && context.SupportsSemanticModel)
                    {
                        return await HasPublicWritableIndexerAsync(
                            context,
                            initializerExpression.Expressions[0],
                            objectCreationExpression).ConfigureAwait(false);
                    }
                }
                else if (context.SupportsSemanticModel)
                {
                    return await HasPublicAddMethodAsync(
                        context,
                        expression,
                        objectCreationExpression).ConfigureAwait(false);
                }
            }

            return false;
        }

        private static async Task<bool> HasPublicAddMethodAsync(
            RefactoringContext context,
            ExpressionSyntax expression,
            ObjectCreationExpressionSyntax objectCreationExpression)
        {
            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            ISymbol symbol = semanticModel
                .GetSymbolInfo(objectCreationExpression.Type, context.CancellationToken)
                .Symbol;

            if (symbol?.IsNamedType() == true)
            {
                foreach (ISymbol member in ((INamedTypeSymbol)symbol).GetMembers("Add"))
                {
                    if (member.IsMethod()
                        && !member.IsStatic
                        && member.IsPublic())
                    {
                        var methodSymbol = (IMethodSymbol)member;

                        if (methodSymbol.Parameters.Length == 1)
                        {
                            ITypeSymbol expressionSymbol = semanticModel
                                .GetTypeInfo(expression, context.CancellationToken)
                                .ConvertedType;

                            if (expressionSymbol != null
                                && expressionSymbol.Equals(methodSymbol.Parameters[0].Type))
                            {
                                return true;
                            }
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        private static async Task<bool> HasPublicWritableIndexerAsync(
            RefactoringContext context,
            ExpressionSyntax expression,
            ObjectCreationExpressionSyntax objectCreationExpression)
        {
            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            ISymbol symbol = semanticModel
                .GetSymbolInfo(objectCreationExpression.Type, context.CancellationToken)
                .Symbol;

            if (symbol?.IsNamedType() == true)
            {
                foreach (ISymbol member in ((INamedTypeSymbol)symbol).GetMembers("this[]"))
                {
                    if (member.IsProperty()
                        && !member.IsStatic
                        && member.IsPublic())
                    {
                        var propertySymbol = (IPropertySymbol)member;

                        if (!propertySymbol.IsReadOnly
                            && propertySymbol.Parameters.Length == 1)
                        {
                            ITypeSymbol expressionSymbol = semanticModel
                                .GetTypeInfo(expression, context.CancellationToken)
                                .ConvertedType;

                            if (expressionSymbol != null
                                && expressionSymbol.Equals(propertySymbol.Parameters[0].Type))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            InitializerExpressionSyntax initializer,
            StatementSyntax statement,
            ExpressionSyntax expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode oldRoot = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

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
                .WithFormatterAnnotation();

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
                            SimpleAssignmentExpression(
                                ElementAccessExpression(
                                    initializedExpression,
                                    ((ImplicitElementAccessSyntax)assignment.Left).ArgumentList),
                                assignment.Right));
                    }
                    else
                    {
                        yield return ExpressionStatement(
                            SimpleAssignmentExpression(
                                SimpleMemberAccessExpression(
                                    initializedExpression,
                                    (IdentifierNameSyntax)assignment.Left),
                                assignment.Right));
                    }
                }
                else if (expression.IsKind(SyntaxKind.ComplexElementInitializerExpression))
                {
                    var elementInitializer = (InitializerExpressionSyntax)expression;

                    yield return ExpressionStatement(
                        SimpleAssignmentExpression(
                            ElementAccessExpression(
                                initializedExpression,
                                BracketedArgumentList(SingletonSeparatedList(Argument(elementInitializer.Expressions[0])))),
                            elementInitializer.Expressions[1]));
                }
                else
                {
                    yield return ExpressionStatement(
                        InvocationExpression(
                            SimpleMemberAccessExpression(
                                initializedExpression,
                                IdentifierName("Add")),
                            ArgumentList(Argument(expression))
                        )
                    );
                }
            }
        }
    }
}
