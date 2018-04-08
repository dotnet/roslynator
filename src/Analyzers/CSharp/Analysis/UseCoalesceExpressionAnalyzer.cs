// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseCoalesceExpressionAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.UseCoalesceExpression,
                    DiagnosticDescriptors.InlineLazyInitialization);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
        }

        public static void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            var ifStatement = (IfStatementSyntax)context.Node;

            if (!ifStatement.IsSimpleIf())
                return;

            if (ifStatement.ContainsDiagnostics)
                return;

            if (ifStatement.SpanContainsDirectives())
                return;

            SyntaxList<StatementSyntax> statements = SyntaxInfo.StatementListInfo(ifStatement).Statements;

            if (!statements.Any())
                return;

            if (IsPartOfLazyInitialization(ifStatement, statements))
                return;

            NullCheckExpressionInfo nullCheck = SyntaxInfo.NullCheckExpressionInfo(ifStatement.Condition, semanticModel: context.SemanticModel, cancellationToken: context.CancellationToken);

            if (!nullCheck.Success)
                return;

            SimpleAssignmentStatementInfo simpleAssignment = SyntaxInfo.SimpleAssignmentStatementInfo(ifStatement.SingleNonBlockStatementOrDefault());

            if (!simpleAssignment.Success)
                return;

            if (!CSharpFactory.AreEquivalent(simpleAssignment.Left, nullCheck.Expression))
                return;

            if (!simpleAssignment.Right.IsSingleLine())
                return;

            int index = statements.IndexOf(ifStatement);

            if (index > 0)
            {
                StatementSyntax previousStatement = statements[index - 1];

                if (!previousStatement.ContainsDiagnostics
                    && IsFixable(previousStatement, ifStatement, nullCheck.Expression, ifStatement.Parent))
                {
                    context.ReportDiagnostic(DiagnosticDescriptors.UseCoalesceExpression, previousStatement);
                }
            }

            if (index == statements.Count - 1)
                return;

            StatementSyntax nextStatement = statements[index + 1];

            if (nextStatement.ContainsDiagnostics)
                return;

            SimpleMemberInvocationStatementInfo invocationInfo = SyntaxInfo.SimpleMemberInvocationStatementInfo(nextStatement);

            if (!invocationInfo.Success)
                return;

            if (!CSharpFactory.AreEquivalent(nullCheck.Expression, invocationInfo.Expression))
                return;

            if (ifStatement.Parent.ContainsDirectives(TextSpan.FromBounds(ifStatement.SpanStart, nextStatement.Span.End)))
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.InlineLazyInitialization, ifStatement);
        }

        private static bool IsPartOfLazyInitialization(IfStatementSyntax ifStatement, SyntaxList<StatementSyntax> statements)
        {
            return statements.Count == 2
                && statements.IndexOf(ifStatement) == 0
                && statements[1].IsKind(SyntaxKind.ReturnStatement);
        }

        private static bool IsFixable(
            StatementSyntax statement,
            IfStatementSyntax ifStatement,
            ExpressionSyntax expression,
            SyntaxNode parent)
        {
            switch (statement.Kind())
            {
                case SyntaxKind.LocalDeclarationStatement:
                    return IsFixable((LocalDeclarationStatementSyntax)statement, ifStatement, expression, parent);
                case SyntaxKind.ExpressionStatement:
                    return IsFixable((ExpressionStatementSyntax)statement, ifStatement, expression, parent);
                default:
                    return false;
            }
        }

        private static bool IsFixable(
            LocalDeclarationStatementSyntax localDeclarationStatement,
            IfStatementSyntax ifStatement,
            ExpressionSyntax expression,
            SyntaxNode parent)
        {
            VariableDeclaratorSyntax declarator = localDeclarationStatement
                .Declaration?
                .Variables
                .SingleOrDefault(shouldThrow: false);

            if (declarator != null)
            {
                ExpressionSyntax value = declarator.Initializer?.Value;

                return value != null
                    && expression.IsKind(SyntaxKind.IdentifierName)
                    && string.Equals(declarator.Identifier.ValueText, ((IdentifierNameSyntax)expression).Identifier.ValueText, StringComparison.Ordinal)
                    && !parent.ContainsDirectives(TextSpan.FromBounds(value.Span.End, ifStatement.SpanStart));
            }

            return false;
        }

        private static bool IsFixable(
            ExpressionStatementSyntax expressionStatement,
            IfStatementSyntax ifStatement,
            ExpressionSyntax expression,
            SyntaxNode parent)
        {
            ExpressionSyntax expression2 = expressionStatement.Expression;

            if (expression2?.Kind() == SyntaxKind.SimpleAssignmentExpression)
            {
                var assignment = (AssignmentExpressionSyntax)expression2;

                ExpressionSyntax left = assignment.Left;

                if (left?.IsMissing == false)
                {
                    ExpressionSyntax right = assignment.Right;

                    return right?.IsMissing == false
                        && CSharpFactory.AreEquivalent(expression, left)
                        && !parent.ContainsDirectives(TextSpan.FromBounds(right.Span.End, ifStatement.SpanStart));
                }
            }

            return false;
        }
    }
}
