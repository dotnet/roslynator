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

            ExpressionSyntax expression = parenthesizedExpression.Expression;

            if (expression?.IsMissing != false)
                return;

            SyntaxKind kind = expression.Kind();

            if (kind == SyntaxKind.ParenthesizedExpression)
            {
                AnalyzeParenthesizedExpression(context, (ParenthesizedExpressionSyntax)expression);
            }
            else if (parenthesizedExpression.IsParentKind(SyntaxKind.LogicalNotExpression)
                && kind.Is(
                    SyntaxKind.IdentifierName,
                    SyntaxKind.GenericName,
                    SyntaxKind.InvocationExpression,
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxKind.ElementAccessExpression,
                    SyntaxKind.ConditionalAccessExpression))
            {
                AnalyzeParenthesizedExpression(context, parenthesizedExpression);
            }
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

        public static void AnalyzeAwaitExpression(SyntaxNodeAnalysisContext context)
        {
            var awaitExpression = (AwaitExpressionSyntax)context.Node;

            if (!(awaitExpression.Expression is ParenthesizedExpressionSyntax parenthesizedExpression))
                return;

            ExpressionSyntax expression = parenthesizedExpression.Expression;

            if (expression?.IsMissing != false)
                return;

            if (OperatorPrecedence.GetPrecedence(expression.Kind()) > OperatorPrecedence.GetPrecedence(SyntaxKind.AwaitExpression))
                return;

            AnalyzeParenthesizedExpression(context, parenthesizedExpression);
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

            if (!(interpolation.Expression is ParenthesizedExpressionSyntax parenthesizedExpression))
                return;

            if (parenthesizedExpression.Expression?.IsKind(SyntaxKind.ConditionalExpression) == true)
                return;

            AnalyzeParenthesizedExpression(context, parenthesizedExpression);
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
        }

        private static void AnalyzeExpression(SyntaxNodeAnalysisContext context, ExpressionSyntax expression)
        {
            if (expression is ParenthesizedExpressionSyntax parenthesizedExpression)
                AnalyzeParenthesizedExpression(context, parenthesizedExpression);
        }

        private static void AnalyzeParenthesizedExpression(SyntaxNodeAnalysisContext context, ParenthesizedExpressionSyntax parenthesizedExpression)
        {
            SyntaxToken openParen = parenthesizedExpression.OpenParenToken;

            if (openParen.IsMissing)
                return;

            SyntaxToken closeParen = parenthesizedExpression.CloseParenToken;

            if (closeParen.IsMissing)
                return;

            context.ReportDiagnostic(
               DiagnosticDescriptors.RemoveRedundantParentheses,
               openParen.GetLocation(),
               additionalLocations: ImmutableArray.Create(closeParen.GetLocation()));

            context.ReportToken(DiagnosticDescriptors.RemoveRedundantParenthesesFadeOut, openParen);
            context.ReportToken(DiagnosticDescriptors.RemoveRedundantParenthesesFadeOut, closeParen);
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

            ExpressionSyntax newNode = expression.WithLeadingTrivia(leading).WithTrailingTrivia(trailing);

            return document.ReplaceNodeAsync(parenthesizedExpression, newNode, cancellationToken);
        }
    }
}
