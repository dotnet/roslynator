// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Refactorings;

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
                    DiagnosticDescriptors.RemoveUnnecessaryCaseLabel,
                    DiagnosticDescriptors.DefaultLabelShouldBeLastLabelInSwitchSection,
                    DiagnosticDescriptors.AddBracesToSwitchSectionWithMultipleStatements,
                    DiagnosticDescriptors.AddBreakStatementToSwitchSection);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(f => AnalyzeSwitchSection(f), SyntaxKind.SwitchSection);
        }

        private void AnalyzeSwitchSection(SyntaxNodeAnalysisContext context)
        {
            var switchSection = (SwitchSectionSyntax)context.Node;

            FormatEachStatementOnSeparateLineRefactoring.Analyze(context, switchSection);

            if (AddBreakStatementToSwitchSectionRefactoring.CanRefactor(context, switchSection))
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.AddBreakStatementToSwitchSection,
                    switchSection);
            }

            RemoveRedundantDefaultSwitchSectionRefactoring.Analyze(context, switchSection);

            RemoveUnnecessaryCaseLabelRefactoring.Analyze(context, switchSection);

            FormatSwitchSectionStatementOnSeparateLineRefactoring.Analyze(context, switchSection);

            DefaultLabelShouldBeLastLabelInSwitchSectionRefactoring.Analyze(context, switchSection);

            SyntaxList<StatementSyntax> statements = switchSection.Statements;

            if (statements.Count > 1)
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.AddBracesToSwitchSectionWithMultipleStatements,
                    Location.Create(switchSection.SyntaxTree, statements.Span));
            }
        }
    }
}
