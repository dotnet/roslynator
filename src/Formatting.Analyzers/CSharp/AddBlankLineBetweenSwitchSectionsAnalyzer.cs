// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.Formatting.CSharp;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AddBlankLineBetweenSwitchSectionsAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.AddBlankLineBetweenSwitchSections);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(f => AnalyzeSwitchStatement(f), SyntaxKind.SwitchStatement);
    }

    private static void AnalyzeSwitchStatement(SyntaxNodeAnalysisContext context)
    {
        var switchStatement = (SwitchStatementSyntax)context.Node;

        SyntaxList<SwitchSectionSyntax> sections = switchStatement.Sections;

        SyntaxList<SwitchSectionSyntax>.Enumerator en = sections.GetEnumerator();

        if (!en.MoveNext())
            return;

        SwitchSectionSyntax previousSection = en.Current;

        var previousBlock = previousSection.Statements.SingleOrDefault(shouldThrow: false) as BlockSyntax;

        while (en.MoveNext())
        {
            TriviaBetweenAnalysis analysis = TriviaBetweenAnalysis.Create(previousSection, en.Current);

            if (!analysis.Success)
                continue;

            if (analysis.Kind != TriviaBetweenKind.BlankLine)
            {
                if (context.GetBlankLineBetweenClosingBraceAndSwitchSection() != false
                    || previousBlock is null)
                {
                    DiagnosticHelpers.ReportDiagnostic(
                        context,
                        DiagnosticRules.AddBlankLineBetweenSwitchSections,
                        analysis.GetLocation());
                }
            }

            previousSection = en.Current;
            previousBlock = en.Current.Statements.SingleOrDefault(shouldThrow: false) as BlockSyntax;
        }
    }
}
