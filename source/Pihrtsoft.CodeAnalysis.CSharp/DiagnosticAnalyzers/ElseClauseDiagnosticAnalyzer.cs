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
                    DiagnosticDescriptors.SimplifyElseClauseContainingOnlyIfStatement,
                    DiagnosticDescriptors.SimplifyElseClauseContainingOnlyIfStatementFadeOut);
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
                        if (elseClause.ElseKeyword.TrailingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia())
                            && block.OpenBraceToken.LeadingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia())
                            && block.OpenBraceToken.TrailingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia())
                            && block.CloseBraceToken.LeadingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia()))
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
                                    DiagnosticDescriptors.SimplifyElseClauseContainingOnlyIfStatement,
                                    block.GetLocation());

                                context.FadeOutBraces(
                                    DiagnosticDescriptors.SimplifyElseClauseContainingOnlyIfStatementFadeOut,
                                    block);
                            }
                        }
                    }
                }
            }
        }
    }
}
