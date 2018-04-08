// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
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
                .SingleOrDefault(shouldThrow: false);

            if (variableDeclarator == null)
                return false;

            if (variableDeclarator.Initializer?.Value?.IsNumericLiteralExpression("0") != true)
                return false;

            ExpressionSyntax condition = forStatement.Condition;

            if (condition?.IsKind(SyntaxKind.LessThanExpression) != true)
                return false;

            ExpressionSyntax right = ((BinaryExpressionSyntax)condition).Right;

            if (right?.IsKind(SyntaxKind.SimpleMemberAccessExpression) != true)
                return false;

            var memberAccessExpression = (MemberAccessExpressionSyntax)right;

            string memberName = memberAccessExpression.Name?.Identifier.ValueText;

            if (memberName != "Count" && memberName != "Length")
                return false;

            ExpressionSyntax expression = memberAccessExpression.Expression;

            if (expression == null)
                return false;

            if (forStatement.Incrementors.SingleOrDefault(shouldThrow: false)?.IsKind(SyntaxKind.PostIncrementExpression) != true)
                return false;

            SemanticModel semanticModel = await context.GetSemanticModelAsync().ConfigureAwait(false);

            ISymbol symbol = semanticModel.GetSymbol(expression, context.CancellationToken);

            if (symbol == null)
                return false;

            ISymbol variableSymbol = semanticModel.GetDeclaredSymbol(variableDeclarator, context.CancellationToken);

            ImmutableArray<SyntaxNode> nodes = await SyntaxFinder.FindReferencesAsync(variableSymbol, context.Document, cancellationToken: context.CancellationToken).ConfigureAwait(false);

            TextSpan span = forStatement.Statement.Span;

            foreach (SyntaxNode node in nodes)
            {
                if (span.Contains(node.Span)
                    && !IsElementAccess(node, symbol, semanticModel, context.CancellationToken))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsElementAccess(
            SyntaxNode node,
            ISymbol symbol,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            if (!node.IsKind(SyntaxKind.IdentifierName))
                return false;

            SyntaxNode parent = node.Parent;

            if (parent?.IsKind(SyntaxKind.Argument) != true)
                return false;

            parent = parent.Parent;

            if (parent?.IsKind(SyntaxKind.BracketedArgumentList) != true)
                return false;

            parent = parent.Parent;

            if (parent?.IsKind(SyntaxKind.ElementAccessExpression) != true)
                return false;

            var elementAccess = (ElementAccessExpressionSyntax)parent;

            ExpressionSyntax expression = elementAccess.Expression;

            if (expression == null)
                return false;

            ISymbol expressionSymbol = semanticModel.GetSymbol(expression, cancellationToken);

            return symbol.Equals(expressionSymbol);
        }
    }
}
