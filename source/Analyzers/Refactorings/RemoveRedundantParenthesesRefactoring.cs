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

namespace Roslynator.CSharp.Refactorings
{
    internal static class RemoveRedundantParenthesesRefactoring
    {
        public static void AnalyzeParenthesizedExpression(SyntaxNodeAnalysisContext context)
        {
            var parenthesizedExpression = (ParenthesizedExpressionSyntax)context.Node;

            AnalyzeExpression(context, parenthesizedExpression.Expression);
        }

        public static void AnalyzeWhileStatement(SyntaxNodeAnalysisContext context)
        {
            var whileStatement = (WhileStatementSyntax)context.Node;

            AnalyzeExpression(context, whileStatement.Condition);
        }

        public static void AnalyzeDoStatement(SyntaxNodeAnalysisContext context)
        {
            var doStatement = (DoStatementSyntax)context.Node;

            AnalyzeExpression(context, doStatement.Condition);
        }

        public static void AnalyzeUsingStatement(SyntaxNodeAnalysisContext context)
        {
            var usingStatement = (UsingStatementSyntax)context.Node;

            AnalyzeExpression(context, usingStatement.Expression);
        }

        public static void AnalyzeLockStatement(SyntaxNodeAnalysisContext context)
        {
            var lockStatement = (LockStatementSyntax)context.Node;

            AnalyzeExpression(context, lockStatement.Expression);
        }

        public static void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            var ifStatement = (IfStatementSyntax)context.Node;

            AnalyzeExpression(context, ifStatement.Condition);
        }

        public static void AnalyzeSwitchStatement(SyntaxNodeAnalysisContext context)
        {
            var switchStatement = (SwitchStatementSyntax)context.Node;

            AnalyzeExpression(context, switchStatement.Expression);
        }

        internal static void AnalyzeCommonForEachStatement(SyntaxNodeAnalysisContext context)
        {
            var forEachStatement = (CommonForEachStatementSyntax)context.Node;

            AnalyzeExpression(context, forEachStatement.Expression);
        }

        public static void AnalyzeReturnStatement(SyntaxNodeAnalysisContext context)
        {
            var returnStatement = (ReturnStatementSyntax)context.Node;

            AnalyzeExpression(context, returnStatement.Expression);
        }

        public static void AnalyzeYieldStatement(SyntaxNodeAnalysisContext context)
        {
            var yieldStatement = (YieldStatementSyntax)context.Node;

            AnalyzeExpression(context, yieldStatement.Expression);
        }

        public static void AnalyzeExpressionStatement(SyntaxNodeAnalysisContext context)
        {
            var expressionStatement = (ExpressionStatementSyntax)context.Node;

            AnalyzeExpression(context, expressionStatement.Expression);
        }

        public static void AnalyzeArgument(SyntaxNodeAnalysisContext context)
        {
            var argument = (ArgumentSyntax)context.Node;

            AnalyzeExpression(context, argument.Expression);
        }

        public static void AnalyzeAttributeArgument(SyntaxNodeAnalysisContext context)
        {
            var attributeArgument = (AttributeArgumentSyntax)context.Node;

            AnalyzeExpression(context, attributeArgument.Expression);
        }

        public static void AnalyzeEqualsValueClause(SyntaxNodeAnalysisContext context)
        {
            var equalsValueClause = (EqualsValueClauseSyntax)context.Node;

            AnalyzeExpression(context, equalsValueClause.Value);
        }

        public static void AnalyzeAwaitExpression(SyntaxNodeAnalysisContext context)
        {
            var awaitExpression = (AwaitExpressionSyntax)context.Node;

            AnalyzeExpression(context, awaitExpression.Expression);
        }

        internal static void AnalyzeInitializerExpression(SyntaxNodeAnalysisContext context)
        {
            var initializerExpression = (InitializerExpressionSyntax)context.Node;

            foreach (ExpressionSyntax expression in initializerExpression.Expressions)
                AnalyzeExpression(context, expression);
        }

        internal static void AnalyzeInterpolation(SyntaxNodeAnalysisContext context)
        {
            var interpolation = (InterpolationSyntax)context.Node;

            ExpressionSyntax expression = interpolation.Expression;

            if (expression?.IsKind(SyntaxKind.ParenthesizedExpression) == true)
            {
                var parenthesizedExpression = (ParenthesizedExpressionSyntax)expression;

                if (parenthesizedExpression.Expression?.IsKind(SyntaxKind.ConditionalExpression) == false)
                    AnalyzeParenthesizedExpression(context, parenthesizedExpression);
            }
        }

        internal static void AnalyzeArrowExpressionClause(SyntaxNodeAnalysisContext context)
        {
            var arrowExpressionClause = (ArrowExpressionClauseSyntax)context.Node;

            AnalyzeExpression(context, arrowExpressionClause.Expression);
        }

        public static void AnalyzeAssignmentExpression(SyntaxNodeAnalysisContext context)
        {
            var assignment = (AssignmentExpressionSyntax)context.Node;

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
