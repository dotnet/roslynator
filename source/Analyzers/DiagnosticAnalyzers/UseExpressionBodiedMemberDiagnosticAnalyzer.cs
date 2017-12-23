// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseExpressionBodiedMemberDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.UseExpressionBodiedMember,
                    DiagnosticDescriptors.UseExpressionBodiedMemberFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeOperatorDeclaration, SyntaxKind.OperatorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeConversionOperatorDeclaration, SyntaxKind.ConversionOperatorDeclaration);
        }

        private void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;

            if (method.ExpressionBody == null)
            {
                BlockSyntax body = method.Body;

                ExpressionSyntax expression = UseExpressionBodiedMemberRefactoring.GetExpression(body);

                if (expression != null)
                    AnalyzeExpression(context, body, expression);
            }
        }

        private void AnalyzeOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (OperatorDeclarationSyntax)context.Node;

            if (declaration.ExpressionBody == null)
                AnalyzeBody(context, declaration.Body);
        }

        private void AnalyzeConversionOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (ConversionOperatorDeclarationSyntax)context.Node;

            if (declaration.ExpressionBody == null)
                AnalyzeBody(context, declaration.Body);
        }

        private static void AnalyzeBody(SyntaxNodeAnalysisContext context, BlockSyntax body)
        {
            ExpressionSyntax expression = UseExpressionBodiedMemberRefactoring.GetReturnExpression(body);

            if (expression != null)
                AnalyzeExpression(context, body, expression);
        }

        private static void AnalyzeExpression(SyntaxNodeAnalysisContext context, BlockSyntax block, ExpressionSyntax expression)
        {
            if (block.DescendantTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia())
                && expression.IsSingleLine())
            {
                ReportDiagnostic(context, block, expression);
            }
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, BlockSyntax block, ExpressionSyntax expression)
        {
            context.ReportDiagnostic(
                DiagnosticDescriptors.UseExpressionBodiedMember,
                block);

            SyntaxNode parent = expression.Parent;

            if (parent.IsKind(SyntaxKind.ReturnStatement))
                context.ReportToken(DiagnosticDescriptors.UseExpressionBodiedMemberFadeOut, ((ReturnStatementSyntax)parent).ReturnKeyword);

            context.ReportBraces(DiagnosticDescriptors.UseExpressionBodiedMemberFadeOut, block);
        }
    }
}
