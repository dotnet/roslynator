// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Refactorings;
using Roslynator.Extensions;

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
                    DiagnosticDescriptors.AddBracesToSwitchSectionWithMultipleStatements);
            }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(f => AnalyzeSyntaxNode(f), SyntaxKind.SwitchSection);
        }

        private void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            var switchSection = (SwitchSectionSyntax)context.Node;

            FormatEachStatementOnSeparateLineRefactoring.Analyze(context, switchSection);

            RemoveRedundantDefaultSwitchSectionRefactoring.Analyze(context, switchSection);

            RemoveUnnecessaryCaseLabelRefactoring.Analyze(context, switchSection);

            FormatSwitchSectionStatementOnSeparateLineRefactoring.Analyze(context, switchSection);

            DefaultLabelShouldBeLastLabelInSwitchSectionRefactoring.Analyze(context, switchSection);

            if (switchSection.Statements.Count > 1)
            {
                context.ReportDiagnostic(
                    DiagnosticDescriptors.AddBracesToSwitchSectionWithMultipleStatements,
                    Location.Create(switchSection.SyntaxTree, switchSection.Statements.Span));
            }
        }
    }
}
