// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

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

            var declaration = (MethodDeclarationSyntax)context.Node;

            if (declaration.Body == null)
                return;

            if (declaration.ExpressionBody != null)
                return;

            if (declaration.ReturnsVoid())
                return;

            AnalyzeBlock(context, declaration.Body);
        }

        private void AnalyzeOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (OperatorDeclarationSyntax)context.Node;

            if (declaration.ExpressionBody != null)
                return;

            AnalyzeBlock(context, declaration.Body);
        }

        private void AnalyzeConversionOperatorDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (ConversionOperatorDeclarationSyntax)context.Node;

            if (declaration.ExpressionBody != null)
                return;

            AnalyzeBlock(context, declaration.Body);
        }

        private void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (PropertyDeclarationSyntax)context.Node;

            if (declaration.ExpressionBody != null)
                return;

            AnalyzeAccessorList(context, declaration.AccessorList);
        }

        private void AnalyzeIndexerDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var declaration = (IndexerDeclarationSyntax)context.Node;

            if (declaration.ExpressionBody != null)
                return;

            AnalyzeAccessorList(context, declaration.AccessorList);
        }

        private static void AnalyzeAccessorList(SyntaxNodeAnalysisContext context, AccessorListSyntax accessorList)
        {
            if (accessorList == null)
                return;

            SyntaxList<AccessorDeclarationSyntax> accessors = accessorList.Accessors;

            if (accessors.Count != 1)
                return;

            AccessorDeclarationSyntax accessor = accessors[0];

            if (accessor.Body == null)
                return;

            if (!accessor.IsKind(SyntaxKind.GetAccessorDeclaration))
                return;

            if (accessor.AttributeLists.Count != 0)
                return;

            if (accessor.Body
                .DescendantTrivia(descendIntoTrivia: true)
                .Any(f => !f.IsWhitespaceOrEndOfLineTrivia()))
            {
                return;
            }

            if (AnalyzeBlock(context, accessor.Body, checkTrivia: false))
            {
                context.FadeOutToken(DiagnosticDescriptors.UseExpressionBodiedMemberFadeOut, accessor.Keyword);
                context.FadeOutBraces(DiagnosticDescriptors.UseExpressionBodiedMemberFadeOut, accessorList);
            }
        }

        private static bool AnalyzeBlock(SyntaxNodeAnalysisContext context, BlockSyntax block, bool checkTrivia = true)
        {
            if (block == null)
                return false;

            SyntaxList<StatementSyntax> statements = block.Statements;
            if (statements.Count != 1)
                return false;

            var returnStatement = statements[0] as ReturnStatementSyntax;
            if (returnStatement == null)
                return false;

            if (returnStatement.Expression == null)
                return false;

            if (checkTrivia && block
                .DescendantTrivia(descendIntoTrivia: true)
                .Any(f => !f.IsWhitespaceOrEndOfLineTrivia()))
            {
                return false;
            }

            if (!returnStatement.IsSingleLine())
                return false;

            context.ReportDiagnostic(
                DiagnosticDescriptors.UseExpressionBodiedMember,
                block.GetLocation());

            context.FadeOutToken(DiagnosticDescriptors.UseExpressionBodiedMemberFadeOut, returnStatement.ReturnKeyword);
            context.FadeOutBraces(DiagnosticDescriptors.UseExpressionBodiedMemberFadeOut, block);

            return true;
        }
    }
}
