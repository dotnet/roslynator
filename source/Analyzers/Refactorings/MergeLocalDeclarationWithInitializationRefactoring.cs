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
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class MergeLocalDeclarationWithInitializationRefactoring
    {
        public static DiagnosticDescriptor FadeOutDescriptor
        {
            get { return DiagnosticDescriptors.MergeLocalDeclarationWithInitializationFadeOut; }
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, LocalDeclarationStatementSyntax localDeclaration)
        {
            if (!localDeclaration.IsConst)
            {
                VariableDeclarationSyntax declaration = localDeclaration.Declaration;

                if (declaration != null)
                {
                    SeparatedSyntaxList<VariableDeclaratorSyntax> variables = declaration.Variables;

                    if (variables.Any())
                    {
                        SyntaxList<StatementSyntax> statements = StatementContainer.GetStatements(localDeclaration);

                        if (statements.Any())
                        {
                            int index = statements.IndexOf(localDeclaration);

                            if (index < statements.Count - 1)
                            {
                                StatementSyntax nextStatement = statements[index + 1];

                                if (nextStatement.IsKind(SyntaxKind.ExpressionStatement))
                                {
                                    var expressionStatement = (ExpressionStatementSyntax)nextStatement;

                                    ExpressionSyntax expression = expressionStatement.Expression;

                                    if (expression?.IsKind(SyntaxKind.SimpleAssignmentExpression) == true)
                                    {
                                        var assignment = (AssignmentExpressionSyntax)expression;

                                        ExpressionSyntax left = assignment.Left;

                                        if (left.IsKind(SyntaxKind.IdentifierName))
                                        {
                                            ExpressionSyntax right = assignment.Right;

                                            if (right?.IsMissing == false)
                                            {
                                                VariableDeclaratorSyntax declarator = FindInitializedVariable(variables, (IdentifierNameSyntax)left, context.SemanticModel, context.CancellationToken);

                                                if (declarator != null)
                                                {
                                                    EqualsValueClauseSyntax initializer = declarator.Initializer;
                                                    ExpressionSyntax value = initializer?.Value;

                                                    if (value == null
                                                        || IsDefaultValue(declaration.Type, value, context.SemanticModel, context.CancellationToken))
                                                    {
                                                        TextSpan span = TextSpan.FromBounds(
                                                            (value != null) ? value.SpanStart : declarator.Span.End,
                                                            right.Span.Start);

                                                        if (!localDeclaration.Parent.ContainsDirectives(span))
                                                        {
                                                            context.ReportDiagnostic(
                                                                DiagnosticDescriptors.MergeLocalDeclarationWithInitialization,
                                                                declarator.Identifier);

                                                            if (value != null)
                                                                context.ReportNode(FadeOutDescriptor, initializer);

                                                            context.ReportToken(FadeOutDescriptor, localDeclaration.SemicolonToken);
                                                            context.ReportNode(FadeOutDescriptor, left);

                                                            if (value != null)
                                                                context.ReportToken(FadeOutDescriptor, assignment.OperatorToken);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static VariableDeclaratorSyntax FindInitializedVariable(
            SeparatedSyntaxList<VariableDeclaratorSyntax> declarators,
            IdentifierNameSyntax identifierName,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            string name = identifierName.Identifier.ValueText;

            ILocalSymbol localSymbol = null;

            foreach (VariableDeclaratorSyntax declarator in declarators)
            {
                if (name == declarator.Identifier.ValueText)
                {
                    if (localSymbol == null)
                    {
                        localSymbol = semanticModel.GetSymbol(identifierName, cancellationToken) as ILocalSymbol;

                        if (localSymbol == null)
                            return null;
                    }

                    if (localSymbol.Equals(semanticModel.GetDeclaredSymbol(declarator, cancellationToken)))
                        return declarator;
                }
            }

            return null;
        }

        private static bool IsDefaultValue(TypeSyntax type, ExpressionSyntax value, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(type, cancellationToken);

            if (typeSymbol != null)
            {
                return semanticModel.IsDefaultValue(typeSymbol, value, cancellationToken);
            }
            else
            {
                return false;
            }
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            VariableDeclaratorSyntax declarator,
            CancellationToken cancellationToken)
        {
            var declaration = (VariableDeclarationSyntax)declarator.Parent;

            var localDeclaration = (LocalDeclarationStatementSyntax)declaration.Parent;

            StatementContainer container;

            if (StatementContainer.TryCreate(localDeclaration, out container))
            {
                SyntaxList<StatementSyntax> statements = container.Statements;

                int index = statements.IndexOf(localDeclaration);

                StatementSyntax nextStatement = statements[index + 1];

                var expressionStatement = (ExpressionStatementSyntax)nextStatement;

                var assignment = (AssignmentExpressionSyntax)expressionStatement.Expression;

                ExpressionSyntax right = assignment.Right;

                EqualsValueClauseSyntax initializer = declarator.Initializer;

                ExpressionSyntax value = initializer?.Value;

                VariableDeclaratorSyntax newDeclarator = (value != null)
                    ? declarator.ReplaceNode(value, right)
                    : declarator.WithInitializer(EqualsValueClause(right));

                LocalDeclarationStatementSyntax newLocalDeclaration = localDeclaration.ReplaceNode(declarator, newDeclarator);

                SyntaxTriviaList trailingTrivia = nextStatement.GetTrailingTrivia();

                IEnumerable<SyntaxTrivia> trivia = container
                    .Node
                    .DescendantTrivia(TextSpan.FromBounds(localDeclaration.Span.End, right.SpanStart));

                if (!trivia.All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                {
                    newLocalDeclaration = newLocalDeclaration.WithTrailingTrivia(trivia.Concat(trailingTrivia));
                }
                else
                {
                    newLocalDeclaration = newLocalDeclaration.WithTrailingTrivia(trailingTrivia);
                }

                SyntaxList<StatementSyntax> newStatements = statements
                    .Replace(localDeclaration, newLocalDeclaration)
                    .RemoveAt(index + 1);

                return await document.ReplaceNodeAsync(container.Node, container.NodeWithStatements(newStatements), cancellationToken).ConfigureAwait(false);
            }

            Debug.Assert(false, "");

            return document;
        }
    }
}