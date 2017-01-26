// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Extensions;
using Roslynator.CSharp.Refactorings;
using Roslynator.Extensions;

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

        private static DiagnosticDescriptor FadeOutDescriptor
        {
            get { return DiagnosticDescriptors.UseExpressionBodiedMemberFadeOut; }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeOperatorDeclaration, SyntaxKind.OperatorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeConversionOperatorDeclaration, SyntaxKind.ConversionOperatorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzePropertyDeclaration, SyntaxKind.PropertyDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeIndexerDeclaration, SyntaxKind.IndexerDeclaration);
        }

        private void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var method = (MethodDeclarationSyntax)context.Node;

            if (method.ExpressionBody == null)
            {
                BlockSyntax body = method.Body;

                ExpressionSyntax expression = UseExpressionBodiedMemberRefactoring.GetMethodExpression(body);

                if (expression != null)
                    AnalyzeExpression(context, body, expression);
            }
        }

        private void AnalyzeOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (OperatorDeclarationSyntax)context.Node;

            if (declaration.ExpressionBody == null)
                AnalyzeBody(context, declaration.Body);
        }

        private void AnalyzeConversionOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (ConversionOperatorDeclarationSyntax)context.Node;

            if (declaration.ExpressionBody == null)
                AnalyzeBody(context, declaration.Body);
        }

        private void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (PropertyDeclarationSyntax)context.Node;

            if (declaration.ExpressionBody == null)
                AnalyzeAccessorList(context, declaration.AccessorList);
        }

        private void AnalyzeIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (IndexerDeclarationSyntax)context.Node;

            if (declaration.ExpressionBody == null)
                AnalyzeAccessorList(context, declaration.AccessorList);
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

        private static void AnalyzeAccessorList(SyntaxNodeAnalysisContext context, AccessorListSyntax accessorList)
        {
            ExpressionSyntax expression = UseExpressionBodiedMemberRefactoring.GetReturnExpression(accessorList);

            if (expression != null
                && accessorList.DescendantTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia())
                && expression.IsSingleLine())
            {
                AccessorDeclarationSyntax accessor = accessorList.Accessors.First();

                ReportDiagnostic(context, accessor.Body, expression);

                context.ReportToken(FadeOutDescriptor, accessor.Keyword);
                context.ReportBraces(FadeOutDescriptor, accessorList);
            }
        }

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, BlockSyntax block, ExpressionSyntax expression)
        {
            context.ReportDiagnostic(
                DiagnosticDescriptors.UseExpressionBodiedMember,
                block.GetLocation());

            SyntaxNode parent = expression.Parent;

            if (parent.IsKind(SyntaxKind.ReturnStatement))
                context.ReportToken(FadeOutDescriptor, ((ReturnStatementSyntax)parent).ReturnKeyword);

            context.ReportBraces(FadeOutDescriptor, block);
        }
    }
}
