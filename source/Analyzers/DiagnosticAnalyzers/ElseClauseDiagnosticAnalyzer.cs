// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.DiagnosticAnalyzers
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
                    DiagnosticDescriptors.MergeElseClauseWithNestedIfStatement,
                    DiagnosticDescriptors.MergeElseClauseWithNestedIfStatementFadeOut);
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
                        if (elseClause
                            .DescendantTrivia(elseClause.Span)
                            .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
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

                            if (ifStatement.Else == null
                                && CheckTrivia(elseClause, ifStatement))
                            {
                                context.ReportDiagnostic(
                                    DiagnosticDescriptors.MergeElseClauseWithNestedIfStatement,
                                    block.GetLocation());

                                context.FadeOutBraces(
                                    DiagnosticDescriptors.MergeElseClauseWithNestedIfStatementFadeOut,
                                    block);
                            }
                        }
                    }
                }
            }
        }

        private static bool CheckTrivia(ElseClauseSyntax elseClause, IfStatementSyntax ifStatement)
        {
            TextSpan elseSpan = elseClause.Span;
            TextSpan ifSpan = ifStatement.Span;

            TextSpan span = TextSpan.FromBounds(elseSpan.Start, ifSpan.Start);
            TextSpan span2 = TextSpan.FromBounds(ifSpan.End, elseSpan.End);

            foreach (SyntaxTrivia trivia in elseClause.DescendantTrivia())
            {
                TextSpan triviaSpan = trivia.Span;

                if (span.Contains(triviaSpan))
                {
                    if (!trivia.IsWhitespaceOrEndOfLineTrivia())
                        return false;
                }
                else if (span2.Contains(triviaSpan))
                {
                    if (!trivia.IsWhitespaceOrEndOfLineTrivia())
                        return false;
                }
            }

            return true;
        }
    }
}
