// Copyright (c) Josef Pihrt. All rights reserved. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Roslynator.CSharp.Analysis
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DefaultLabelShouldBeLastLabelInSwitchSectionAnalyzer : BaseDiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(DiagnosticDescriptors.DefaultLabelShouldBeLastLabelInSwitchSection); }
        }

        public override void Initialize(AnalysisContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            base.Initialize(context);

            context.RegisterSyntaxNodeAction(AnalyzeSwitchSection, SyntaxKind.SwitchSection);
        }

        public static void AnalyzeSwitchSection(SyntaxNodeAnalysisContext context)
        {
            var switchSection = (SwitchSectionSyntax)context.Node;

            SyntaxList<SwitchLabelSyntax> labels = switchSection.Labels;

            int count = labels.Count;

            if (count <= 1)
                return;

            SwitchLabelSyntax lastLabel = labels.Last();

            for (int i = 0; i < count - 1; i++)
            {
                SwitchLabelSyntax label = labels[i];

                if (label.Kind() == SyntaxKind.DefaultSwitchLabel)
                {
                    TextSpan span = TextSpan.FromBounds(label.Span.End, lastLabel.SpanStart);

                    if (!switchSection.ContainsDirectives(span))
                    {
                        context.ReportDiagnostic(
                            DiagnosticDescriptors.DefaultLabelShouldBeLastLabelInSwitchSection,
                            label);

                        break;
                    }
                }
            }
        }
    }
}
