// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Analyzers;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IfStatementSyntaxDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.AddBracesToIfElse,
                    DiagnosticDescriptors.RemoveBracesFromIfElse,
                    DiagnosticDescriptors.RemoveBracesFromIfElseFadeOut,
                    DiagnosticDescriptors.MergeIfStatementWithNestedIfStatement,
                    DiagnosticDescriptors.MergeIfStatementWithNestedIfStatementFadeOut,
                    DiagnosticDescriptors.ReplaceIfStatementWithReturnStatement,
                    DiagnosticDescriptors.ReplaceIfStatementWithReturnStatementFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeIfStatement(f), SyntaxKind.IfStatement);
        }

        private void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            if (context.Node.Parent?.IsKind(SyntaxKind.ElseClause) == true)
                return;

            var ifStatement = (IfStatementSyntax)context.Node;

            if (ifStatement.Else != null)
            {
                var result = new IfElseAnalysisResult(ifStatement);

                if (result.AddBraces)
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.AddBracesToIfElse,
                        ifStatement.GetLocation());
                }

                if (result.RemoveBraces)
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.RemoveBracesFromIfElse,
                        ifStatement.GetLocation());

                    RemoveBracesFromIfElseFadeOut(context, ifStatement);
                }
            }

            MergeIfStatementWithNestedIfStatementAnalyzer.Analyze(context, ifStatement);

            SimplifyIfStatementToReturnStatementAnalyzer.Analyze(context);
        }

        private static void RemoveBracesFromIfElseFadeOut(SyntaxNodeAnalysisContext context, IfStatementSyntax ifStatement)
        {
            foreach (SyntaxNode node in ifStatement.DescendantNodes())
            {
                if (node.IsKind(SyntaxKind.Block))
                {
                    context.FadeOutBraces(
                        DiagnosticDescriptors.RemoveBracesFromIfElseFadeOut,
                        (BlockSyntax)node);
                }
            }
        }
    }
}
