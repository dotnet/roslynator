// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
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
    internal static class ReplaceForWithForEachRefactoring
    {
        public static async Task<Document> RefactorAsync(
            Document document,
            ForStatementSyntax forStatement,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            StatementSyntax statement = forStatement.Statement;

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            string name = NameGenerator.Default.EnsureUniqueLocalName(
                DefaultNames.ForEachVariable,
                semanticModel,
                statement.SpanStart,
                cancellationToken: cancellationToken);

            IdentifierNameSyntax identifierName = IdentifierName(name);

            var condition = (BinaryExpressionSyntax)forStatement.Condition;
            var memberAccessExpression = (MemberAccessExpressionSyntax)condition.Right;
            ExpressionSyntax expression = memberAccessExpression.Expression;

            ISymbol symbol = semanticModel.GetDeclaredSymbol(forStatement.Declaration.Variables.First(), cancellationToken);
            ImmutableArray<SyntaxNode> nodes = await SyntaxFinder.FindReferencesAsync(symbol, document, cancellationToken: cancellationToken).ConfigureAwait(false);

            StatementSyntax newStatement = statement.ReplaceNodes(
                nodes.Select(f => f.Parent.Parent.Parent),
                (f, _) => identifierName.WithTriviaFrom(f));

            ForEachStatementSyntax forEachStatement = ForEachStatement(
                VarType(),
                name,
                expression,
                newStatement);

            forEachStatement = forEachStatement
                .WithTriviaFrom(forStatement)
                .WithFormatterAnnotation();

            return await document.ReplaceNodeAsync(forStatement, forEachStatement, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<bool> CanRefactorAsync(RefactoringContext context, ForStatementSyntax forStatement)
        {
            VariableDeclaratorSyntax variableDeclarator = forStatement
                .Declaration?
                .Variables
                .SingleOrDefault(shouldthrow: false);

            if (variableDeclarator != null)
            {
                ExpressionSyntax value = variableDeclarator.Initializer?.Value;

                if (value?.IsNumericLiteralExpression("0") == true)
                {
                    ExpressionSyntax condition = forStatement.Condition;

                    if (condition?.IsKind(SyntaxKind.LessThanExpression) == true)
                    {
                        ExpressionSyntax right = ((BinaryExpressionSyntax)condition).Right;

                        if (right?.IsKind(SyntaxKind.SimpleMemberAccessExpression) == true)
                        {
                            var memberAccess = (MemberAccessExpressionSyntax)right;

                            string memberName = memberAccess.Name?.Identifier.ValueText;

                            if (memberName == "Count" || memberName == "Length")
                            {
                                ExpressionSyntax expression = memberAccess.Expression;

                                if (expression != null)
                                {
                                    SeparatedSyntaxList<ExpressionSyntax> incrementors = forStatement.Incrementors;

                                    if (incrementors.Count == 1
                                        && incrementors.First().IsKind(SyntaxKind.PostIncrementExpression))
                                    {
                                        return await IsElementAccessAsync(context, forStatement, variableDeclarator, expression).ConfigureAwait(false);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        private static async Task<bool> IsElementAccessAsync(
            RefactoringContext context,
            ForStatementSyntax forStatement,
            VariableDeclaratorSyntax variableDeclarator,
            ExpressionSyntax memberAccessExpression)
        {
            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            ISymbol symbol = semanticModel.GetSymbol(memberAccessExpression, context.CancellationToken);

            if (symbol != null)
            {
                ISymbol variableSymbol = semanticModel.GetDeclaredSymbol(variableDeclarator, context.CancellationToken);

                ImmutableArray<SyntaxNode> nodes = await SyntaxFinder.FindReferencesAsync(variableSymbol, context.Document, cancellationToken: context.CancellationToken).ConfigureAwait(false);

                StatementSyntax statement = forStatement.Statement;

                return nodes
                    .Where(f => statement.Span.Contains(f.Span))
                    .All(node => IsElementAccess(node, symbol, semanticModel, context.CancellationToken));
            }

            return true;
        }

        private static bool IsElementAccess(
            SyntaxNode node,
            ISymbol symbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (node.IsKind(SyntaxKind.IdentifierName))
            {
                SyntaxNode parent = node.Parent;

                if (parent?.IsKind(SyntaxKind.Argument) == true)
                {
                    parent = parent.Parent;

                    if (parent?.IsKind(SyntaxKind.BracketedArgumentList) == true)
                    {
                        parent = parent.Parent;

                        if (parent?.IsKind(SyntaxKind.ElementAccessExpression) == true)
                        {
                            var elementAccess = (ElementAccessExpressionSyntax)parent;

                            ExpressionSyntax expression = elementAccess.Expression;

                            if (expression != null)
                            {
                                ISymbol expressionSymbol = semanticModel.GetSymbol(expression, cancellationToken);

                                return symbol.Equals(expressionSymbol);
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}
