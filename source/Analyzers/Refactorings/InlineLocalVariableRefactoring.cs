// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Refactorings
{
    internal static class InlineLocalVariableRefactoring
    {
        public static void AnalyzeLocalDeclarationStatement(SyntaxNodeAnalysisContext context)
        {
            var localDeclaration = (LocalDeclarationStatementSyntax)context.Node;

            VariableDeclarationSyntax declaration = localDeclaration.Declaration;

            if (declaration != null)
            {
                SeparatedSyntaxList<VariableDeclaratorSyntax> variables = declaration.Variables;

                if (variables.Count == 1)
                {
                    VariableDeclaratorSyntax declarator = variables[0];
                    EqualsValueClauseSyntax initializer = declarator.Initializer;

                    if (initializer != null)
                    {
                        ExpressionSyntax value = initializer.Value;

                        if (value != null)
                        {
                            SyntaxList<StatementSyntax> statements;
                            if (localDeclaration.TryGetContainingList(out statements))
                            {
                                int index = statements.IndexOf(localDeclaration);

                                if (index < statements.Count - 1)
                                {
                                    StatementSyntax nextStatement = statements[index + 1];

                                    ExpressionSyntax right = GetAssignedValue(nextStatement);

                                    SyntaxToken identifier = declarator.Identifier;

                                    if (right?.IsKind(SyntaxKind.IdentifierName) == true)
                                    {
                                        var identifierName = (IdentifierNameSyntax)right;

                                        string name = identifier.ValueText;

                                        if (string.Equals(name, identifierName.Identifier.ValueText, StringComparison.Ordinal))
                                        {
                                            bool isReferenced = false;

                                            if (index < statements.Count - 2)
                                            {
                                                TextSpan span = TextSpan.FromBounds(statements[index + 2].SpanStart, statements.Last().Span.End);

                                                isReferenced = IsLocalVariableReferenced(declarator, name, localDeclaration.Parent, span, context.SemanticModel, context.CancellationToken);
                                            }

                                            if (!isReferenced
                                                && !localDeclaration.Parent.ContainsDirectives(TextSpan.FromBounds(localDeclaration.SpanStart, nextStatement.Span.End)))
                                            {
                                                ReportDiagnostic(context, localDeclaration, declaration, identifier, initializer, right);
                                            }
                                        }
                                    }
                                    else if (nextStatement.IsKind(SyntaxKind.ForEachStatement))
                                    {
                                        var forEachStatement = (ForEachStatementSyntax)nextStatement;

                                        Analyze(context, statements, localDeclaration, declaration, declarator, identifier, initializer, forEachStatement.Expression);
                                    }
                                    else if (nextStatement.IsKind(SyntaxKind.SwitchStatement))
                                    {
                                        var switchStatement = (SwitchStatementSyntax)nextStatement;

                                        Analyze(context, statements, localDeclaration, declaration, declarator, identifier, initializer, switchStatement.Expression);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void Analyze(
            SyntaxNodeAnalysisContext context,
            SyntaxList<StatementSyntax> statements,
            LocalDeclarationStatementSyntax localDeclaration,
            VariableDeclarationSyntax declaration,
            VariableDeclaratorSyntax declarator,
            SyntaxToken identifier,
            EqualsValueClauseSyntax initializer,
            ExpressionSyntax expression)
        {
            if (expression?.IsKind(SyntaxKind.IdentifierName) == true)
            {
                var identifierName = (IdentifierNameSyntax)expression;

                string name = identifier.ValueText;

                if (string.Equals(name, identifierName.Identifier.ValueText, StringComparison.Ordinal))
                {
                    TextSpan span = TextSpan.FromBounds(expression.Span.End, statements.Last().Span.End);

                    if (!IsLocalVariableReferenced(declarator, name, localDeclaration.Parent, span, context.SemanticModel, context.CancellationToken)
                        && !localDeclaration.Parent.ContainsDirectives(TextSpan.FromBounds(localDeclaration.SpanStart, expression.Span.End)))
                    {
                        ReportDiagnostic(context, localDeclaration, declaration, identifier, initializer, expression);
                    }
                }
            }
        }

        private static void ReportDiagnostic(
            SyntaxNodeAnalysisContext context,
            LocalDeclarationStatementSyntax localDeclaration,
            VariableDeclarationSyntax declaration,
            SyntaxToken identifier,
            EqualsValueClauseSyntax initializer,
            ExpressionSyntax expression)
        {
            context.ReportDiagnostic(DiagnosticDescriptors.InlineLocalVariable, localDeclaration);

            foreach (SyntaxToken modifier in localDeclaration.Modifiers)
                context.ReportToken(DiagnosticDescriptors.InlineLocalVariableFadeOut, modifier);

            context.ReportNode(DiagnosticDescriptors.InlineLocalVariableFadeOut, declaration.Type);
            context.ReportToken(DiagnosticDescriptors.InlineLocalVariableFadeOut, identifier);
            context.ReportToken(DiagnosticDescriptors.InlineLocalVariableFadeOut, initializer.EqualsToken);
            context.ReportToken(DiagnosticDescriptors.InlineLocalVariableFadeOut, localDeclaration.SemicolonToken);
            context.ReportNode(DiagnosticDescriptors.InlineLocalVariableFadeOut, expression);
        }

        private static ExpressionSyntax GetAssignedValue(StatementSyntax statement)
        {
            switch (statement.Kind())
            {
                case SyntaxKind.ExpressionStatement:
                    {
                        var expressionStatement = (ExpressionStatementSyntax)statement;

                        ExpressionSyntax expression = expressionStatement.Expression;

                        if (expression?.IsKind(SyntaxKind.SimpleAssignmentExpression) == true)
                        {
                            var assignment = (AssignmentExpressionSyntax)expression;

                            return assignment.Right;
                        }

                        break;
                    }
                case SyntaxKind.LocalDeclarationStatement:
                    {
                        var localDeclaration = (LocalDeclarationStatementSyntax)statement;

                        return localDeclaration
                            .Declaration?
                            .SingleVariableOrDefault()?
                            .Initializer?
                            .Value;
                    }
            }

            return null;
        }

        private static bool IsLocalVariableReferenced(
            VariableDeclaratorSyntax declarator,
            string name,
            SyntaxNode node,
            TextSpan span,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            ISymbol symbol = semanticModel.GetDeclaredSymbol(declarator, cancellationToken);

            return symbol?.IsErrorType() == false
                && IsLocalVariableReferenced(symbol, name, node, span, semanticModel, cancellationToken);
        }

        private static bool IsLocalVariableReferenced(
            ISymbol symbol,
            string name,
            SyntaxNode node,
            TextSpan span,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            foreach (SyntaxNode descendant in node.DescendantNodes(span))
            {
                if (descendant.IsKind(SyntaxKind.IdentifierName))
                {
                    var identifierName = (IdentifierNameSyntax)descendant;

                    if (string.Equals(identifierName.Identifier.ValueText, name, StringComparison.Ordinal)
                        && symbol.Equals(semanticModel.GetSymbol(identifierName, cancellationToken)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            LocalDeclarationStatementSyntax localDeclaration,
            CancellationToken cancellationToken)
        {
            StatementContainer container;
            if (StatementContainer.TryCreate(localDeclaration, out container))
            {
                SyntaxList<StatementSyntax> statements = container.Statements;

                int index = statements.IndexOf(localDeclaration);

                ExpressionSyntax value = localDeclaration
                    .Declaration
                    .Variables
                    .First()
                    .Initializer
                    .Value;

                StatementSyntax nextStatement = statements[index + 1];

                StatementSyntax newStatement = GetStatementWithReplacedValue(nextStatement, value);

                SyntaxTriviaList leadingTrivia = localDeclaration.GetLeadingTrivia();

                IEnumerable<SyntaxTrivia> trivia = container
                    .Node
                    .DescendantTrivia(TextSpan.FromBounds(localDeclaration.SpanStart, nextStatement.Span.Start));

                if (!trivia.All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                {
                    newStatement = newStatement.WithLeadingTrivia(leadingTrivia.Concat(trivia));
                }
                else
                {
                    newStatement = newStatement.WithLeadingTrivia(leadingTrivia);
                }

                SyntaxList<StatementSyntax> newStatements = statements
                    .Replace(nextStatement, newStatement)
                    .RemoveAt(index);

                return await document.ReplaceNodeAsync(container.Node, container.NodeWithStatements(newStatements), cancellationToken).ConfigureAwait(false);
            }

            Debug.Assert(false, "");

            return document;
        }

        private static StatementSyntax GetStatementWithReplacedValue(StatementSyntax statement, ExpressionSyntax newValue)
        {
            switch (statement.Kind())
            {
                case SyntaxKind.ExpressionStatement:
                    {
                        var expressionStatement = (ExpressionStatementSyntax)statement;

                        var assignment = (AssignmentExpressionSyntax)expressionStatement.Expression;

                        AssignmentExpressionSyntax newAssignment = assignment.WithRight(newValue.WithTriviaFrom(assignment.Right));

                        return expressionStatement.WithExpression(newAssignment);
                    }
                case SyntaxKind.LocalDeclarationStatement:
                    {
                        var localDeclaration = (LocalDeclarationStatementSyntax)statement;

                        ExpressionSyntax value = localDeclaration
                            .Declaration
                            .Variables[0]
                            .Initializer
                            .Value;

                        return statement.ReplaceNode(value, newValue.WithTriviaFrom(value));
                    }
                case SyntaxKind.ForEachStatement:
                    {
                        var forEachStatement = (ForEachStatementSyntax)statement;

                        return forEachStatement.WithExpression(newValue.WithTriviaFrom(forEachStatement.Expression));
                    }
                case SyntaxKind.SwitchStatement:
                    {
                        var switchStatement = (SwitchStatementSyntax)statement;

                        return switchStatement.WithExpression(newValue.WithTriviaFrom(switchStatement.Expression));
                    }
                default:
                    {
                        Debug.Assert(false, "");
                        return statement;
                    }
            }
        }
    }
}
