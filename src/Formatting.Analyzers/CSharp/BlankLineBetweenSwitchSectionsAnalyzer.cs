// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
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
            TriviaBetweenAnalysis analysis = TriviaBetweenAnalysis.Create(previousSection, en.Current);

            switch (analysis.Kind)
            {
                case TriviaBetweenKind.NoNewLine:
                case TriviaBetweenKind.NewLine:
                    {
                        if (option == BlankLineBetweenSwitchSections.Include)
                        {
                            ReportAddBlankLine(context, analysis.Position);
                        }
                        else if (option == BlankLineBetweenSwitchSections.Omit_After_Block
                            && !previousLastStatement.IsKind(SyntaxKind.Block))
                        {
                            ReportAddBlankLine(context, analysis.Position);
                        }

                        break;
                    }
                case TriviaBetweenKind.BlankLine:
                case TriviaBetweenKind.BlankLines:
                    {
                        if (option == BlankLineBetweenSwitchSections.Omit)
                        {
                            ReportRemoveBlankLine(context, analysis.Position);
                        }
                        else if (option == BlankLineBetweenSwitchSections.Omit_After_Block
                            && previousLastStatement.IsKind(SyntaxKind.Block))
                        {
                            ReportRemoveBlankLine(context, analysis.Position);
                        }

                        break;
                    }
                case TriviaBetweenKind.Unknown:
                case TriviaBetweenKind.PreprocessorDirective:
                    {
                        break;
                    }
                default:
                    {
                        Debug.Fail(analysis.Kind.ToString());
                        break;
                    }
            }

            previousSection = en.Current;
            previousLastStatement = previousSection.Statements.LastOrDefault();
        }
    }

    private static void ReportAddBlankLine(SyntaxNodeAnalysisContext context, int position)
    {
        context.ReportDiagnostic(
            DiagnosticRules.BlankLineBetweenSwitchSections,
            Location.Create(context.Node.SyntaxTree, new TextSpan(position, 0)),
            "Add");
    }

    private static void ReportRemoveBlankLine(SyntaxNodeAnalysisContext context, int position)
    {
        context.ReportDiagnostic(
            DiagnosticRules.BlankLineBetweenSwitchSections,
            Location.Create(context.Node.SyntaxTree, new TextSpan(position, 0)),
            "Remove");
    }
}
