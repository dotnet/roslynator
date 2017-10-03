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
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Refactorings
{
    internal static class InlineLocalVariableRefactoring
    {
        public static void AnalyzeLocalDeclarationStatement(SyntaxNodeAnalysisContext context)
        {
            var localDeclarationStatement = (LocalDeclarationStatementSyntax)context.Node;

            if (!localDeclarationStatement.ContainsDiagnostics
                && !localDeclarationStatement.SpanOrTrailingTriviaContainsDirectives())
            {
                SingleLocalDeclarationStatement localDeclaration;
                if (SingleLocalDeclarationStatement.TryCreate(localDeclarationStatement, out localDeclaration))
                {
                    ExpressionSyntax value = localDeclaration.Initializer?.Value;

                    if (value != null)
                    {
                        SyntaxList<StatementSyntax> statements;
                        if (localDeclarationStatement.TryGetContainingList(out statements))
                        {
                            int index = statements.IndexOf(localDeclarationStatement);

                            if (index < statements.Count - 1)
                            {
                                StatementSyntax nextStatement = statements[index + 1];

                                if (!nextStatement.ContainsDiagnostics)
                                {
                                    switch (nextStatement.Kind())
                                    {
                                        case SyntaxKind.ExpressionStatement:
                                            {
                                                Analyze(context, statements, localDeclaration, index, (ExpressionStatementSyntax)nextStatement);
                                                break;
                                            }
                                        case SyntaxKind.LocalDeclarationStatement:
                                            {
                                                Analyze(context, statements, localDeclaration, index, (LocalDeclarationStatementSyntax)nextStatement);
                                                break;
                                            }
                                        case SyntaxKind.ReturnStatement:
                                            {
                                                var returnStatement = (ReturnStatementSyntax)nextStatement;
                                                if (!returnStatement.SpanOrLeadingTriviaContainsDirectives())
                                                {
                                                    ExpressionSyntax expression = returnStatement.Expression;
                                                    if (expression?.IsKind(SyntaxKind.IdentifierName) == true)
                                                    {
                                                        var identifierName = (IdentifierNameSyntax)expression;

                                                        if (string.Equals(localDeclaration.IdentifierText, identifierName.Identifier.ValueText, StringComparison.Ordinal))
                                                            ReportDiagnostic(context, localDeclaration, expression);
                                                    }
                                                }

                                                break;
                                            }
                                        case SyntaxKind.ForEachStatement:
                                            {
                                                if (value.IsSingleLine()
                                                    && !value.IsKind(SyntaxKind.ArrayInitializerExpression))
                                                {
                                                    var forEachStatement = (ForEachStatementSyntax)nextStatement;
                                                    Analyze(context, statements, localDeclaration, forEachStatement.Expression);
                                                }

                                                break;
                                            }
                                        case SyntaxKind.SwitchStatement:
                                            {
                                                if (value.IsSingleLine())
                                                {
                                                    var switchStatement = (SwitchStatementSyntax)nextStatement;
                                                    Analyze(context, statements, localDeclaration, switchStatement.Expression);
                                                }

                                                break;
                                            }
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
            SingleLocalDeclarationStatement localDeclaration,
            int index,
            ExpressionStatementSyntax expressionStatement)
        {
            ExpressionSyntax expression = expressionStatement.Expression;

            if (expression?.IsKind(SyntaxKind.SimpleAssignmentExpression) == true)
            {
                var assignment = (AssignmentExpressionSyntax)expression;

                ExpressionSyntax left = assignment.Left;
                ExpressionSyntax right = assignment.Right;

                if (left?.IsMissing == false
                    && right?.IsKind(SyntaxKind.IdentifierName) == true)
                {
                    var identifierName = (IdentifierNameSyntax)right;

                    string name = localDeclaration.IdentifierText;

                    if (string.Equals(name, identifierName.Identifier.ValueText, StringComparison.Ordinal))
                    {
                        VariableDeclaratorSyntax declarator = localDeclaration.Declarator;

                        ISymbol localSymbol = context.SemanticModel.GetDeclaredSymbol(declarator, context.CancellationToken);

                        if (localSymbol?.IsErrorType() == false)
                        {
                            bool isReferenced = IsLocalVariableReferenced(localSymbol, name, left, left.Span, context.SemanticModel, context.CancellationToken);

                            if (!isReferenced
                                && index < statements.Count - 2)
                            {
                                TextSpan span = TextSpan.FromBounds(statements[index + 2].SpanStart, statements.Last().Span.End);

                                isReferenced = IsLocalVariableReferenced(localSymbol, name, localDeclaration.Statement.Parent, span, context.SemanticModel, context.CancellationToken);
                            }

                            if (!isReferenced
                                && !expressionStatement.SpanOrLeadingTriviaContainsDirectives())
                            {
                                ReportDiagnostic(context, localDeclaration, right);
                            }
                        }
                    }
                }
            }
        }

        private static void Analyze(
            SyntaxNodeAnalysisContext context,
            SyntaxList<StatementSyntax> statements,
            SingleLocalDeclarationStatement localDeclaration,
            int index,
            LocalDeclarationStatementSyntax localDeclaration2)
        {
            ExpressionSyntax value2 = localDeclaration2
                .Declaration?
                .Variables
                .SingleOrDefault(throwException: false)?
                .Initializer?
                .Value;

            if (value2?.IsKind(SyntaxKind.IdentifierName) == true)
            {
                var identifierName = (IdentifierNameSyntax)value2;

                string name = localDeclaration.IdentifierText;

                if (string.Equals(name, identifierName.Identifier.ValueText, StringComparison.Ordinal))
                {
                    bool isReferenced = false;

                    if (index < statements.Count - 2)
                    {
                        TextSpan span = TextSpan.FromBounds(statements[index + 2].SpanStart, statements.Last().Span.End);

                        isReferenced = IsLocalVariableReferenced(localDeclaration.Declarator, name, localDeclaration.Statement.Parent, span, context.SemanticModel, context.CancellationToken);
                    }

                    if (!isReferenced
                        && !localDeclaration2.SpanOrLeadingTriviaContainsDirectives())
                    {
                        ReportDiagnostic(context, localDeclaration, value2);
                    }
                }
            }
        }

        private static void Analyze(
            SyntaxNodeAnalysisContext context,
            SyntaxList<StatementSyntax> statements,
            SingleLocalDeclarationStatement localDeclaration,
            ExpressionSyntax expression)
        {
            if (expression?.IsKind(SyntaxKind.IdentifierName) == true)
            {
                var identifierName = (IdentifierNameSyntax)expression;

                string name = localDeclaration.IdentifierText;

                if (string.Equals(name, identifierName.Identifier.ValueText, StringComparison.Ordinal))
                {
                    TextSpan span = TextSpan.FromBounds(expression.Span.End, statements.Last().Span.End);

                    SyntaxNode parent = localDeclaration.Statement.Parent;

                    if (!IsLocalVariableReferenced(localDeclaration.Declarator, name, parent, span, context.SemanticModel, context.CancellationToken)
                        && !parent.ContainsDirectives(TextSpan.FromBounds(localDeclaration.Statement.SpanStart, expression.Span.End)))
                    {
                        ReportDiagnostic(context, localDeclaration, expression);
                    }
                }
            }
        }

        private static void ReportDiagnostic(
            SyntaxNodeAnalysisContext context,
            SingleLocalDeclarationStatement localDeclaration,
            ExpressionSyntax expression)
        {
            context.ReportDiagnostic(DiagnosticDescriptors.InlineLocalVariable, localDeclaration.Statement);

            foreach (SyntaxToken modifier in localDeclaration.Modifiers)
                context.ReportToken(DiagnosticDescriptors.InlineLocalVariableFadeOut, modifier);

            context.ReportNode(DiagnosticDescriptors.InlineLocalVariableFadeOut, localDeclaration.Type);
            context.ReportToken(DiagnosticDescriptors.InlineLocalVariableFadeOut, localDeclaration.Identifier);
            context.ReportToken(DiagnosticDescriptors.InlineLocalVariableFadeOut, localDeclaration.EqualsToken);
            context.ReportToken(DiagnosticDescriptors.InlineLocalVariableFadeOut, localDeclaration.SemicolonToken);
            context.ReportNode(DiagnosticDescriptors.InlineLocalVariableFadeOut, expression);
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
            foreach (SyntaxNode descendant in node.DescendantNodesAndSelf(span))
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
            StatementContainer container = StatementContainer.Create(localDeclaration);

            int index = container.Statements.IndexOf(localDeclaration);

            StatementSyntax nextStatement = container.Statements[index + 1];

            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            ExpressionSyntax value = GetExpressionToInline(localDeclaration, semanticModel, cancellationToken);

            StatementSyntax newStatement = GetStatementWithInlinedExpression(nextStatement, value);

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

            SyntaxList<StatementSyntax> newStatements = container.Statements
                .Replace(nextStatement, newStatement)
                .RemoveAt(index);

            return await document.ReplaceNodeAsync(container.Node, container.NodeWithStatements(newStatements), cancellationToken).ConfigureAwait(false);
        }

        private static ExpressionSyntax GetExpressionToInline(LocalDeclarationStatementSyntax localDeclaration, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            VariableDeclarationSyntax variableDeclaration = localDeclaration.Declaration;

            ExpressionSyntax expression = variableDeclaration
                .Variables
                .First()
                .Initializer
                .Value;

            if (expression.IsKind(SyntaxKind.ArrayInitializerExpression))
            {
                expression = SyntaxFactory.ArrayCreationExpression(
                    (ArrayTypeSyntax)variableDeclaration.Type.WithoutTrivia(),
                    (InitializerExpressionSyntax)expression);

                return expression.WithFormatterAnnotation();
            }
            else
            {
                expression = expression.Parenthesize();

                ITypeSymbol typeSymbol = semanticModel.GetTypeSymbol(variableDeclaration.Type, cancellationToken);

                if (typeSymbol.SupportsExplicitDeclaration())
                {
                    TypeSyntax type = typeSymbol.ToMinimalTypeSyntax(semanticModel, localDeclaration.SpanStart);

                    expression = SyntaxFactory.CastExpression(type, expression).WithSimplifierAnnotation();
                }

                return expression;
            }
        }

        private static StatementSyntax GetStatementWithInlinedExpression(StatementSyntax statement, ExpressionSyntax expression)
        {
            switch (statement.Kind())
            {
                case SyntaxKind.ExpressionStatement:
                    {
                        var expressionStatement = (ExpressionStatementSyntax)statement;

                        var assignment = (AssignmentExpressionSyntax)expressionStatement.Expression;

                        AssignmentExpressionSyntax newAssignment = assignment.WithRight(expression.WithTriviaFrom(assignment.Right));

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

                        return statement.ReplaceNode(value, expression.WithTriviaFrom(value));
                    }
                case SyntaxKind.ReturnStatement:
                    {
                        var returnStatement = (ReturnStatementSyntax)statement;

                        return returnStatement.WithExpression(expression.WithTriviaFrom(returnStatement.Expression));
                    }
                case SyntaxKind.ForEachStatement:
                    {
                        var forEachStatement = (ForEachStatementSyntax)statement;

                        return forEachStatement.WithExpression(expression.WithTriviaFrom(forEachStatement.Expression));
                    }
                case SyntaxKind.SwitchStatement:
                    {
                        var switchStatement = (SwitchStatementSyntax)statement;

                        return switchStatement.WithExpression(expression.WithTriviaFrom(switchStatement.Expression));
                    }
                default:
                    {
                        Debug.Fail("");
                        return statement;
                    }
            }
        }
    }
}
