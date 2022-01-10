// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.Formatting.CSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class FormatBlockBracesAnalyzer : BaseDiagnosticAnalyzer
    {
        private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                if (_supportedDiagnostics.IsDefault)
                {
                    Immutable.InterlockedInitialize(
                        ref _supportedDiagnostics,
                        DiagnosticRules.FormatBlockBraces,
                        DiagnosticRules.AddNewLineAfterOpeningBraceOfEmptyBlock);
                }

                return _supportedDiagnostics;
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            base.Initialize(context);

            context.RegisterSyntaxNodeAction(f => AnalyzeBlock(f), SyntaxKind.Block);
        }

        private static void AnalyzeBlock(SyntaxNodeAnalysisContext context)
        {
            var block = (BlockSyntax)context.Node;

            if (block.Parent is AccessorDeclarationSyntax)
                return;

            if (block.Parent is AnonymousFunctionExpressionSyntax)
                return;

            SyntaxToken openBrace = block.OpenBraceToken;

            if (openBrace.IsMissing)
                return;

            if (DiagnosticRules.AddNewLineAfterOpeningBraceOfEmptyBlock.IsEffective(context)
                && block.SyntaxTree.IsSingleLineSpan(block.Span))
            {
                DiagnosticHelpers.ReportDiagnostic(
                    context,
                    DiagnosticRules.AddNewLineAfterOpeningBraceOfEmptyBlock,
                    Location.Create(block.SyntaxTree, new TextSpan(openBrace.Span.End, 0)));
            }

            BlockBracesStyle style = context.GetBlockBracesStyle();

            if (style == BlockBracesStyle.None)
                return;

            if (block.SyntaxTree.IsSingleLineSpan(block.Span))
            {
                if (style == BlockBracesStyle.MultiLine
                    || !IsEmptyBlock(block))
                {
                    DiagnosticHelpers.ReportDiagnostic(
                        context,
                        DiagnosticRules.FormatBlockBraces,
                        block.OpenBraceToken,
                        "multiple lines");
                }
            }
            else if (style == BlockBracesStyle.SingleLineWhenEmpty
                && IsEmptyBlock(block))
            {
                DiagnosticHelpers.ReportDiagnostic(
                    context,
                    DiagnosticRules.FormatBlockBraces,
                    block.OpenBraceToken,
                    "a single line");
            }
        }

        private static bool IsEmptyBlock(BlockSyntax block)
        {
            return !block.Statements.Any()
                && block.OpenBraceToken.TrailingTrivia.IsEmptyOrWhitespace()
                && block.CloseBraceToken.LeadingTrivia.IsEmptyOrWhitespace();
        }
    }
}
