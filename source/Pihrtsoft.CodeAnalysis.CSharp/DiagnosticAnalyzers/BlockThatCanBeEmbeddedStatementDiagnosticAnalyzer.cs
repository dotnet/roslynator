// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Pihrtsoft.CodeAnalysis.CSharp.Analysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BlockThatCanBeEmbeddedStatementDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.RemoveBracesFromStatement,
                    DiagnosticDescriptors.RemoveBracesFromStatementFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeStatement(f),
                SyntaxKind.IfStatement,
                SyntaxKind.ForEachStatement,
                SyntaxKind.ForStatement,
                SyntaxKind.UsingStatement,
                SyntaxKind.WhileStatement,
                SyntaxKind.DoStatement,
                SyntaxKind.LockStatement,
                SyntaxKind.FixedStatement);
        }

        private void AnalyzeStatement(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            if (context.Node.IsKind(SyntaxKind.IfStatement)
                && !IfElseChainAnalysis.IsIsolatedIf((IfStatementSyntax)context.Node))
            {
                return;
            }

            BlockSyntax block = EmbeddedStatementAnalysis.GetBlockThatCanBeEmbeddedStatement(context.Node);

            if (block == null)
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.RemoveBracesFromStatement, block.GetLocation());

            DiagnosticHelper.FadeOutBraces(
                context,
                block,
                DiagnosticDescriptors.RemoveBracesFromStatementFadeOut);
        }
    }
}
