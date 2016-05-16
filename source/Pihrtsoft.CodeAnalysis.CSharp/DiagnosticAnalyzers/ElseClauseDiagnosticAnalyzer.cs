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

            StatementSyntax statement = elseClause.Statement;

            if (statement != null)
            {
                if (!statement.IsKind(SyntaxKind.Block)
                    && !statement.IsKind(SyntaxKind.IfStatement)
                    && elseClause.ElseKeyword.GetSpanStartLine() == statement.GetSpanStartLine())
                {
                    context.ReportDiagnostic(
                        DiagnosticDescriptors.FormatEmbeddedStatementOnSeparateLine,
                        statement.GetLocation());
                }

                if (statement.IsKind(SyntaxKind.Block))
                {
                    var block = (BlockSyntax)statement;

                    if (block.Statements.Count == 0)
                    {
                        if (elseClause.ElseKeyword.TrailingTrivia.IsWhitespaceOrEndOfLine()
                            && block.OpenBraceToken.LeadingTrivia.IsWhitespaceOrEndOfLine()
                            && block.OpenBraceToken.TrailingTrivia.IsWhitespaceOrEndOfLine()
                            && block.CloseBraceToken.LeadingTrivia.IsWhitespaceOrEndOfLine())
                        {
                            context.ReportDiagnostic(
                                DiagnosticDescriptors.RemoveEmptyElseClause,
                                elseClause.GetLocation());
                        }
                    }
                    else if (block.Statements.Count == 1)
                    {
                        if (block.Statements[0].IsKind(SyntaxKind.IfStatement))
                        {
                            var ifStatement = (IfStatementSyntax)block.Statements[0];

                            if (ifStatement.Else == null)
                            {
                                context.ReportDiagnostic(
                                    DiagnosticDescriptors.SimplifyElseClauseContainingIfStatement,
                                    block.GetLocation());

                                DiagnosticHelper.FadeOutBraces(
                                    context,
                                    block,
                                    DiagnosticDescriptors.SimplifyElseClauseContainingIfStatementFadeOut);
                            }
                        }
                    }
                }
            }
        }
    }
}
