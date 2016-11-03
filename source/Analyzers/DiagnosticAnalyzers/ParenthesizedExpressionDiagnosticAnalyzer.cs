// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ParenthesizedExpressionDiagnosticAnalyzer : BaseDiagnosticAnalyzer
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

            context.RegisterSyntaxNodeAction(f => AnalyzeParenthesizedExpression(f), SyntaxKind.ParenthesizedExpression);

            context.RegisterSyntaxNodeAction(f => AnalyzeWhileStatement(f), SyntaxKind.WhileStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeDoStatement(f), SyntaxKind.DoStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeUsingStatement(f), SyntaxKind.UsingStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeLockStatement(f), SyntaxKind.LockStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeIfStatement(f), SyntaxKind.IfStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeSwitchStatement(f), SyntaxKind.SwitchStatement);

            context.RegisterSyntaxNodeAction(f => AnalyzeReturnStatement(f), SyntaxKind.ReturnStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeYieldReturnStatement(f), SyntaxKind.YieldReturnStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeExpressionStatement(f), SyntaxKind.ExpressionStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeArgument(f), SyntaxKind.Argument);
            context.RegisterSyntaxNodeAction(f => AnalyzeAttributeArgument(f), SyntaxKind.AttributeArgument);
            context.RegisterSyntaxNodeAction(f => AnalyzeEqualsValueClause(f), SyntaxKind.EqualsValueClause);
            context.RegisterSyntaxNodeAction(f => AnalyzeAwaitExpression(f), SyntaxKind.AwaitExpression);

            context.RegisterSyntaxNodeAction(f => AnalyzeAssignment(f), SyntaxKind.SimpleAssignmentExpression);
            context.RegisterSyntaxNodeAction(f => AnalyzeAssignment(f), SyntaxKind.AddAssignmentExpression);
            context.RegisterSyntaxNodeAction(f => AnalyzeAssignment(f), SyntaxKind.SubtractAssignmentExpression);
            context.RegisterSyntaxNodeAction(f => AnalyzeAssignment(f), SyntaxKind.MultiplyAssignmentExpression);
            context.RegisterSyntaxNodeAction(f => AnalyzeAssignment(f), SyntaxKind.DivideAssignmentExpression);
            context.RegisterSyntaxNodeAction(f => AnalyzeAssignment(f), SyntaxKind.ModuloAssignmentExpression);
            context.RegisterSyntaxNodeAction(f => AnalyzeAssignment(f), SyntaxKind.AndAssignmentExpression);
            context.RegisterSyntaxNodeAction(f => AnalyzeAssignment(f), SyntaxKind.ExclusiveOrAssignmentExpression);
            context.RegisterSyntaxNodeAction(f => AnalyzeAssignment(f), SyntaxKind.OrAssignmentExpression);
            context.RegisterSyntaxNodeAction(f => AnalyzeAssignment(f), SyntaxKind.LeftShiftAssignmentExpression);
            context.RegisterSyntaxNodeAction(f => AnalyzeAssignment(f), SyntaxKind.RightShiftAssignmentExpression);
        }

        private void AnalyzeParenthesizedExpression(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var parenthesizedExpression = (ParenthesizedExpressionSyntax)context.Node;

            AnalyzeExpression(context, parenthesizedExpression.Expression);
        }

        private void AnalyzeWhileStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var whileStatement = (WhileStatementSyntax)context.Node;

            AnalyzeExpression(context, whileStatement.Condition);
        }

        private void AnalyzeDoStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var doStatement = (DoStatementSyntax)context.Node;

            AnalyzeExpression(context, doStatement.Condition);
        }

        private void AnalyzeUsingStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var usingStatement = (UsingStatementSyntax)context.Node;

            AnalyzeExpression(context, usingStatement.Expression);
        }

        private void AnalyzeLockStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var lockStatement = (LockStatementSyntax)context.Node;

            AnalyzeExpression(context, lockStatement.Expression);
        }

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var ifStatement = (IfStatementSyntax)context.Node;

            AnalyzeExpression(context, ifStatement.Condition);
        }

        private void AnalyzeSwitchStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var switchStatement = (SwitchStatementSyntax)context.Node;

            AnalyzeExpression(context, switchStatement.Expression);
        }

        private void AnalyzeReturnStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var returnStatement = (ReturnStatementSyntax)context.Node;

            AnalyzeExpression(context, returnStatement.Expression);
        }

        private void AnalyzeYieldReturnStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var yieldStatement = (YieldStatementSyntax)context.Node;

            AnalyzeExpression(context, yieldStatement.Expression);
        }

        private void AnalyzeExpressionStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var expressionStatement = (ExpressionStatementSyntax)context.Node;

            AnalyzeExpression(context, expressionStatement.Expression);
        }

        private void AnalyzeArgument(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var argument = (ArgumentSyntax)context.Node;

            AnalyzeExpression(context, argument.Expression);
        }

        private void AnalyzeAttributeArgument(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var attributeArgument = (AttributeArgumentSyntax)context.Node;

            AnalyzeExpression(context, attributeArgument.Expression);
        }

        private void AnalyzeEqualsValueClause(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var equalsValueClause = (EqualsValueClauseSyntax)context.Node;

            AnalyzeExpression(context, equalsValueClause.Value);
        }

        private void AnalyzeAwaitExpression(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var awaitExpression = (AwaitExpressionSyntax)context.Node;

            AnalyzeExpression(context, awaitExpression.Expression);
        }

        private void AnalyzeAssignment(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var assignment = (AssignmentExpressionSyntax)context.Node;

            AnalyzeExpression(context, assignment.Left);
            AnalyzeExpression(context, assignment.Right);
        }

        private static void AnalyzeExpression(SyntaxNodeAnalysisContext context, ExpressionSyntax expression)
        {
            if (expression?.IsKind(SyntaxKind.ParenthesizedExpression) == true)
            {
                var parenthesizedExpression = (ParenthesizedExpressionSyntax)expression;

                SyntaxToken openParen = parenthesizedExpression.OpenParenToken;
                SyntaxToken closeParen = parenthesizedExpression.CloseParenToken;

                context.ReportDiagnostic(
                    DiagnosticDescriptors.RemoveRedundantParentheses,
                    openParen.GetLocation(),
                    additionalLocations: new Location[] { closeParen.GetLocation() });

                context.FadeOutToken(DiagnosticDescriptors.RemoveRedundantParenthesesFadeOut, openParen);
                context.FadeOutToken(DiagnosticDescriptors.RemoveRedundantParenthesesFadeOut, closeParen);
            }
        }
    }
}
