// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;
using Roslynator.CSharp.CodeStyle;

namespace Roslynator.Formatting.CSharp;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class BlankLineBetweenSwitchSectionsAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.BlankLineBetweenSwitchSections);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(c => AnalyzeSwitchStatement(c), SyntaxKind.SwitchStatement);
    }

    private static void AnalyzeSwitchStatement(SyntaxNodeAnalysisContext context)
    {
        var switchStatement = (SwitchStatementSyntax)context.Node;

        BlankLineBetweenSwitchSections option = context.GetBlankLineBetweenSwitchSections();

        if (option == BlankLineBetweenSwitchSections.None)
            return;

        SyntaxList<SwitchSectionSyntax> sections = switchStatement.Sections;
        SyntaxList<SwitchSectionSyntax>.Enumerator en = sections.GetEnumerator();

        if (!en.MoveNext())
            return;

        SwitchSectionSyntax previousSection = en.Current;
        StatementSyntax previousLastStatement = previousSection.Statements.LastOrDefault();

        while (en.MoveNext())
        {
            TriviaBlockAnalysis analysis = TriviaBlockAnalysis.FromBetween(previousSection, en.Current);

            if (!analysis.Success)
                continue;

            if (analysis.Kind == TriviaBlockKind.BlankLine)
            {
                if (option == BlankLineBetweenSwitchSections.Omit)
                {
                    ReportDiagnostic(context, analysis, "Remove");
                }
                else if (option == BlankLineBetweenSwitchSections.OmitAfterBlock
                    && previousLastStatement.IsKind(SyntaxKind.Block))
                {
                    ReportDiagnostic(context, analysis, "Remove");
                }
            }
            else if (option == BlankLineBetweenSwitchSections.Include)
            {
                ReportDiagnostic(context, analysis, "Add");
            }
            else if (option == BlankLineBetweenSwitchSections.OmitAfterBlock
                && !previousLastStatement.IsKind(SyntaxKind.Block))
            {
                ReportDiagnostic(context, analysis, "Add");
            }

            previousSection = en.Current;
            previousLastStatement = previousSection.Statements.LastOrDefault();
        }
    }

    private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, TriviaBlockAnalysis analysis, string messageArg)
    {
        context.ReportDiagnostic(
            DiagnosticRules.BlankLineBetweenSwitchSections,
            analysis.GetLocation(),
            messageArg);
    }
}
