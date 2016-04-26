// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Pihrtsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BlockDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.FormatBlock,
                    DiagnosticDescriptors.FormatEachStatementOnSeparateLine,
                    DiagnosticDescriptors.SimplifyLambdaExpression,
                    DiagnosticDescriptors.SimplifyLambdaExpressionFadeOut);
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

            DiagnosticHelper.AnalyzeStatements(context, block.Statements);

            if (block.Parent != null)
            {
                if (block.Parent.IsAccessorDeclaration())
                    return;

                if (block.Parent.IsKind(SyntaxKind.AnonymousMethodExpression))
                    return;

                if (block.Parent.IsKind(SyntaxKind.SimpleLambdaExpression) || block.Parent.IsKind(SyntaxKind.ParenthesizedLambdaExpression))
                {
                    if (block.Statements.Count == 1
                        && block.Statements[0].IsAnyKind(SyntaxKind.ReturnStatement, SyntaxKind.ExpressionStatement)
                        && block.IsSingleline())
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.SimplifyLambdaExpression,
                            block.GetLocation());

                        FadeOut(context, block);
                    }

                    return;
                }
            }

            if (block.Statements.Count == 0)
            {
                int startLineIndex = block.OpenBraceToken.GetSpanStartLine();
                int endLineIndex = block.CloseBraceToken.GetSpanEndLine();

                if ((endLineIndex - startLineIndex) != 1
                    && block.OpenBraceToken.TrailingTrivia.All(f => f.IsWhitespaceOrEndOfLine())
                    && block.CloseBraceToken.LeadingTrivia.All(f => f.IsWhitespaceOrEndOfLine()))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.FormatBlock,
                        block.GetLocation());
                }
            }
        }

        private static void FadeOut(SyntaxNodeAnalysisContext context, BlockSyntax block)
        {
            DiagnosticDescriptor descriptor = DiagnosticDescriptors.SimplifyLambdaExpressionFadeOut;

            DiagnosticHelper.FadeOutBraces(context, block, descriptor);

            if (block.Statements[0].IsKind(SyntaxKind.ReturnStatement))
                DiagnosticHelper.FadeOutToken(context, ((ReturnStatementSyntax)block.Statements[0]).ReturnKeyword, descriptor);
        }
    }
}
