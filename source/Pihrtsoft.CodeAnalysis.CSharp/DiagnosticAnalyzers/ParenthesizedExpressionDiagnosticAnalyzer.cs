// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Pihrtsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
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
        }

        private void AnalyzeParenthesizedExpression(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var parenthesizedExpression = (ParenthesizedExpressionSyntax)context.Node;

            AnalyzeExpression(context, parenthesizedExpression.Expression, parenthesizedExpression.OpenParenToken, parenthesizedExpression.CloseParenToken);
        }

        private void AnalyzeWhileStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var whileStatement = (WhileStatementSyntax)context.Node;

            AnalyzeExpression(context, whileStatement.Condition, whileStatement.OpenParenToken, whileStatement.CloseParenToken);
        }

        private void AnalyzeDoStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var doStatement = (DoStatementSyntax)context.Node;

            AnalyzeExpression(context, doStatement.Condition, doStatement.OpenParenToken, doStatement.CloseParenToken);
        }

        private void AnalyzeUsingStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var usingStatement = (UsingStatementSyntax)context.Node;

            AnalyzeExpression(context, usingStatement.Expression, usingStatement.OpenParenToken, usingStatement.CloseParenToken);
        }

        private void AnalyzeLockStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var lockStatement = (LockStatementSyntax)context.Node;

            AnalyzeExpression(context, lockStatement.Expression, lockStatement.OpenParenToken, lockStatement.CloseParenToken);
        }

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var ifStatement = (IfStatementSyntax)context.Node;

            AnalyzeExpression(context, ifStatement.Condition, ifStatement.OpenParenToken, ifStatement.CloseParenToken);
        }

        private void AnalyzeSwitchStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var switchStatement = (SwitchStatementSyntax)context.Node;

            AnalyzeExpression(context, switchStatement.Expression, switchStatement.OpenParenToken, switchStatement.CloseParenToken);
        }

        private static void AnalyzeExpression(
            SyntaxNodeAnalysisContext context,
            ExpressionSyntax expression,
            SyntaxToken openParenToken,
            SyntaxToken closeParenToken)
        {
            if (expression?.IsKind(SyntaxKind.ParenthesizedExpression) == true)
            {
                var parenthesizedExpression = (ParenthesizedExpressionSyntax)expression;

                if (openParenToken.TrailingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia())
                    && closeParenToken.LeadingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia())
                    && parenthesizedExpression.GetLeadingTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia())
                    && parenthesizedExpression.GetTrailingTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                {
                    Diagnostic diagnostic = Diagnostic.Create(
                        DiagnosticDescriptors.RemoveRedundantParentheses,
                        parenthesizedExpression.OpenParenToken.GetLocation(),
                        additionalLocations: new Location[] { parenthesizedExpression.CloseParenToken.GetLocation() });

                    context.ReportDiagnostic(diagnostic);

                    context.FadeOutToken(DiagnosticDescriptors.RemoveRedundantParenthesesFadeOut, parenthesizedExpression.OpenParenToken);
                    context.FadeOutToken(DiagnosticDescriptors.RemoveRedundantParenthesesFadeOut, parenthesizedExpression.CloseParenToken);
                }
            }
        }
    }
}
