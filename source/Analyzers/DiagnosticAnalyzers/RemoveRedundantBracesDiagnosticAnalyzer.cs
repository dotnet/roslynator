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
    public class RemoveRedundantBracesDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.RemoveRedundantBraces,
                    DiagnosticDescriptors.RemoveRedundantBracesFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeBlock(f), SyntaxKind.Block);
        }

        private void AnalyzeBlock(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var block = (BlockSyntax)context.Node;

            if (block.Statements.Count != 1)
                return;

            if (!block.Statements[0].IsKind(SyntaxKind.Block))
                return;

            var block2 = (BlockSyntax)block.Statements[0];

            if (block.OpenBraceToken.TrailingTrivia.Any(f => !f.IsWhitespaceOrEndOfLineTrivia()))
                return;

            if (block.CloseBraceToken.LeadingTrivia.Any(f => !f.IsWhitespaceOrEndOfLineTrivia()))
                return;

            if (block2.OpenBraceToken.LeadingTrivia.Any(f => !f.IsWhitespaceOrEndOfLineTrivia()))
                return;

            if (block2.CloseBraceToken.TrailingTrivia.Any(f => !f.IsWhitespaceOrEndOfLineTrivia()))
                return;

            context.ReportDiagnostic(
                DiagnosticDescriptors.RemoveRedundantBraces,
                block2.GetLocation());

            context.FadeOutBraces(DiagnosticDescriptors.RemoveRedundantBracesFadeOut, block2);
        }
    }
}
