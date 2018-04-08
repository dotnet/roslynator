// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RemoveRedundantParenthesesAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.RemoveRedundantParentheses,
                    DiagnosticDescriptors.RemoveRedundantParenthesesFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeParenthesizedExpression, SyntaxKind.ParenthesizedExpression);

            context.RegisterSyntaxNodeAction(AnalyzeWhileStatement, SyntaxKind.WhileStatement);
            context.RegisterSyntaxNodeAction(AnalyzeDoStatement, SyntaxKind.DoStatement);
            context.RegisterSyntaxNodeAction(AnalyzeUsingStatement, SyntaxKind.UsingStatement);
            context.RegisterSyntaxNodeAction(AnalyzeLockStatement, SyntaxKind.LockStatement);
            context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
            context.RegisterSyntaxNodeAction(AnalyzeSwitchStatement, SyntaxKind.SwitchStatement);

            context.RegisterSyntaxNodeAction(AnalyzeReturnStatement, SyntaxKind.ReturnStatement);
            context.RegisterSyntaxNodeAction(AnalyzeYieldStatement, SyntaxKind.YieldReturnStatement);
            context.RegisterSyntaxNodeAction(AnalyzeExpressionStatement, SyntaxKind.ExpressionStatement);
            context.RegisterSyntaxNodeAction(AnalyzeArgument, SyntaxKind.Argument);
            context.RegisterSyntaxNodeAction(AnalyzeAttributeArgument, SyntaxKind.AttributeArgument);
            context.RegisterSyntaxNodeAction(AnalyzeAwaitExpression, SyntaxKind.AwaitExpression);
            context.RegisterSyntaxNodeAction(AnalyzeArrowExpressionClause, SyntaxKind.ArrowExpressionClause);
            context.RegisterSyntaxNodeAction(AnalyzeInterpolation, SyntaxKind.Interpolation);
            context.RegisterSyntaxNodeAction(AnalyzeInitializerExpression, SyntaxKind.ArrayInitializerExpression);
            context.RegisterSyntaxNodeAction(AnalyzeInitializerExpression, SyntaxKind.CollectionInitializerExpression);

            context.RegisterSyntaxNodeAction(AnalyzeAssignmentExpression, SyntaxKind.SimpleAssignmentExpression);
            context.RegisterSyntaxNodeAction(AnalyzeAssignmentExpression, SyntaxKind.AddAssignmentExpression);
            context.RegisterSyntaxNodeAction(AnalyzeAssignmentExpression, SyntaxKind.SubtractAssignmentExpression);
            context.RegisterSyntaxNodeAction(AnalyzeAssignmentExpression, SyntaxKind.MultiplyAssignmentExpression);
            context.RegisterSyntaxNodeAction(AnalyzeAssignmentExpression, SyntaxKind.DivideAssignmentExpression);
            context.RegisterSyntaxNodeAction(AnalyzeAssignmentExpression, SyntaxKind.ModuloAssignmentExpression);
            context.RegisterSyntaxNodeAction(AnalyzeAssignmentExpression, SyntaxKind.AndAssignmentExpression);
            context.RegisterSyntaxNodeAction(AnalyzeAssignmentExpression, SyntaxKind.ExclusiveOrAssignmentExpression);
            context.RegisterSyntaxNodeAction(AnalyzeAssignmentExpression, SyntaxKind.OrAssignmentExpression);
            context.RegisterSyntaxNodeAction(AnalyzeAssignmentExpression, SyntaxKind.LeftShiftAssignmentExpression);
            context.RegisterSyntaxNodeAction(AnalyzeAssignmentExpression, SyntaxKind.RightShiftAssignmentExpression);
        }

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

            if (CSharpFacts.GetOperatorPrecedence(expression.Kind()) > CSharpFacts.GetOperatorPrecedence(SyntaxKind.AwaitExpression))
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

            if (parenthesizedExpression.Expression?.Kind() == SyntaxKind.ConditionalExpression)
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
    }
}
