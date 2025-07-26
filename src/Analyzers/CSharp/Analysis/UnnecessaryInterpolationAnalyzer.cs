﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslynator.CSharp.Syntax;

namespace Roslynator.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UnnecessaryInterpolationAnalyzer : BaseDiagnosticAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, DiagnosticRules.UnnecessaryInterpolation);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        base.Initialize(context);

        context.RegisterSyntaxNodeAction(f => AnalyzeInterpolation(f), SyntaxKind.Interpolation);
    }

    private static void AnalyzeInterpolation(SyntaxNodeAnalysisContext context)
    {
        var interpolation = (InterpolationSyntax)context.Node;

        if (interpolation.AlignmentClause is not null)
            return;

        if (interpolation.FormatClause is not null)
            return;

        StringLiteralExpressionInfo stringLiteralInfo = SyntaxInfo.StringLiteralExpressionInfo(interpolation.Expression);

        if (!stringLiteralInfo.Success)
            return;

        if (interpolation.Parent is not InterpolatedStringExpressionSyntax interpolatedString)
            return;

        if (interpolatedString.StringStartToken.ValueText.Contains("@") != stringLiteralInfo.IsVerbatim)
            return;

#if ROSLYN_4_2
        if (interpolatedString.StringStartToken.IsKind(SyntaxKind.InterpolatedSingleLineRawStringStartToken, SyntaxKind.InterpolatedMultiLineRawStringStartToken)
            && stringLiteralInfo.ContainsEscapeSequence)
        {
            return;
        }
#endif

        DiagnosticHelpers.ReportDiagnostic(context, DiagnosticRules.UnnecessaryInterpolation, interpolation);
    }
}
