// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.Diagnostics.Extensions;
using Roslynator.Extensions;

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveRedundantParenthesesRefactoring
    {
        public static void Analyze(SyntaxNodeAnalysisContext context, ParenthesizedExpressionSyntax parenthesizedExpression)
        {
            AnalyzeExpression(context, parenthesizedExpression.Expression);
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, WhileStatementSyntax whileStatement)
        {
            AnalyzeExpression(context, whileStatement.Condition);
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, DoStatementSyntax doStatement)
        {
            AnalyzeExpression(context, doStatement.Condition);
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, UsingStatementSyntax usingStatement)
        {
            AnalyzeExpression(context, usingStatement.Expression);
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, LockStatementSyntax lockStatement)
        {
            AnalyzeExpression(context, lockStatement.Expression);
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, IfStatementSyntax ifStatement)
        {
            AnalyzeExpression(context, ifStatement.Condition);
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, SwitchStatementSyntax switchStatement)
        {
            AnalyzeExpression(context, switchStatement.Expression);
        }

        internal static void Analyze(SyntaxNodeAnalysisContext context, CommonForEachStatementSyntax forEachStatement)
        {
            AnalyzeExpression(context, forEachStatement.Expression);
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, ReturnStatementSyntax returnStatement)
        {
            AnalyzeExpression(context, returnStatement.Expression);
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, YieldStatementSyntax yieldStatement)
        {
            AnalyzeExpression(context, yieldStatement.Expression);
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, ExpressionStatementSyntax expressionStatement)
        {
            AnalyzeExpression(context, expressionStatement.Expression);
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, ArgumentSyntax argument)
        {
            AnalyzeExpression(context, argument.Expression);
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, AttributeArgumentSyntax attributeArgument)
        {
            AnalyzeExpression(context, attributeArgument.Expression);
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, EqualsValueClauseSyntax equalsValueClause)
        {
            AnalyzeExpression(context, equalsValueClause.Value);
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, AwaitExpressionSyntax awaitExpression)
        {
            AnalyzeExpression(context, awaitExpression.Expression);
        }

        internal static void Analyze(SyntaxNodeAnalysisContext context, InitializerExpressionSyntax initializerExpression)
        {
            foreach (ExpressionSyntax expression in initializerExpression.Expressions)
                AnalyzeExpression(context, expression);
        }

        internal static void Analyze(SyntaxNodeAnalysisContext context, InterpolationSyntax interpolation)
        {
            ExpressionSyntax expression = interpolation.Expression;

            if (expression?.IsKind(SyntaxKind.ParenthesizedExpression) == true)
            {
                var parenthesizedExpression = (ParenthesizedExpressionSyntax)expression;

                if (parenthesizedExpression.Expression?.IsKind(SyntaxKind.ConditionalExpression) == false)
                    AnalyzeParenthesizedExpression(context, parenthesizedExpression);
            }
        }

        internal static void Analyze(SyntaxNodeAnalysisContext context, ArrowExpressionClauseSyntax arrowExpressionClause)
        {
            AnalyzeExpression(context, arrowExpressionClause.Expression);
        }

        public static void Analyze(SyntaxNodeAnalysisContext context, AssignmentExpressionSyntax assignment)
        {
            AnalyzeExpression(context, assignment.Left);
            AnalyzeExpression(context, assignment.Right);
        }

        private static void AnalyzeExpression(SyntaxNodeAnalysisContext context, ExpressionSyntax expression)
        {
            if (expression?.IsKind(SyntaxKind.ParenthesizedExpression) == true)
                AnalyzeParenthesizedExpression(context, (ParenthesizedExpressionSyntax)expression);
        }

        private static void AnalyzeParenthesizedExpression(SyntaxNodeAnalysisContext context, ParenthesizedExpressionSyntax parenthesizedExpression)
        {
            SyntaxToken openParen = parenthesizedExpression.OpenParenToken;

            if (!openParen.IsMissing)
            {
                SyntaxToken closeParen = parenthesizedExpression.CloseParenToken;

                if (!closeParen.IsMissing)
                {
                    context.ReportDiagnostic(
                       DiagnosticDescriptors.RemoveRedundantParentheses,
                       openParen.GetLocation(),
                       additionalLocations: ImmutableArray.Create(closeParen.GetLocation()));

                    context.ReportToken(DiagnosticDescriptors.RemoveRedundantParenthesesFadeOut, openParen);
                    context.ReportToken(DiagnosticDescriptors.RemoveRedundantParenthesesFadeOut, closeParen);
                }
            }
        }

        public static Task<Document> RefactorAsync(
            Document document,
            ParenthesizedExpressionSyntax parenthesizedExpression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ExpressionSyntax expression = parenthesizedExpression.Expression;

            IEnumerable<SyntaxTrivia> leading = parenthesizedExpression.GetLeadingTrivia()
                .Concat(parenthesizedExpression.OpenParenToken.TrailingTrivia)
                .Concat(expression.GetLeadingTrivia());

            IEnumerable<SyntaxTrivia> trailing = expression.GetTrailingTrivia()
                .Concat(parenthesizedExpression.CloseParenToken.LeadingTrivia)
                .Concat(parenthesizedExpression.GetTrailingTrivia());

            return document.ReplaceNodeAsync(
                parenthesizedExpression,
                expression.WithLeadingTrivia(leading).WithTrailingTrivia(trailing),
                cancellationToken);
        }
    }
}
