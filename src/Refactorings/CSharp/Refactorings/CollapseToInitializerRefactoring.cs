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
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, StatementListSelection selectedStatements)
        {
            if (selectedStatements.Count <= 1)
                return;

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
                    .SingleOrDefault(shouldThrow: false);

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

            if (objectCreation == null)
                return;

            if (symbol?.IsErrorType() != false)
                return;

            for (int i = 1; i < selectedStatements.Count; i++)
            {
                if (!IsValidAssignmentStatement(selectedStatements[i], symbol, semanticModel, context.CancellationToken))
                    return;
            }

            context.RegisterRefactoring(
                "Collapse to initializer",
                ct => RefactorAsync(context.Document, objectCreation, selectedStatements, ct));
        }

        public static bool IsValidAssignmentStatement(
            StatementSyntax statement,
            ISymbol symbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            SimpleAssignmentStatementInfo simpleAssignment = SyntaxInfo.SimpleAssignmentStatementInfo(statement);

            if (!simpleAssignment.Success)
                return false;

            ExpressionSyntax left = simpleAssignment.Left;

            if (left.Kind() != SyntaxKind.SimpleMemberAccessExpression)
                return false;

            var memberAccess = (MemberAccessExpressionSyntax)left;

            ISymbol expressionSymbol = semanticModel.GetSymbol(memberAccess.Expression, cancellationToken);

            if (!symbol.Equals(expressionSymbol))
                return false;

            return semanticModel.GetSymbol(left, cancellationToken)?.Kind == SymbolKind.Property;
        }

        public static Task<Document> RefactorAsync(
            Document document,
            ObjectCreationExpressionSyntax objectCreation,
            StatementListSelection selectedStatements,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ExpressionStatementSyntax[] expressionStatements = selectedStatements
                .Skip(1)
                .Cast<ExpressionStatementSyntax>()
                .ToArray();

            StatementSyntax firstStatement = selectedStatements.First();

            SyntaxList<StatementSyntax> newStatements = selectedStatements.UnderlyingList.Replace(
                firstStatement,
                firstStatement.ReplaceNode(
                    objectCreation,
                    objectCreation.WithInitializer(CreateInitializer(objectCreation, expressionStatements))));

            int count = expressionStatements.Length;
            int index = selectedStatements.FirstIndex + 1;

            while (count > 0)
            {
                newStatements = newStatements.RemoveAt(index);
                count--;
            }

            return document.ReplaceStatementsAsync(SyntaxInfo.StatementListInfo(selectedStatements), newStatements, cancellationToken);
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
