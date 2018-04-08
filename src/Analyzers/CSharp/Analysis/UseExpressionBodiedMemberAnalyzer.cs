// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UseExpressionBodiedMemberAnalyzer : BaseDiagnosticAnalyzer
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
            context.RegisterSyntaxNodeAction(AnalyzeConstructorDeclaration, SyntaxKind.ConstructorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeDestructorDeclaration, SyntaxKind.DestructorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeLocalFunctionStatement, SyntaxKind.LocalFunctionStatement);
            context.RegisterSyntaxNodeAction(AnalyzeAccessorDeclaration, SyntaxKind.GetAccessorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeAccessorDeclaration, SyntaxKind.SetAccessorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeAccessorDeclaration, SyntaxKind.AddAccessorDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeAccessorDeclaration, SyntaxKind.RemoveAccessorDeclaration);
        }

        private static void  AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;

            if (method.ExpressionBody == null)
            {
                BlockSyntax body = method.Body;

                ExpressionSyntax expression = UseExpressionBodiedMemberAnalysis.GetExpression(body);

                if (expression != null)
                    AnalyzeExpression(context, body, expression);
            }
        }

        private static void  AnalyzeOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (OperatorDeclarationSyntax)context.Node;

            if (declaration.ExpressionBody == null)
                AnalyzeBody(context, declaration.Body);
        }

        private static void  AnalyzeConversionOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (ConversionOperatorDeclarationSyntax)context.Node;

            if (declaration.ExpressionBody == null)
                AnalyzeBody(context, declaration.Body);
        }

        private static void  AnalyzeConstructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (ConstructorDeclarationSyntax)context.Node;

            if (declaration.ExpressionBody == null)
            {
                BlockSyntax body = declaration.Body;

                ExpressionSyntax expression = UseExpressionBodiedMemberAnalysis.GetExpression(body);

                if (expression != null)
                    AnalyzeExpression(context, body, expression);
            }
        }

        private static void  AnalyzeDestructorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var declaration = (DestructorDeclarationSyntax)context.Node;

            if (declaration.ExpressionBody == null)
            {
                BlockSyntax body = declaration.Body;

                ExpressionSyntax expression = UseExpressionBodiedMemberAnalysis.GetExpression(body);

                if (expression != null)
                    AnalyzeExpression(context, body, expression);
            }
        }

        private static void  AnalyzeLocalFunctionStatement(SyntaxNodeAnalysisContext context)
        {
            var localFunctionStatement = (LocalFunctionStatementSyntax)context.Node;

            if (localFunctionStatement.ExpressionBody == null)
            {
                BlockSyntax body = localFunctionStatement.Body;

                ExpressionSyntax expression = UseExpressionBodiedMemberAnalysis.GetExpression(body);

                if (expression != null)
                    AnalyzeExpression(context, body, expression);
            }
        }

        private static void  AnalyzeAccessorDeclaration(SyntaxNodeAnalysisContext context)
        {
            var accessor = (AccessorDeclarationSyntax)context.Node;

            if (accessor.ExpressionBody == null
                && !accessor.AttributeLists.Any())
            {
                BlockSyntax body = accessor.Body;

                ExpressionSyntax expression = UseExpressionBodiedMemberAnalysis.GetExpression(body);

                if (expression?.IsSingleLine() == true)
                {
                    if (accessor.Parent is AccessorListSyntax accessorList)
                    {
                        SyntaxList<AccessorDeclarationSyntax> accessors = accessorList.Accessors;

                        if (accessors.Count == 1
                            && accessors.First().IsKind(SyntaxKind.GetAccessorDeclaration))
                        {
                            if (accessorList.DescendantTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                            {
                                ReportDiagnostic(context, accessorList, expression);
                                context.ReportToken(DiagnosticDescriptors.UseExpressionBodiedMemberFadeOut, accessor.Keyword);
                                context.ReportBraces(DiagnosticDescriptors.UseExpressionBodiedMemberFadeOut, body);
                            }

                            return;
                        }
                    }

                    if (accessor.DescendantTrivia().All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                        ReportDiagnostic(context, body, expression);
                }
            }
        }

        private static void AnalyzeBody(SyntaxNodeAnalysisContext context, BlockSyntax body)
        {
            ExpressionSyntax expression = UseExpressionBodiedMemberAnalysis.GetReturnExpression(body);

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

        private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, AccessorListSyntax accessorList, ExpressionSyntax expression)
        {
            context.ReportDiagnostic(
                DiagnosticDescriptors.UseExpressionBodiedMember,
                accessorList);

            SyntaxNode parent = expression.Parent;

            if (parent.IsKind(SyntaxKind.ReturnStatement))
                context.ReportToken(DiagnosticDescriptors.UseExpressionBodiedMemberFadeOut, ((ReturnStatementSyntax)parent).ReturnKeyword);

            context.ReportBraces(DiagnosticDescriptors.UseExpressionBodiedMemberFadeOut, accessorList);
        }
    }
}
