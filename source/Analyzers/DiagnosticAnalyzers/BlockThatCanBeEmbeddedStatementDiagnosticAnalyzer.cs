// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Analysis;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BlockThatCanBeEmbeddedStatementDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.RemoveBraces,
                    DiagnosticDescriptors.RemoveBracesFadeOut);
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

            if (!context.Node.IsKind(SyntaxKind.IfStatement)
                || IfElseAnalysis.IsIsolatedIf((IfStatementSyntax)context.Node))
            {
                BlockSyntax block = EmbeddedStatementAnalysis.GetBlockThatCanBeEmbeddedStatement(context.Node);

                if (block != null
                    && !block.OpenBraceToken.IsMissing
                    && !block.CloseBraceToken.IsMissing
                    && block.OpenBraceToken.LeadingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia())
                    && block.OpenBraceToken.TrailingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia())
                    && block.CloseBraceToken.LeadingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia())
                    && block.CloseBraceToken.TrailingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.RemoveBraces,
                        block.GetLocation(),
                        SyntaxHelper.GetNodeTitle(context.Node));

                    context.FadeOutBraces(DiagnosticDescriptors.RemoveBracesFadeOut, block);
                }
            }
        }
    }
}
