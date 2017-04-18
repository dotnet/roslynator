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
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Roslynator.CSharp.Refactorings
{
    internal static class UseCoalesceExpressionRefactoring
    {
        public static void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            var ifStatement = (IfStatementSyntax)context.Node;

            if (ifStatement.IsSimpleIf()
                && !ifStatement.ContainsDiagnostics
                && !IsPartOfLazyInitialization(ifStatement))
            {
                EqualsToNullExpression equalsToNull;
                if (EqualsToNullExpression.TryCreate(ifStatement.Condition, out equalsToNull))
                {
                    SimpleAssignmentStatement assignment;
                    if (SimpleAssignmentStatement.TryCreate(ifStatement.GetSingleStatementOrDefault(), out assignment)
                        && assignment.Left.IsEquivalentTo(equalsToNull.Left, topLevel: false)
                        && assignment.Right.IsSingleLine()
                        && !ifStatement.SpanContainsDirectives())
                    {
                        StatementSyntax fixableStatement = ifStatement;

                        SyntaxList<StatementSyntax> statements;
                        if (ifStatement.TryGetContainingList(out statements))
                        {
                            int index = statements.IndexOf(ifStatement);

                            if (index > 0)
                            {
                                StatementSyntax previousStatement = statements[index - 1];

                                if (!previousStatement.ContainsDiagnostics
                                    && CanRefactor(previousStatement, ifStatement, equalsToNull.Left, ifStatement.Parent))
                                {
                                    fixableStatement = previousStatement;
                                }
                            }
                        }

                        context.ReportDiagnostic(DiagnosticDescriptors.UseCoalesceExpression, fixableStatement);
                    }
                }
            }
        }

        private static bool IsPartOfLazyInitialization(IfStatementSyntax ifStatement)
        {
            SyntaxList<StatementSyntax> statements;
            return ifStatement.TryGetContainingList(out statements)
                && statements.Count == 2
                && statements.IndexOf(ifStatement) == 0
                && statements[1].IsKind(SyntaxKind.ReturnStatement);
        }

        private static bool CanRefactor(
            StatementSyntax statement,
            IfStatementSyntax ifStatement,
            ExpressionSyntax expression,
            SyntaxNode parent)
        {
            switch (statement.Kind())
            {
                case SyntaxKind.LocalDeclarationStatement:
                    return CanRefactor((LocalDeclarationStatementSyntax)statement, ifStatement, expression, parent);
                case SyntaxKind.ExpressionStatement:
                    return CanRefactor((ExpressionStatementSyntax)statement, ifStatement, expression, parent);
                default:
                    return false;
            }
        }

        private static bool CanRefactor(
            LocalDeclarationStatementSyntax localDeclarationStatement,
            IfStatementSyntax ifStatement,
            ExpressionSyntax expression,
            SyntaxNode parent)
        {
            VariableDeclaratorSyntax declarator = localDeclarationStatement.Declaration?.SingleVariableOrDefault();

            if (declarator != null)
            {
                ExpressionSyntax value = declarator.Initializer?.Value;

                return value != null
                    && expression.IsKind(SyntaxKind.IdentifierName)
                    && string.Equals(declarator.Identifier.ValueText, ((IdentifierNameSyntax)expression).Identifier.ValueText, StringComparison.Ordinal)
                    && !parent.ContainsDirectives(TextSpan.FromBounds(value.Span.End, ifStatement.Span.Start));
            }

            return false;
        }

        private static bool CanRefactor(
            ExpressionStatementSyntax expressionStatement,
            IfStatementSyntax ifStatement,
            ExpressionSyntax expression,
            SyntaxNode parent)
        {
            ExpressionSyntax expression2 = expressionStatement.Expression;

            if (expression2?.IsKind(SyntaxKind.SimpleAssignmentExpression) == true)
            {
                var assignment = (AssignmentExpressionSyntax)expression2;

                ExpressionSyntax left = assignment.Left;

                if (left?.IsMissing == false)
                {
                    ExpressionSyntax right = assignment.Right;

                    return right?.IsMissing == false
                        && expression.IsEquivalentTo(left, topLevel: false)
                        && !parent.ContainsDirectives(TextSpan.FromBounds(right.Span.End, ifStatement.Span.Start));
                }
            }

            return false;
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken)
        {
            StatementContainer container;
            if (StatementContainer.TryCreate(statement, out container))
            {
                SyntaxList<StatementSyntax> statements = container.Statements;

                int index = statements.IndexOf(statement);

                switch (statement.Kind())
                {
                    case SyntaxKind.IfStatement:
                        {
                            var ifStatement = (IfStatementSyntax)statement;

                            var expressionStatement = (ExpressionStatementSyntax)ifStatement.GetSingleStatementOrDefault();

                            var assignment = (AssignmentExpressionSyntax)expressionStatement.Expression;

                            ExpressionSyntax left = assignment.Left;
                            ExpressionSyntax right = assignment.Right;

                            ParenthesizedExpressionSyntax newLeft = left
                                .WithoutLeadingTrivia()
                                .WithTrailingTrivia(Space)
                                .Parenthesize(moveTrivia: true)
                                .WithSimplifierAnnotation();

                            ParenthesizedExpressionSyntax newRight = right
                                    .WithLeadingTrivia(Space)
                                    .Parenthesize(moveTrivia: true)
                                    .WithSimplifierAnnotation();

                            BinaryExpressionSyntax coalesceExpression = CSharpFactory.CoalesceExpression(newLeft, newRight);

                            AssignmentExpressionSyntax newAssignment = assignment.WithRight(coalesceExpression.WithTriviaFrom(right));

                            ExpressionStatementSyntax newNode = expressionStatement.WithExpression(newAssignment);

                            IEnumerable<SyntaxTrivia> trivia = ifStatement.DescendantTrivia(TextSpan.FromBounds(ifStatement.SpanStart, expressionStatement.SpanStart));

                            if (trivia.All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                            {
                                newNode = newNode.WithLeadingTrivia(ifStatement.GetLeadingTrivia());
                            }
                            else
                            {
                                newNode = newNode
                                    .WithLeadingTrivia(ifStatement.GetLeadingTrivia().Concat(trivia))
                                    .WithFormatterAnnotation();
                            }

                            return await document.ReplaceNodeAsync(ifStatement, newNode, cancellationToken).ConfigureAwait(false);
                        }
                    case SyntaxKind.ExpressionStatement:
                        {
                            var expressionStatement = (ExpressionStatementSyntax)statement;

                            var assignment = (AssignmentExpressionSyntax)expressionStatement.Expression;

                            return await RefactorAsync(document, expressionStatement, (IfStatementSyntax)statements[index + 1], index, container, assignment.Right, cancellationToken).ConfigureAwait(false);
                        }
                    case SyntaxKind.LocalDeclarationStatement:
                        {
                            var localDeclaration = (LocalDeclarationStatementSyntax)statement;

                            ExpressionSyntax value = localDeclaration
                                .Declaration
                                .Variables
                                .First()
                                .Initializer
                                .Value;

                            return await RefactorAsync(document, localDeclaration, (IfStatementSyntax)statements[index + 1], index, container, value, cancellationToken).ConfigureAwait(false);
                        }
                }
            }

            Debug.Assert(false, statement.Kind().ToString());

            return document;
        }

        private static Task<Document> RefactorAsync(
            Document document,
            StatementSyntax statement,
            IfStatementSyntax ifStatement,
            int statementIndex,
            StatementContainer container,
            ExpressionSyntax expression,
            CancellationToken cancellationToken)
        {
            var expressionStatement = (ExpressionStatementSyntax)ifStatement.GetSingleStatementOrDefault();

            var assignment = (AssignmentExpressionSyntax)expressionStatement.Expression;

            ExpressionSyntax left = expression
                .WithoutTrailingTrivia()
                .Parenthesize(moveTrivia: true)
                .WithSimplifierAnnotation();

            ExpressionSyntax right = assignment.Right
                .WithTrailingTrivia(expression.GetTrailingTrivia())
                .Parenthesize(moveTrivia: true)
                .WithSimplifierAnnotation();

            BinaryExpressionSyntax newNode = CSharpFactory.CoalesceExpression(left, right);

            StatementSyntax newStatement = statement.ReplaceNode(expression, newNode);

            IEnumerable<SyntaxTrivia> trivia = container.Node.DescendantTrivia(TextSpan.FromBounds(statement.Span.End, ifStatement.Span.End));

            if (!trivia.All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                newStatement = newStatement.WithTrailingTrivia(trivia);
                newStatement = newStatement.AppendToTrailingTrivia(ifStatement.GetTrailingTrivia());
            }
            else
            {
                newStatement = newStatement.WithTrailingTrivia(ifStatement.GetTrailingTrivia());
            }

            SyntaxList<StatementSyntax> newStatements = container.Statements
                .Remove(ifStatement)
                .ReplaceAt(statementIndex, newStatement);

            return document.ReplaceNodeAsync(container.Node, container.NodeWithStatements(newStatements), cancellationToken);
        }
    }
}
