// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Pihrtsoft.CodeAnalysis;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class UsingStatementDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                  DiagnosticDescriptors.SimplifyNestedUsingStatement,
                  DiagnosticDescriptors.SimplifyNestedUsingStatementFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeSyntaxNode(f), SyntaxKind.UsingStatement);
        }

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var usingStatement = (UsingStatementSyntax)context.Node;
            if (usingStatement.Statement?.IsKind(SyntaxKind.Block) == true)
            {
                var block = (BlockSyntax)usingStatement.Statement;
                if (block.Statements.Count == 1
                    && block.Statements[0].IsKind(SyntaxKind.UsingStatement)
                    && CheckTrivia(block, (UsingStatementSyntax)block.Statements[0]))
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.SimplifyNestedUsingStatement,
                        block.GetLocation());

                    DiagnosticHelper.FadeOutBraces(context, block, DiagnosticDescriptors.SimplifyNestedUsingStatementFadeOut);
                }
            }
        }

        private static bool CheckTrivia(BlockSyntax block, UsingStatementSyntax usingStatement)
        {
            return block.OpenBraceToken.TrailingTrivia.All(f => f.IsWhitespaceOrEndOfLine())
                && block.CloseBraceToken.LeadingTrivia.All(f => f.IsWhitespaceOrEndOfLine())
                && usingStatement.GetLeadingTrivia().All(f => f.IsWhitespaceOrEndOfLine())
                && usingStatement.GetTrailingTrivia().All(f => f.IsWhitespaceOrEndOfLine());
        }
    }
}
