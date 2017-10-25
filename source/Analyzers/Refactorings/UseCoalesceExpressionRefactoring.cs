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
                && !ifStatement.ContainsDiagnostics)
            {
                SyntaxList<StatementSyntax> statements;
                if (ifStatement.TryGetContainingList(out statements)
                    && !IsPartOfLazyInitialization(ifStatement, statements))
                {
                    NullCheckExpressionInfo nullCheck = SyntaxInfo.NullCheckExpressionInfo(ifStatement.Condition, semanticModel: context.SemanticModel, cancellationToken: context.CancellationToken);
                    if (nullCheck.Success)
                    {
                        SimpleAssignmentStatementInfo assignmentInfo = SyntaxInfo.SimpleAssignmentStatementInfo(ifStatement.GetSingleStatementOrDefault());
                        if (assignmentInfo.Success
                            && SyntaxComparer.AreEquivalent(assignmentInfo.Left, nullCheck.Expression)
                            && assignmentInfo.Right.IsSingleLine()
                            && !ifStatement.SpanContainsDirectives())
                        {
                            int index = statements.IndexOf(ifStatement);

                            if (index > 0)
                            {
                                StatementSyntax previousStatement = statements[index - 1];

                                if (!previousStatement.ContainsDiagnostics
                                    && CanRefactor(previousStatement, ifStatement, nullCheck.Expression, ifStatement.Parent))
                                {
                                    context.ReportDiagnostic(DiagnosticDescriptors.UseCoalesceExpression, previousStatement);
                                }
                            }

                            if (index < statements.Count - 1)
                            {
                                StatementSyntax nextStatement = statements[index + 1];

                                if (!nextStatement.ContainsDiagnostics)
                                {
                                    MemberInvocationStatementInfo invocationInfo = SyntaxInfo.MemberInvocationStatementInfo(nextStatement);
                                    if (invocationInfo.Success
                                        && SyntaxComparer.AreEquivalent(nullCheck.Expression, invocationInfo.Expression)
                                        && !ifStatement.Parent.ContainsDirectives(TextSpan.FromBounds(ifStatement.SpanStart, nextStatement.Span.End)))
                                    {
                                        context.ReportDiagnostic(DiagnosticDescriptors.InlineLazyInitialization, ifStatement);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static bool IsPartOfLazyInitialization(IfStatementSyntax ifStatement, SyntaxList<StatementSyntax> statements)
        {
            return statements.Count == 2
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
            VariableDeclaratorSyntax declarator = localDeclarationStatement
                .Declaration?
                .Variables
                .SingleOrDefault(throwException: false);

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
                        && SyntaxComparer.AreEquivalent(expression, left)
                        && !parent.ContainsDirectives(TextSpan.FromBounds(right.Span.End, ifStatement.Span.Start));
                }
            }

            return false;
        }

        public static Task<Document> InlineLazyInitializationAsync(
            Document document,
            IfStatementSyntax ifStatement,
            CancellationToken cancellationToken)
        {
            StatementsInfo statementsInfo = SyntaxInfo.StatementsInfo(ifStatement);

            SyntaxList<StatementSyntax> statements = statementsInfo.Statements;

            int index = statements.IndexOf(ifStatement);

            StatementSyntax expressionStatement = (ExpressionStatementSyntax)statements[index + 1];

            MemberInvocationStatementInfo invocationInfo = SyntaxInfo.MemberInvocationStatementInfo((ExpressionStatementSyntax)expressionStatement);

            ExpressionSyntax expression = invocationInfo.Expression;

            SimpleAssignmentStatementInfo assignmentInfo = SyntaxInfo.SimpleAssignmentStatementInfo((ExpressionStatementSyntax)ifStatement.GetSingleStatementOrDefault());

            BinaryExpressionSyntax coalesceExpression = CSharpFactory.CoalesceExpression(expression.WithoutTrivia(), ParenthesizedExpression(assignmentInfo.AssignmentExpression));

            ParenthesizedExpressionSyntax newExpression = ParenthesizedExpression(coalesceExpression)
                .WithTriviaFrom(expression);

            StatementSyntax newExpressionStatement = expressionStatement.ReplaceNode(expression, newExpression);

            IEnumerable<SyntaxTrivia> trivia = statementsInfo.Node.DescendantTrivia(TextSpan.FromBounds(ifStatement.FullSpan.Start, expressionStatement.FullSpan.Start));

            if (trivia.Any(f => !f.IsWhitespaceOrEndOfLineTrivia()))
                newExpressionStatement = newExpressionStatement.PrependToLeadingTrivia(trivia);

            SyntaxList<StatementSyntax> newStatements = statements
                .Replace(expressionStatement, newExpressionStatement)
                .RemoveAt(index);

            return document.ReplaceStatementsAsync(statementsInfo, newStatements, cancellationToken);
        }

        public static async Task<Document> RefactorAsync(
            Document document,
            StatementSyntax statement,
            CancellationToken cancellationToken)
        {
            SemanticModel semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            StatementsInfo statementsInfo = SyntaxInfo.StatementsInfo(statement);

            SyntaxList<StatementSyntax> statements = statementsInfo.Statements;

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

                        BinaryExpressionSyntax coalesceExpression = RefactoringHelper.CreateCoalesceExpression(
                            semanticModel.GetTypeSymbol(left, cancellationToken),
                            left.WithoutLeadingTrivia().WithTrailingTrivia(Space),
                            right.WithLeadingTrivia(Space),
                            ifStatement.SpanStart,
                            semanticModel);

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

                        return await RefactorAsync(document, expressionStatement, (IfStatementSyntax)statements[index + 1], index, statementsInfo, assignment.Right, semanticModel, cancellationToken).ConfigureAwait(false);
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

                        return await RefactorAsync(document, localDeclaration, (IfStatementSyntax)statements[index + 1], index, statementsInfo, value, semanticModel, cancellationToken).ConfigureAwait(false);
                    }
                default:
                    {
                        Debug.Fail(statement.Kind().ToString());

                        return document;
                    }
            }
        }

        private static Task<Document> RefactorAsync(
            Document document,
            StatementSyntax statement,
            IfStatementSyntax ifStatement,
            int statementIndex,
            StatementsInfo statementsInfo,
            ExpressionSyntax expression,
            SemanticModel semanticModel,
            CancellationToken cancellationToken)
        {
            var expressionStatement = (ExpressionStatementSyntax)ifStatement.GetSingleStatementOrDefault();

            var assignment = (AssignmentExpressionSyntax)expressionStatement.Expression;

            BinaryExpressionSyntax newNode = RefactoringHelper.CreateCoalesceExpression(
                semanticModel.GetTypeSymbol(assignment.Left, cancellationToken),
                expression.WithoutTrailingTrivia(),
                assignment.Right.WithTrailingTrivia(expression.GetTrailingTrivia()),
                statement.SpanStart,
                semanticModel);

            StatementSyntax newStatement = statement.ReplaceNode(expression, newNode);

            IEnumerable<SyntaxTrivia> trivia = statementsInfo.Node.DescendantTrivia(TextSpan.FromBounds(statement.Span.End, ifStatement.Span.End));

            if (!trivia.All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                newStatement = newStatement.WithTrailingTrivia(trivia);
                newStatement = newStatement.AppendToTrailingTrivia(ifStatement.GetTrailingTrivia());
            }
            else
            {
                newStatement = newStatement.WithTrailingTrivia(ifStatement.GetTrailingTrivia());
            }

            SyntaxList<StatementSyntax> newStatements = statementsInfo.Statements
                .Remove(ifStatement)
                .ReplaceAt(statementIndex, newStatement);

            return document.ReplaceStatementsAsync(statementsInfo, newStatements, cancellationToken);
        }
    }
}
