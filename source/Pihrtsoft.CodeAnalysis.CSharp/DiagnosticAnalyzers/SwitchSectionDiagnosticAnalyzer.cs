// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Pihrtsoft.CodeAnalysis.CSharp.Analyzers;

namespace Pihrtsoft.CodeAnalysis.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SwitchSectionDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.FormatCaseLabelStatementOnSeparateLine,
                    DiagnosticDescriptors.FormatEachStatementOnSeparateLine);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            context.RegisterSyntaxNodeAction(f => AnalyzeSyntaxNode(f), SyntaxKind.SwitchSection);
        }

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            if (GeneratedCodeAnalyzer?.IsGeneratedCode(context) == true)
                return;

            var switchSection = (SwitchSectionSyntax)context.Node;

            FormatEachStatementOnSeparateLineAnalyzer.AnalyzeStatements(context, switchSection.Statements);

            AnalyzeFirstStatement(context, switchSection);
        }

        public static void AnalyzeFirstStatement(SyntaxNodeAnalysisContext context, SwitchSectionSyntax switchSection)
        {
            if (switchSection == null)
                throw new ArgumentNullException(nameof(switchSection));

            SyntaxList<StatementSyntax> statements = switchSection.Statements;

            if (statements.Count == 0)
                return;

            if (switchSection.Labels.Count == 0)
                return;

            if (switchSection.Labels.Last().GetSpanEndLine() == statements[0].GetSpanStartLine())
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.FormatCaseLabelStatementOnSeparateLine,
                    statements[0].GetLocation());
            }
        }
    }
}
