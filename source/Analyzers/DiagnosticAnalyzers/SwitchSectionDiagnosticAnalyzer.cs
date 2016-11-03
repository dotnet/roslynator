// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Analyzers;

namespace Roslynator.CSharp.DiagnosticAnalyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SwitchSectionDiagnosticAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get
            {
                return ImmutableArray.Create(
                    DiagnosticDescriptors.FormatSwitchSectionStatementOnSeparateLine,
                    DiagnosticDescriptors.FormatEachStatementOnSeparateLine,
                    DiagnosticDescriptors.RemoveRedundantDefaultSwitchSection,
                    DiagnosticDescriptors.RemoveUnnecessaryCaseLabel);
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

            if (switchSection.Parent?.IsKind(SyntaxKind.SwitchStatement) == true)
            {
                AnalyzeRedundantDefaultSwitchSection(context, switchSection);
                AnalyzeUnnecessaryCaseLabel(context, switchSection);
            }

            AnalyzeFirstStatement(context, switchSection);
        }

        private static void AnalyzeRedundantDefaultSwitchSection(SyntaxNodeAnalysisContext context, SwitchSectionSyntax switchSection)
        {
            if (switchSection.Labels.Any(SyntaxKind.DefaultSwitchLabel)
                && ContainsOnlyBreakStatement(switchSection)
                && switchSection
                    .DescendantTrivia(switchSection.Span)
                    .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.RemoveRedundantDefaultSwitchSection,
                    switchSection.GetLocation());
            }
        }

        private static bool ContainsOnlyBreakStatement(SwitchSectionSyntax switchSection)
        {
            if (switchSection.Statements.Count == 1)
            {
                StatementSyntax statement = switchSection.Statements[0];

                switch (statement.Kind())
                {
                    case SyntaxKind.Block:
                        {
                            var block = (BlockSyntax)statement;

                            if (block.Statements.Count == 1
                                && block.Statements[0].IsKind(SyntaxKind.BreakStatement))
                            {
                                return true;
                            }

                            break;
                        }
                    case SyntaxKind.BreakStatement:
                        {
                            return true;
                        }
                }
            }

            return false;
        }

        private static void AnalyzeUnnecessaryCaseLabel(SyntaxNodeAnalysisContext context, SwitchSectionSyntax switchSection)
        {
            if (switchSection.Labels.Count > 1
                && switchSection.Labels.Any(SyntaxKind.DefaultSwitchLabel))
            {
                foreach (SwitchLabelSyntax label in switchSection.Labels)
                {
                    if (!label.IsKind(SyntaxKind.DefaultSwitchLabel)
                        && label
                            .DescendantTrivia(label.Span)
                            .All(f => f.IsWhitespaceOrEndOfLineTrivia()))
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.RemoveUnnecessaryCaseLabel,
                            label.GetLocation());
                    }
                }
            }
        }

        private static void AnalyzeFirstStatement(SyntaxNodeAnalysisContext context, SwitchSectionSyntax switchSection)
        {
            SyntaxList<StatementSyntax> statements = switchSection.Statements;

            if (statements.Count == 0)
                return;

            if (switchSection.Labels.Count == 0)
                return;

            if (switchSection.Labels.Last().GetSpanEndLine() == statements[0].GetSpanStartLine())
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.FormatSwitchSectionStatementOnSeparateLine,
                    statements[0].GetLocation());
            }
        }
    }
}
