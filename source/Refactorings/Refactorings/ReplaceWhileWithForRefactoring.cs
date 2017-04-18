// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class ReplaceWhileWithForRefactoring
    {
        private const string Title = "Replace while with for";

        public static void ComputeRefactoring(RefactoringContext context, WhileStatementSyntax whileStatement)
        {
            context.RegisterRefactoring(
                Title,
                cancellationToken => RefactorAsync(context.Document, null, null, whileStatement, cancellationToken));
        }

        public static async Task ComputeRefactoringAsync(RefactoringContext context, StatementContainerSelection selectedStatements)
        {
            if (selectedStatements.Count > 1)
            {
                StatementSyntax lastStatement = selectedStatements.Last();

                if (lastStatement.IsKind(SyntaxKind.WhileStatement))
                {
                    IEnumerable<StatementSyntax> statements = selectedStatements
                        .Take(selectedStatements.EndIndex - selectedStatements.StartIndex);

                    SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

                    List<LocalDeclarationStatementSyntax> localDeclarations = null;
                    List<ExpressionSyntax> expressions = null;

                    foreach (StatementSyntax statement in statements)
                    {
                        SyntaxKind kind = statement.Kind();

                        if (kind == SyntaxKind.LocalDeclarationStatement)
                        {
                            var localDeclaration = (LocalDeclarationStatementSyntax)statement;

                            if (!IsAnySymbolReferenced(localDeclaration, selectedStatements.ToImmutableArray(), selectedStatements.EndIndex + 1, semanticModel, context.CancellationToken))
                            {
                                (localDeclarations ?? (localDeclarations = new List<LocalDeclarationStatementSyntax>())).Add(localDeclaration);
                            }
                            else
                            {
                                return;
                            }
                        }
                        else if (kind == SyntaxKind.ExpressionStatement)
                        {
                            var expressionStatement = (ExpressionStatementSyntax)statement;

                            ExpressionSyntax expression = expressionStatement.Expression;

                            if (expression != null)
                            {
                                if (CanBeInitializer(expression))
                                {
                                    (expressions ?? (expressions = new List<ExpressionSyntax>())).Add(expression);
                                }
                                else
                                {
                                    return;
                                }
                            }
                        }
                        else
                        {
                            return;
                        }
                    }

                    if (localDeclarations == null
                        || expressions == null)
                    {
                        if (localDeclarations == null
                            || localDeclarations
                                .Select(f => f.Declaration)
                                .Where(f => f != null)
                                .Select(f => semanticModel.GetTypeSymbol(f.Type, context.CancellationToken))
                                .Distinct()
                                .Count() == 1)
                        {
                            context.RegisterRefactoring(
                            Title,
                            cancellationToken => RefactorAsync(context.Document, localDeclarations, expressions, (WhileStatementSyntax)lastStatement, cancellationToken));
                        }
                    }
                }
            }
        }

        private static bool IsAnySymbolReferenced(
            LocalDeclarationStatementSyntax localDeclaration,
            ImmutableArray<StatementSyntax> statements,
            int startIndex,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            VariableDeclarationSyntax declaration = localDeclaration.Declaration;

            if (declaration != null)
            {
                foreach (VariableDeclaratorSyntax variable in declaration.Variables)
                {
                    ISymbol symbol = semanticModel.GetDeclaredSymbol(variable, cancellationToken);

                    if (IsSymbolReferenced(symbol, statements, startIndex, semanticModel, cancellationToken))
                        return true;
                }
            }

            return false;
        }

        private static bool IsSymbolReferenced(
             ISymbol symbol,
             ImmutableArray<StatementSyntax> statements,
             int startIndex,
             SemanticModel semanticModel,
             CancellationToken cancellationToken)
        {
            for (int i = startIndex; i < statements.Length; i++)
            {
                foreach (SyntaxNode node in statements[i].DescendantNodes())
                {
                    if (node.IsKind(SyntaxKind.IdentifierName)
                        && symbol.Equals(semanticModel.GetSymbol(node, cancellationToken)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static async Task<Document> RefactorAsync(
            Document document,
            List<LocalDeclarationStatementSyntax> localDeclarations,
            List<ExpressionSyntax> expressions,
            WhileStatementSyntax whileStatement,
            CancellationToken cancellationToken)
        {
            var declaration = default(VariableDeclarationSyntax);
            var initializers = default(SeparatedSyntaxList<ExpressionSyntax>);
            var incrementors = default(SeparatedSyntaxList<ExpressionSyntax>);

            if (localDeclarations != null)
            {
                IEnumerable<VariableDeclarationSyntax> declarations = localDeclarations
                    .Select(f => f.Declaration)
                    .Where(f => f != null);

                TypeSyntax type = declarations.First().Type.TrimTrivia();

                IEnumerable<VariableDeclaratorSyntax> variables = declarations
                    .SelectMany(f => f.Variables)
                    .Select(f => f.TrimTrivia());

                declaration = VariableDeclaration(type, SeparatedList(variables));

                StatementContainer container;
                if (StatementContainer.TryCreate(whileStatement, out container))
                {
                    SyntaxList<StatementSyntax> statements = container.Statements;

                    int index = statements.IndexOf(localDeclarations[0]);

                    ForStatementSyntax forStatement = CreateForStatement(whileStatement, declaration, initializers, incrementors);

                    forStatement = forStatement
                        .TrimLeadingTrivia()
                        .PrependToLeadingTrivia(localDeclarations[0].GetLeadingTrivia());

                    IEnumerable<StatementSyntax> newStatements = statements.Take(index)
                        .Concat(new ForStatementSyntax[] { forStatement })
                        .Concat(statements.Skip(index + localDeclarations.Count + 1));

                    return await document.ReplaceNodeAsync(container.Node, container.NodeWithStatements(newStatements), cancellationToken).ConfigureAwait(false);
                }
            }
            else if (expressions != null)
            {
                initializers = SeparatedList(expressions);
            }

            return await document.ReplaceNodeAsync(
                whileStatement,
                CreateForStatement(whileStatement, declaration, initializers, incrementors),
                cancellationToken).ConfigureAwait(false);
        }

        private static ForStatementSyntax CreateForStatement(
            WhileStatementSyntax whileStatement,
            VariableDeclarationSyntax declaration,
            SeparatedSyntaxList<ExpressionSyntax> initializers,
            SeparatedSyntaxList<ExpressionSyntax> incrementors)
        {
            StatementSyntax statement = whileStatement.Statement;

            if (statement?.IsKind(SyntaxKind.Block) == true)
            {
                var block = (BlockSyntax)statement;

                incrementors = SeparatedList(GetIncrementors(block));

                if (incrementors.Any())
                {
                    SyntaxList<StatementSyntax> statements = block.Statements;

                    statement = block.WithStatements(List(statements.Take(statements.Count - incrementors.Count)));
                }
            }

            return ForStatement(
                declaration,
                initializers,
                whileStatement.Condition,
                incrementors,
                statement);
        }

        private static IEnumerable<ExpressionSyntax> GetIncrementors(BlockSyntax block)
        {
            SyntaxList<StatementSyntax> statements = block.Statements;

            for (int i = statements.Count - 1; i >= 0; i--)
            {
                if (statements[i].IsKind(SyntaxKind.ExpressionStatement))
                {
                    var expressionStatement = (ExpressionStatementSyntax)statements[i];

                    ExpressionSyntax expression = expressionStatement.Expression;

                    if (expression != null)
                    {
                        if (expression.IsIncrementOrDecrementExpression())
                        {
                            yield return expression;
                        }
                        else
                        {
                            yield break;
                        }
                    }
                }
                else
                {
                    yield break;
                }
            }
        }

        private static bool CanBeInitializer(ExpressionSyntax expression)
        {
            switch (expression.Kind())
            {
                case SyntaxKind.SimpleAssignmentExpression:
                case SyntaxKind.InvocationExpression:
                case SyntaxKind.PreIncrementExpression:
                case SyntaxKind.PreDecrementExpression:
                case SyntaxKind.PostIncrementExpression:
                case SyntaxKind.PostDecrementExpression:
                case SyntaxKind.ObjectCreationExpression:
                case SyntaxKind.AwaitExpression:
                    return true;
                default:
                    return false;
            }
        }
    }
}