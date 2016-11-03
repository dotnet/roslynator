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
    public class FinallyClauseDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(DiagnosticDescriptors.RemoveEmptyFinallyClause);

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeSyntaxNode(f), SyntaxKind.FinallyClause);
        }

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var finallyClause = (FinallyClauseSyntax)context.Node;

            if (finallyClause.Parent?.IsKind(SyntaxKind.TryStatement) == true)
            {
                var tryStatement = (TryStatementSyntax)finallyClause.Parent;

                if (tryStatement.Catches.Count > 0)
                {
                    BlockSyntax block = finallyClause.Block;

                    if (block != null
                        && block.Statements.Count == 0
                        && finallyClause.FinallyKeyword.TrailingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia())
                        && block.OpenBraceToken.LeadingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia())
                        && block.OpenBraceToken.TrailingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia())
                        && block.CloseBraceToken.LeadingTrivia.All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.RemoveEmptyFinallyClause,
                            finallyClause.GetLocation());
                    }
                }
            }
        }
    }
}
