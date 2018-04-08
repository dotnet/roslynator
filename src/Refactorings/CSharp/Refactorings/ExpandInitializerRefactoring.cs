// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
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
    internal static class ExpandInitializerRefactoring
    {
        private const string Title = "Expand initializer";

        public static async Task ComputeRefactoringsAsync(RefactoringContext context, InitializerExpressionSyntax initializer)
        {
            if (!initializer.IsKind(
                SyntaxKind.ObjectInitializerExpression,
                SyntaxKind.CollectionInitializerExpression))
            {
                return;
            }

            if (!initializer.Expressions.Any())
                return;

            SyntaxNode parent = initializer.Parent;

            if (parent?.Kind() != SyntaxKind.ObjectCreationExpression)
                return;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            if (!CanExpand(initializer, semanticModel, context.CancellationToken))
                return;

            if (parent.IsParentKind(SyntaxKind.SimpleAssignmentExpression))
            {
                SimpleAssignmentStatementInfo simpleAssignment = SyntaxInfo.SimpleAssignmentStatementInfo((AssignmentExpressionSyntax)parent.Parent);

                if (simpleAssignment.Success)
                    RegisterRefactoring(context, simpleAssignment.Statement, initializer, simpleAssignment.Left);
            }
            else
            {
                LocalDeclarationStatementInfo localInfo = SyntaxInfo.LocalDeclarationStatementInfo((ExpressionSyntax)parent);

                if (localInfo.Success)
                {
                    var declarator = (VariableDeclaratorSyntax)parent.Parent.Parent;

                    RegisterRefactoring(
                        context,
                        localInfo.Statement,
                        initializer,
                        IdentifierName(declarator.Identifier.ValueText));
                }
            }
        }

        private static void RegisterRefactoring(
            RefactoringContext context,
            StatementSyntax statement,
            InitializerExpressionSyntax initializer,
            ExpressionSyntax expression)
        {
            StatementListInfo statementsInfo = SyntaxInfo.StatementListInfo(statement);

            if (!statementsInfo.Success)
                return;

            context.RegisterRefactoring(
                Title,
                cancellationToken => RefactorAsync(
                    context.Document,
                    statementsInfo,
                    statement,
                    initializer,
                    expression.WithoutTrivia(),
                    cancellationToken));
        }

        private static bool CanExpand(
            InitializerExpressionSyntax initializer,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            var objectCreationExpression = (ObjectCreationExpressionSyntax)initializer.Parent;

            if (objectCreationExpression.Type != null)
            {
                ExpressionSyntax expression = initializer.Expressions[0];

                if (expression.IsKind(SyntaxKind.SimpleAssignmentExpression))
                {
                    var assignment = (AssignmentExpressionSyntax)expression;

                    ExpressionSyntax left = assignment.Left;

                    if (left.IsKind(SyntaxKind.ImplicitElementAccess))
                    {
                        var implicitElementAccess = (ImplicitElementAccessSyntax)left;

                        BracketedArgumentListSyntax argumentList = implicitElementAccess.ArgumentList;

                        if (argumentList?.Arguments.Any() == true)
                        {
                            return HasAccessibleIndexer(
                                argumentList.Arguments[0].Expression,
                                objectCreationExpression,
                                semanticModel,
                                cancellationToken);
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

                    SeparatedSyntaxList<ExpressionSyntax> expressions = initializerExpression.Expressions;

                    if (expressions.Any())
                        return HasAccessibleIndexer(expressions[0], objectCreationExpression, semanticModel, cancellationToken);
                }
                else
                {
                    return HasAccessibleAddMethod(expression, objectCreationExpression, semanticModel, cancellationToken);
                }
            }

            return false;
        }

        private static bool HasAccessibleAddMethod(
            ExpressionSyntax expression,
            ObjectCreationExpressionSyntax objectCreationExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (semanticModel.GetSymbol(objectCreationExpression.Type, cancellationToken) is ITypeSymbol typeSymbol)
            {
                foreach (ISymbol symbol in semanticModel.LookupSymbols(objectCreationExpression.SpanStart, typeSymbol, "Add"))
                {
                    if (!symbol.IsStatic
                        && symbol.Kind == SymbolKind.Method)
                    {
                        var methodSymbol = (IMethodSymbol)symbol;

                        IParameterSymbol parameter = methodSymbol.Parameters.SingleOrDefault(shouldThrow: false);

                        if (parameter != null)
                        {
                            TypeInfo typeInfo = semanticModel.GetTypeInfo(expression, cancellationToken);

                            if (parameter.Type.Equals(typeInfo.ConvertedType))
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        private static bool HasAccessibleIndexer(
            ExpressionSyntax expression,
            ObjectCreationExpressionSyntax objectCreationExpression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (semanticModel.GetSymbol(objectCreationExpression.Type, cancellationToken) is ITypeSymbol typeSymbol)
            {
                int position = objectCreationExpression.SpanStart;

                foreach (ISymbol member in semanticModel.LookupSymbols(position, typeSymbol, "this[]"))
                {
                    var propertySymbol = (IPropertySymbol)member;

                    if (!propertySymbol.IsReadOnly
                        && semanticModel.IsAccessible(position, propertySymbol.SetMethod))
                    {
                        IParameterSymbol parameter = propertySymbol.Parameters.SingleOrDefault(shouldThrow: false);

                        if (parameter != null)
                        {
                            TypeInfo typeInfo = semanticModel.GetTypeInfo(expression, cancellationToken);

                            if (parameter.Type.Equals(typeInfo.ConvertedType))
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        private static Task<Document> RefactorAsync(
            Document document,
            StatementListInfo statementsInfo,
            StatementSyntax statement,
            InitializerExpressionSyntax initializer,
            ExpressionSyntax initializedExpression,
            CancellationToken cancellationToken)
        {
            ExpressionStatementSyntax[] expressions = CreateExpressionStatements(initializer, initializedExpression).ToArray();

            expressions[expressions.Length - 1] = expressions[expressions.Length - 1]
                .WithTrailingTrivia(statement.GetTrailingTrivia());

            var objectCreationExpression = (ObjectCreationExpressionSyntax)initializer.Parent;

            ObjectCreationExpressionSyntax newObjectCreationExpression = objectCreationExpression.WithInitializer(null);

            if (newObjectCreationExpression.ArgumentList == null)
            {
                TypeSyntax type = newObjectCreationExpression.Type;

                newObjectCreationExpression = newObjectCreationExpression
                    .WithArgumentList(ArgumentList().WithTrailingTrivia(type.GetTrailingTrivia()))
                    .WithType(type.WithoutTrailingTrivia());
            }

            SyntaxList<StatementSyntax> statements = statementsInfo.Statements;

            int index = statements.IndexOf(statement);

            StatementSyntax newStatement = statement.ReplaceNode(objectCreationExpression, newObjectCreationExpression);

            SyntaxKind statementKind = statement.Kind();

            if (statementKind == SyntaxKind.ExpressionStatement)
            {
                var expressionStatement = (ExpressionStatementSyntax)newStatement;

                newStatement = expressionStatement
                    .WithExpression(expressionStatement.Expression.WithoutTrailingTrivia());
            }
            else if (statementKind == SyntaxKind.LocalDeclarationStatement)
            {
                var localDeclaration = (LocalDeclarationStatementSyntax)newStatement;

                newStatement = localDeclaration
                    .WithDeclaration(localDeclaration.Declaration.WithoutTrailingTrivia());
            }

            SyntaxList<StatementSyntax> newStatements = statements.Replace(statement, newStatement);

            SyntaxNode newNode = statementsInfo
                .WithStatements(newStatements.InsertRange(index + 1, expressions))
                .Parent
                .WithFormatterAnnotation();

            return document.ReplaceNodeAsync(statementsInfo.Parent, newNode, cancellationToken);
        }

        private static IEnumerable<ExpressionStatementSyntax> CreateExpressionStatements(
            InitializerExpressionSyntax initializer,
            ExpressionSyntax initializedExpression)
        {
            foreach (ExpressionSyntax expression in initializer.Expressions)
            {
                SyntaxKind kind = expression.Kind();

                if (kind == SyntaxKind.SimpleAssignmentExpression)
                {
                    var assignment = (AssignmentExpressionSyntax)expression;
                    ExpressionSyntax left = assignment.Left;
                    ExpressionSyntax right = assignment.Right;

                    if (left.IsKind(SyntaxKind.ImplicitElementAccess))
                    {
                        yield return SimpleAssignmentStatement(
                                ElementAccessExpression(
                                    initializedExpression,
                                    ((ImplicitElementAccessSyntax)left).ArgumentList),
                                right);
                    }
                    else
                    {
                        yield return SimpleAssignmentStatement(
                                SimpleMemberAccessExpression(
                                    initializedExpression,
                                    (IdentifierNameSyntax)left),
                                right);
                    }
                }
                else if (kind == SyntaxKind.ComplexElementInitializerExpression)
                {
                    var elementInitializer = (InitializerExpressionSyntax)expression;

                    yield return SimpleAssignmentStatement(
                            ElementAccessExpression(
                                initializedExpression,
                                BracketedArgumentList(SingletonSeparatedList(Argument(elementInitializer.Expressions[0])))),
                            elementInitializer.Expressions[1]);
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
