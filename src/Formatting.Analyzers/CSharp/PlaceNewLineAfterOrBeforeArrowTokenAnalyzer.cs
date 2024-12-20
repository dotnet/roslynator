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
public sealed class PlaceNewLineAfterOrBeforeArrowTokenAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, FormattingDiagnosticRules.PlaceNewLineAfterOrBeforeArrowToken);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(f => AnalyzeArrowExpressionClause(f), SyntaxKind.ArrowExpressionClause);
    }

    private static void AnalyzeArrowExpressionClause(SyntaxNodeAnalysisContext context)
    {
        var arrowExpressionClause = (ArrowExpressionClauseSyntax)context.Node;

        SyntaxToken arrowToken = arrowExpressionClause.ArrowToken;

        NewLinePosition newLinePosition = context.GetArrowTokenNewLinePosition();

        TriviaBlock block = TriviaBlock.FromSurrounding(arrowToken, arrowExpressionClause.Expression, newLinePosition);

        if (block.Success)
        {
            DiagnosticHelpers.ReportDiagnostic(
                context,
                FormattingDiagnosticRules.PlaceNewLineAfterOrBeforeArrowToken,
                block.GetLocation(),
                (newLinePosition == NewLinePosition.Before) ? "before" : "after");
        }
    }
}
