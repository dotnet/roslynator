// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RemoveEmptyElseClauseAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.RemoveEmptyElseClause); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeElseClause, SyntaxKind.ElseClause);
        }

        public static void AnalyzeElseClause(SyntaxNodeAnalysisContext context)
        {
            var elseClause = (ElseClauseSyntax)context.Node;

            StatementSyntax statement = elseClause.Statement;

            if (statement?.Kind() != SyntaxKind.Block)
                return;

            var block = (BlockSyntax)statement;

            if (block.Statements.Any())
                return;

            if (!elseClause.ElseKeyword.TrailingTrivia.IsEmptyOrWhitespace())
                return;

            SyntaxToken openBrace = block.OpenBraceToken;

            if (!openBrace.LeadingTrivia.IsEmptyOrWhitespace())
                return;

            if (!openBrace.TrailingTrivia.IsEmptyOrWhitespace())
                return;

            if (!block.CloseBraceToken.LeadingTrivia.IsEmptyOrWhitespace())
                return;

            context.ReportDiagnostic(DiagnosticDescriptors.RemoveEmptyElseClause, elseClause);
        }
    }
}
