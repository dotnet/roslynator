// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.Formatting.CSharp;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AddBlankLineBeforeUsingDirectiveListAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.AddBlankLineBeforeUsingDirectiveList);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(f => AnalyzeCompilationUnit(f), SyntaxKind.CompilationUnit);
    }

    private static void AnalyzeCompilationUnit(SyntaxNodeAnalysisContext context)
    {
        var compilationUnit = (CompilationUnitSyntax)context.Node;

        UsingDirectiveSyntax usingDirective = compilationUnit.Usings.FirstOrDefault();

        if (usingDirective is null)
            return;

        SyntaxToken previousToken = usingDirective.GetFirstToken().GetPreviousToken();

        TriviaBlockAnalysis analysis;
        analysis = (previousToken.IsKind(SyntaxKind.None))
            ? TriviaBlockAnalysis.AnalyzeBefore(usingDirective)
            : TriviaBlockAnalysis.AnalyzeBetween(previousToken, usingDirective);

        if (analysis.Kind == TriviaBlockKind.NoNewLine
            || analysis.Kind == TriviaBlockKind.NewLine)
        {
            DiagnosticHelpers.ReportDiagnostic(
                context,
                DiagnosticRules.AddBlankLineBeforeUsingDirectiveList,
                analysis.GetLocation());
        }
    }
}
