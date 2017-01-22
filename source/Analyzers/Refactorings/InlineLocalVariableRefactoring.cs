// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
using Roslynator.CSharp.Extensions;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Refactorings
{
    internal static class InlineLocalVariableRefactoring
    {
        public static DiagnosticDescriptor FadeOutDescriptor
        {
            get { return DiagnosticDescriptors.InlineLocalVariableFadeOut; }
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, LocalDeclarationStatementSyntax localDeclaration)
        {
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
                            SyntaxToken identifier = declarator.Identifier;

                            SyntaxNode parent = localDeclaration.Parent;

                            SyntaxList<StatementSyntax> statements = GetStatements(parent);

                            if (statements.Any())
                            {
                                int index = statements.IndexOf(localDeclaration);

                                if (index < statements.Count - 1)
                                {
                                    StatementSyntax nextStatement = statements[index + 1];

                                    ExpressionSyntax right = GetAssignedValue(nextStatement);

                                    if (right?.IsKind(SyntaxKind.IdentifierName) == true)
                                    {
                                        var identifierName = (IdentifierNameSyntax)right;

                                        if (identifier.ValueText == identifierName.Identifier.ValueText
                                            && !IsLocalVariableReferencedAfterStatement(context, localDeclaration, index, declarator, identifier, statements, parent)
                                            && !parent.ContainsDirectives(TextSpan.FromBounds(localDeclaration.SpanStart, nextStatement.Span.End)))
                                        {
                                            context.ReportDiagnostic(
                                                DiagnosticDescriptors.InlineLocalVariable,
                                                localDeclaration.GetLocation());

                                            foreach (SyntaxToken modifier in localDeclaration.Modifiers)
                                                context.FadeOutToken(FadeOutDescriptor, modifier);

                                            context.FadeOutNode(FadeOutDescriptor, declaration.Type);
                                            context.FadeOutToken(FadeOutDescriptor, identifier);
                                            context.FadeOutToken(FadeOutDescriptor, initializer.EqualsToken);
                                            context.FadeOutToken(FadeOutDescriptor, localDeclaration.SemicolonToken);
                                            context.FadeOutNode(FadeOutDescriptor, right);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
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

        private static bool IsLocalVariableReferencedAfterStatement(
            SyntaxNodeAnalysisContext context,
            LocalDeclarationStatementSyntax localDeclaration,
            int localDeclarationIndex,
            VariableDeclaratorSyntax declarator,
            SyntaxToken identifier,
            SyntaxList<StatementSyntax> statements,
            SyntaxNode parent)
        {
            if (localDeclarationIndex < statements.Count - 2)
            {
                TextSpan span = TextSpan.FromBounds(
                    statements[localDeclarationIndex + 2].SpanStart,
                    statements.Last().Span.End);

                foreach (SyntaxNode node in parent.DescendantNodes(span))
                {
                    if (node.IsKind(SyntaxKind.IdentifierName))
                    {
                        var identifierName = (IdentifierNameSyntax)node;

                        if (identifier.ValueText == identifierName.Identifier.ValueText)
                        {
                            ISymbol symbol = context.SemanticModel.GetSymbol(node, context.CancellationToken);

                            if (symbol?.IsLocal() == true
                                && symbol.Equals(context.SemanticModel.GetDeclaredSymbol(declarator, context.CancellationToken)))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private static SyntaxList<StatementSyntax> GetStatements(SyntaxNode node)
        {
            switch (node?.Kind())
            {
                case SyntaxKind.Block:
                    return ((BlockSyntax)node).Statements;
                case SyntaxKind.SwitchSection:
                    return ((SwitchSectionSyntax)node).Statements;
                default:
                    return default(SyntaxList<StatementSyntax>);
            }
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
                default:
                    {
                        Debug.Assert(false, "");
                        return statement;
                    }
            }
        }
    }
}
