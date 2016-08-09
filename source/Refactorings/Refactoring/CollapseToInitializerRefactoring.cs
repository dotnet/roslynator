// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Pihrtsoft.CodeAnalysis.CSharp.Refactoring
{
    internal static class CollapseToInitializerRefactoring
    {
        public static async Task ComputeRefactoringsAsync(RefactoringContext context, BlockSpan blockSpan)
        {
            StatementSyntax firstStatement = blockSpan.FirstSelectedStatement;
            SemanticModel semanticModel = null;
            ISymbol symbol = null;
            ObjectCreationExpressionSyntax objectCreation = null;

            using (IEnumerator<StatementSyntax> en = blockSpan.SelectedStatements().GetEnumerator())
            {
                en.MoveNext();

                if (en.MoveNext())
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
                        && symbol?.IsErrorType() == false)
                    {
                        List<ExpressionStatementSyntax> statements = GetExpressionStatements(en, symbol, semanticModel, context.CancellationToken);

                        if (statements?.Count > 0)
                        {
                            context.RegisterRefactoring(
                                "Collapse to initializer",
                                cancellationToken =>
                                {
                                    return RefactorAsync(
                                        context.Document,
                                        objectCreation,
                                        statements.ToImmutableArray(),
                                        cancellationToken);
                                });
                        }
                    }
                }
            }
        }

        private static List<ExpressionStatementSyntax> GetExpressionStatements(
            IEnumerator<StatementSyntax> en,
            ISymbol symbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ExpressionStatementSyntax expressionStatement = GetExpressionStatement(en.Current, symbol, semanticModel, cancellationToken);

            if (expressionStatement != null)
            {
                var statements = new List<ExpressionStatementSyntax>();
                statements.Add(expressionStatement);

                while (en.MoveNext())
                {
                    expressionStatement = GetExpressionStatement(en.Current, symbol, semanticModel, cancellationToken);

                    if (expressionStatement != null)
                    {
                        statements.Add(expressionStatement);
                    }
                    else
                    {
                        return null;
                    }
                }

                return statements;
            }

            return null;
        }

        public static ExpressionStatementSyntax GetExpressionStatement(
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
                            {
                                return expressionStatement;
                            }
                        }
                    }
                }
            }

            return null;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            ObjectCreationExpressionSyntax objectCreation,
            ImmutableArray<ExpressionStatementSyntax> expressionStatements,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxNode root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);

            var block = (BlockSyntax)expressionStatements[0].Parent;

            int index = block.Statements.IndexOf(expressionStatements[0]);

            BlockSyntax newBlock = block.ReplaceNode(
                objectCreation,
                objectCreation.WithInitializer(CreateInitializer(objectCreation, expressionStatements)));

            SyntaxList<StatementSyntax> statements = newBlock.Statements;

            int count = expressionStatements.Length;

            while (count > 0)
            {
                statements = statements.RemoveAt(index);
                count--;
            }

            root = root.ReplaceNode(
                block,
                newBlock.WithStatements(statements));

            return document.WithSyntaxRoot(root);
        }

        private static InitializerExpressionSyntax CreateInitializer(ObjectCreationExpressionSyntax objectCreation, ImmutableArray<ExpressionStatementSyntax> expressionStatements)
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
