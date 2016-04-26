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
    public class ElseClauseDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.RemoveEmptyElseClause,
                    DiagnosticDescriptors.FormatEmbeddedStatementOnSeparateLine,
                    DiagnosticDescriptors.SimplifyElseClauseContainingIfStatement,
                    DiagnosticDescriptors.SimplifyElseClauseContainingIfStatementFadeOut);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeSyntaxNode(f), SyntaxKind.ElseClause);
        }

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var elseClause = (ElseClauseSyntax)context.Node;

            AnalyzeEmptyElseClause(context, elseClause);

            AnalyzeEmbeddedStatement(context, elseClause);

            AnalyzeElseClauseWithSingleIfStatement(context, elseClause);
        }

        private static void AnalyzeEmbeddedStatement(SyntaxNodeAnalysisContext context, ElseClauseSyntax elseClause)
        {
            StatementSyntax statement = elseClause.Statement;

            if (statement == null)
                return;

            if (statement.IsKind(SyntaxKind.Block))
                return;

            if (statement.IsKind(SyntaxKind.IfStatement))
                return;

            if (elseClause.ElseKeyword.GetSpanStartLine() == statement.GetSpanStartLine())
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.FormatEmbeddedStatementOnSeparateLine,
                    statement.GetLocation());
            }
        }

        private static void AnalyzeEmptyElseClause(SyntaxNodeAnalysisContext context, ElseClauseSyntax elseClause)
        {
            if (!elseClause.Statement.IsKind(SyntaxKind.Block))
                return;

            var block = (BlockSyntax)elseClause.Statement;

            if (block.Statements.Count != 0)
                return;

            if (block.OpenBraceToken.TrailingTrivia.Any(f => !f.IsWhitespaceOrEndOfLine()))
                return;

            if (block.CloseBraceToken.LeadingTrivia.Any(f => !f.IsWhitespaceOrEndOfLine()))
                return;

            context.ReportDiagnostic(
                DiagnosticDescriptors.RemoveEmptyElseClause,
                context.Node.GetLocation());
        }

        private static void AnalyzeElseClauseWithSingleIfStatement(SyntaxNodeAnalysisContext context, ElseClauseSyntax elseClause)
        {
            if (elseClause.Statement?.IsKind(SyntaxKind.Block) == true)
            {
                var block = (BlockSyntax)elseClause.Statement;

                if (block.Statements.Count == 1
                    && block.Statements[0].IsKind(SyntaxKind.IfStatement))
                {
                    var ifStatement = (IfStatementSyntax)block.Statements[0];

                    if (ifStatement.Else == null)
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.SimplifyElseClauseContainingIfStatement,
                            block.GetLocation());

                        DiagnosticHelper.FadeOutBraces(context, block, DiagnosticDescriptors.SimplifyElseClauseContainingIfStatementFadeOut);
                    }
                }
            }
        }
    }
}
