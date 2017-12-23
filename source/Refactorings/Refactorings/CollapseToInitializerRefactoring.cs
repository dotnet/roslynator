// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class CollapseToInitializerRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, StatementsSelection selectedStatements)
        {
            if (selectedStatements.Count > 1)
            {
                StatementSyntax firstStatement = selectedStatements.First();

                SemanticModel semanticModel = null;
                ISymbol symbol = null;
                ObjectCreationExpressionSyntax objectCreation = null;

                SyntaxKind kind = firstStatement.Kind();

                if (kind == SyntaxKind.LocalDeclarationStatement)
                {
                    var localDeclaration = (LocalDeclarationStatementSyntax)firstStatement;

                    VariableDeclaratorSyntax variable = localDeclaration
                        .Declaration?
                        .Variables
                        .SingleOrDefault(shouldthrow: false);

                    objectCreation = variable?.Initializer?.Value as ObjectCreationExpressionSyntax;

                    if (objectCreation != null)
                    {
                        semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                        symbol = semanticModel.GetDeclaredSymbol(variable, context.CancellationToken);
                    }
                }
                else if (kind == SyntaxKind.ExpressionStatement)
                {
                    var expressionStatement = (ExpressionStatementSyntax)firstStatement;

                    if (expressionStatement.Expression is AssignmentExpressionSyntax assignment)
                    {
                        objectCreation = assignment.Right as ObjectCreationExpressionSyntax;

                        if (objectCreation != null)
                        {
                            semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                            symbol = semanticModel.GetSymbol(assignment.Left, context.CancellationToken);
                        }
                    }
                }

                if (objectCreation != null
                    && symbol?.IsErrorType() == false
                    && selectedStatements
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
                                selectedStatements,
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
                            .GetSymbol(memberAccess.Expression, cancellationToken);

                        if (symbol.Equals(expressionSymbol))
                        {
                            ISymbol leftSymbol = semanticModel.GetSymbol(assignment.Left, cancellationToken);

                            if (leftSymbol?.IsProperty() == true)
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            ObjectCreationExpressionSyntax objectCreation,
            StatementsSelection selectedStatements,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            StatementsInfo statementsInfo = selectedStatements.Info;

            ExpressionStatementSyntax[] expressionStatements = selectedStatements
                .Skip(1)
                .Cast<ExpressionStatementSyntax>()
                .ToArray();

            StatementSyntax firstStatement = selectedStatements.First();

            SyntaxList<StatementSyntax> newStatements = statementsInfo.Statements.Replace(
                firstStatement,
                firstStatement.ReplaceNode(
                    objectCreation,
                    objectCreation.WithInitializer(CreateInitializer(objectCreation, expressionStatements))));

            int count = expressionStatements.Length;
            int index = selectedStatements.StartIndex + 1;

            while (count > 0)
            {
                newStatements = newStatements.RemoveAt(index);
                count--;
            }

            return document.ReplaceStatementsAsync(statementsInfo, newStatements, cancellationToken);
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
