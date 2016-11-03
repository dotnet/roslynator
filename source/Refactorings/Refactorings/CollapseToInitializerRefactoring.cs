// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CollapseToInitializerRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, SelectedStatementsInfo info)
        {
            StatementSyntax firstStatement = info.FirstSelectedNode;
            SemanticModel semanticModel = null;
            ISymbol symbol = null;
            ObjectCreationExpressionSyntax objectCreation = null;

            if (info.AreManySelected)
            {
                SyntaxKind kind = firstStatement.Kind();

                if (kind == SyntaxKind.LocalDeclarationStatement)
                {
                    var localDeclaration = (LocalDeclarationStatementSyntax)firstStatement;

                    VariableDeclarationSyntax declaration = localDeclaration.Declaration;

                    if (declaration != null)
                    {
                        SeparatedSyntaxList<VariableDeclaratorSyntax> variables = declaration.Variables;

                        if (variables.Count == 1)
                        {
                            VariableDeclaratorSyntax variable = variables[0];

                            objectCreation = variable.Initializer?.Value as ObjectCreationExpressionSyntax;

                            if (objectCreation != null)
                            {
                                semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                                symbol = semanticModel.GetDeclaredSymbol(variable, context.CancellationToken);
                            }
                        }
                    }
                }
                else if (kind == SyntaxKind.ExpressionStatement)
                {
                    var expressionStatement = (ExpressionStatementSyntax)firstStatement;

                    var assignment = expressionStatement.Expression as AssignmentExpressionSyntax;

                    if (assignment != null)
                    {
                        objectCreation = assignment.Right as ObjectCreationExpressionSyntax;

                        if (objectCreation != null)
                        {
                            semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            symbol = semanticModel.GetSymbolInfo(assignment.Left, context.CancellationToken).Symbol;
                        }
                    }
                }

                if (objectCreation != null
                    && symbol?.IsErrorType() == false
                    && info
                        .SelectedNodes()
                        .Skip(1)
                        .All(f => IsValidAssignmentStatement(f, symbol, semanticModel, context.CancellationToken)))
                {
                    context.RegisterRefactoring(
                        "Collapse to initializer",
                        cancellationToken =>
                        {
                            return RefactorAsync(
                                context.Document,
                                objectCreation,
                                info,
                                cancellationToken);
                        });
                }
            }
        }

        public static bool IsValidAssignmentStatement(
            StatementSyntax statement,
            ISymbol symbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (statement.IsKind(SyntaxKind.ExpressionStatement))
            {
                var expressionStatement = (ExpressionStatementSyntax)statement;

                if (expressionStatement.Expression?.IsKind(SyntaxKind.SimpleAssignmentExpression) == true)
                {
                    var assignment = (AssignmentExpressionSyntax)expressionStatement.Expression;

                    if (assignment.Left?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
                    {
                        var memberAccess = (MemberAccessExpressionSyntax)assignment.Left;

                        ISymbol expressionSymbol = semanticModel
                            .GetSymbolInfo(memberAccess.Expression, cancellationToken)
                            .Symbol;

                        if (symbol.Equals(expressionSymbol))
                        {
                            ISymbol leftSymbol = semanticModel.GetSymbolInfo(assignment.Left, cancellationToken).Symbol;

                            if (leftSymbol?.IsProperty() == true)
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ObjectCreationExpressionSyntax objectCreation,
            SelectedStatementsInfo info,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            StatementContainer container = info.Container;

            ExpressionStatementSyntax[] expressionStatements = info
                .SelectedNodes()
                .Skip(1)
                .Cast<ExpressionStatementSyntax>()
                .ToArray();

            StatementSyntax firstNode = info.FirstSelectedNode;

            SyntaxList<StatementSyntax> newStatements = container.Statements.Replace(
                firstNode,
                firstNode.ReplaceNode(
                    objectCreation,
                    objectCreation.WithInitializer(CreateInitializer(objectCreation, expressionStatements))));

            int count = expressionStatements.Length;
            int index = info.FirstSelectedNodeIndex + 1;

            while (count > 0)
            {
                newStatements = newStatements.RemoveAt(index);
                count--;
            }

            SyntaxNode newRoot = root.ReplaceNode(
                container.Node,
                container.NodeWithStatements(newStatements));

            return document.WithSyntaxRoot(newRoot);
        }

        private static InitializerExpressionSyntax CreateInitializer(ObjectCreationExpressionSyntax objectCreation, ExpressionStatementSyntax[] expressionStatements)
        {
            InitializerExpressionSyntax initializer = objectCreation.Initializer
                ?? SyntaxFactory.InitializerExpression(SyntaxKind.ObjectInitializerExpression);

            var expressions = new AssignmentExpressionSyntax[expressionStatements.Length];

            for (int i = 0; i < expressionStatements.Length; i++)
            {
                var assignment = (AssignmentExpressionSyntax)expressionStatements[i].Expression;

                var memberAccess = (MemberAccessExpressionSyntax)assignment.Left;

                expressions[i] = assignment.ReplaceNode(memberAccess, memberAccess.Name);
            }

            return initializer
                .AddExpressions(expressions)
                .WithFormatterAnnotation();
        }
    }
}
