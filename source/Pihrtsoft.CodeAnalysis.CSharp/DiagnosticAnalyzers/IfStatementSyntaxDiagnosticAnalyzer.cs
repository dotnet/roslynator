// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Pihrtsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class IfStatementSyntaxDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.AddBracesToIfElseChain,
                    DiagnosticDescriptors.RemoveBracesFromIfElseChain,
                    DiagnosticDescriptors.RemoveBracesFromStatementFadeOut);
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

            if (ifStatement.Else == null)
                return;

            var result = new IfElseChainAnalysisResult(ifStatement);

            if (result.AddBracesToChain)
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.AddBracesToIfElseChain,
                    ifStatement.GetLocation());
            }

            if (result.RemoveBracesFromChain)
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.RemoveBracesFromIfElseChain,
                    ifStatement.GetLocation());

                foreach (SyntaxNode node in ifStatement.DescendantNodes())
                {
                    if (node.IsKind(SyntaxKind.Block))
                    {
                        DiagnosticHelper.FadeOutBraces(
                            context,
                            (BlockSyntax)node,
                            DiagnosticDescriptors.RemoveBracesFromStatementFadeOut);
                    }
                }
            }
        }
    }
}
