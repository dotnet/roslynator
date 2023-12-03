// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp;

namespace Roslynator.Formatting.CSharp;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AddBlankLineAfterTopCommentAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.AddBlankLineAfterTopComment);

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

        SyntaxNode node = compilationUnit.Externs.FirstOrDefault()
            ?? compilationUnit.Usings.FirstOrDefault()
            ?? (SyntaxNode)compilationUnit.AttributeLists.FirstOrDefault()
            ?? compilationUnit.Members.FirstOrDefault();

        if (node is null)
            return;

        TriviaBlockAnalysis analysis = SyntaxTriviaAnalysis.AnalyzeBefore(node);

        if (analysis.Kind == TriviaBlockKind.NewLine)
        {
            DiagnosticHelpers.ReportDiagnostic(
                context,
                DiagnosticRules.AddBlankLineAfterTopComment,
                analysis.GetLocation());
        }
    }
}
