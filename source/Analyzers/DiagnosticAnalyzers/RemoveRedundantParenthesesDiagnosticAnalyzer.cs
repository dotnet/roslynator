// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RemoveRedundantParenthesesDiagnosticAnalyzer : BaseDiagnosticAnalyzer
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

            context.RegisterSyntaxNodeAction(f => AnalyzeParenthesizedExpression(f), SyntaxKind.ParenthesizedExpression);

            context.RegisterSyntaxNodeAction(f => AnalyzeWhileStatement(f), SyntaxKind.WhileStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeDoStatement(f), SyntaxKind.DoStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeUsingStatement(f), SyntaxKind.UsingStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeLockStatement(f), SyntaxKind.LockStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeIfStatement(f), SyntaxKind.IfStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeSwitchStatement(f), SyntaxKind.SwitchStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeCommonForEachStatement(f), SyntaxKind.ForEachStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeCommonForEachStatement(f), SyntaxKind.ForEachVariableStatement);

            context.RegisterSyntaxNodeAction(f => AnalyzeReturnStatement(f), SyntaxKind.ReturnStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeYieldReturnStatement(f), SyntaxKind.YieldReturnStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeExpressionStatement(f), SyntaxKind.ExpressionStatement);
            context.RegisterSyntaxNodeAction(f => AnalyzeArgument(f), SyntaxKind.Argument);
            context.RegisterSyntaxNodeAction(f => AnalyzeAttributeArgument(f), SyntaxKind.AttributeArgument);
            context.RegisterSyntaxNodeAction(f => AnalyzeEqualsValueClause(f), SyntaxKind.EqualsValueClause);
            context.RegisterSyntaxNodeAction(f => AnalyzeAwaitExpression(f), SyntaxKind.AwaitExpression);
            context.RegisterSyntaxNodeAction(f => AnalyzeArrowExpressionClause(f), SyntaxKind.ArrowExpressionClause);
            context.RegisterSyntaxNodeAction(f => AnalyzeInterpolation(f), SyntaxKind.Interpolation);
            context.RegisterSyntaxNodeAction(f => AnalyzeInitializer(f), SyntaxKind.ArrayInitializerExpression);
            context.RegisterSyntaxNodeAction(f => AnalyzeInitializer(f), SyntaxKind.CollectionInitializerExpression);

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
            RemoveRedundantParenthesesRefactoring.Analyze(context, (ParenthesizedExpressionSyntax)context.Node);
        }

        private void AnalyzeWhileStatement(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantParenthesesRefactoring.Analyze(context, (WhileStatementSyntax)context.Node);
        }

        private void AnalyzeDoStatement(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantParenthesesRefactoring.Analyze(context, (DoStatementSyntax)context.Node);
        }

        private void AnalyzeUsingStatement(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantParenthesesRefactoring.Analyze(context, (UsingStatementSyntax)context.Node);
        }

        private void AnalyzeLockStatement(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantParenthesesRefactoring.Analyze(context, (LockStatementSyntax)context.Node);
        }

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantParenthesesRefactoring.Analyze(context, (IfStatementSyntax)context.Node);
        }

        private void AnalyzeSwitchStatement(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantParenthesesRefactoring.Analyze(context, (SwitchStatementSyntax)context.Node);
        }

        private void AnalyzeCommonForEachStatement(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantParenthesesRefactoring.Analyze(context, (CommonForEachStatementSyntax)context.Node);
        }

        private void AnalyzeReturnStatement(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantParenthesesRefactoring.Analyze(context, (ReturnStatementSyntax)context.Node);
        }

        private void AnalyzeYieldReturnStatement(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantParenthesesRefactoring.Analyze(context, (YieldStatementSyntax)context.Node);
        }

        private void AnalyzeExpressionStatement(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantParenthesesRefactoring.Analyze(context, (ExpressionStatementSyntax)context.Node);
        }

        private void AnalyzeArgument(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantParenthesesRefactoring.Analyze(context, (ArgumentSyntax)context.Node);
        }

        private void AnalyzeAttributeArgument(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantParenthesesRefactoring.Analyze(context, (AttributeArgumentSyntax)context.Node);
        }

        private void AnalyzeEqualsValueClause(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantParenthesesRefactoring.Analyze(context, (EqualsValueClauseSyntax)context.Node);
        }

        private void AnalyzeAwaitExpression(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantParenthesesRefactoring.Analyze(context, (AwaitExpressionSyntax)context.Node);
        }

        private void AnalyzeArrowExpressionClause(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantParenthesesRefactoring.Analyze(context, (ArrowExpressionClauseSyntax)context.Node);
        }

        private void AnalyzeInterpolation(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantParenthesesRefactoring.Analyze(context, (InterpolationSyntax)context.Node);
        }

        private void AnalyzeInitializer(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantParenthesesRefactoring.Analyze(context, (InitializerExpressionSyntax)context.Node);
        }

        private void AnalyzeAssignment(SyntaxNodeAnalysisContext context)
        {
            RemoveRedundantParenthesesRefactoring.Analyze(context, (AssignmentExpressionSyntax)context.Node);
        }
    }
}
