// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Roslynator.CSharp.CodeStyle;
using Roslynator.CSharp;

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
            SyntaxTriviaList leadingTrivia = en.Current.GetLeadingTrivia();

            SyntaxTrivia blankLine = GetBlankLine(leadingTrivia);

            static SyntaxTrivia GetBlankLine(SyntaxTriviaList leadingTrivia)
            {
                SyntaxTriviaList.Reversed.Enumerator triviaEnumerator = leadingTrivia.Reverse().GetEnumerator();

                if (triviaEnumerator.MoveNext())
                {
                    if (triviaEnumerator.Current.IsWhitespaceTrivia()
                        && !triviaEnumerator.MoveNext())
                    {
                        return default;
                    }

                    if (triviaEnumerator.Current.IsEndOfLineTrivia())
                        return triviaEnumerator.Current;
                }

                return default;
            }

            if (blankLine.IsKind(SyntaxKind.EndOfLineTrivia))
            {
                if (option == BlankLineBetweenSwitchSections.Omit)
                {
                    ReportDiagnostic(context, blankLine);
                }
                else if (option == BlankLineBetweenSwitchSections.Omit_After_Block
                    && previousLastStatement.IsKind(SyntaxKind.Block))
                {
                    ReportDiagnostic(context, blankLine);
                }
            }
            else if (option == BlankLineBetweenSwitchSections.Include)
            {
                ReportDiagnostic(context, previousSection);
            }
            else if (option == BlankLineBetweenSwitchSections.Omit_After_Block
                && !previousLastStatement.IsKind(SyntaxKind.Block))
            {
                ReportDiagnostic(context, previousSection);
            }

            previousSection = en.Current;
            previousLastStatement = previousSection.Statements.LastOrDefault();
        }
    }

    private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, SwitchSectionSyntax previousSection)
    {
        context.ReportDiagnostic(
            DiagnosticRules.BlankLineBetweenSwitchSections,
            Location.Create(context.Node.SyntaxTree, new TextSpan(previousSection.Span.End, 0)));
    }

    private static void ReportDiagnostic(SyntaxNodeAnalysisContext context, SyntaxTrivia blankLine)
    {
        context.ReportDiagnostic(
            DiagnosticRules.BlankLineBetweenSwitchSections,
            Location.Create(context.Node.SyntaxTree, new TextSpan(blankLine.SpanStart, 0)));
    }
}
